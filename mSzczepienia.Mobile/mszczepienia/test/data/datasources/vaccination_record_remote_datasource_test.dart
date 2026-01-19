import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/vaccination_record_remote_datasource.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';

// Mock Dio
class MockDio extends Mock implements Dio {}

void main() {
  late VaccinationRecordRemoteDatasource dataSource;
  late MockDio mockDio;

  setUp(() {
    mockDio = MockDio();
    dataSource = VaccinationRecordRemoteDatasource(mockDio);
  });

  group('VaccinationRecordRemoteDatasource - fetchVaccinationHistory', () {
    final testQuery = VaccinationRecordOverviewQuery(
      pageNumber: 1,
      pageSize: 10,
      sortingField: 'vaccinationDate',
      sortingDirection: 'desc',
    );

    test(
      'returns list of VaccinationRecordOverviewModel on successful API call',
      () async {
        // Arrange
        final responseData = [
          {
            'id': 1,
            'vaccineName': 'BCG',
            'shortDescriptionEn': 'Tuberculosis vaccine',
            'shortDescriptionPl': 'Szczepionka przeciw gruźlicy',
            'vaccinationDate': '2023-05-15T00:00:00.000',
            'dosesNumber': 1,
          },
          {
            'id': 2,
            'vaccineName': 'MMR',
            'shortDescriptionEn': 'Measles, Mumps, Rubella vaccine',
            'shortDescriptionPl': 'Szczepionka MMR',
            'vaccinationDate': '2023-06-20T00:00:00.000',
            'dosesNumber': 2,
          },
        ];

        when(
          () => mockDio.get(
            '/vaccination-records/history',
            queryParameters: testQuery.toJson(),
          ),
        ).thenAnswer(
          (_) async => Response(
            data: responseData,
            statusCode: 200,
            requestOptions: RequestOptions(
              path: '/vaccination-records/history',
            ),
          ),
        );

        // Act
        final result = await dataSource.fetchVaccinationHistory(testQuery);

        // Assert
        expect(result.length, equals(2));
        expect(result[0].id, equals(1));
        expect(result[0].vaccineName, equals('BCG'));
        expect(result[0].shortDesciriptionEn, equals('Tuberculosis vaccine'));
        expect(result[0].dosesNumber, equals(1));
        expect(result[1].id, equals(2));
        expect(result[1].vaccineName, equals('MMR'));

        verify(
          () => mockDio.get(
            '/vaccination-records/history',
            queryParameters: testQuery.toJson(),
          ),
        ).called(1);
      },
    );

    test('returns empty list when no records found', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: testQuery.toJson(),
        ),
      ).thenAnswer(
        (_) async => Response(
          data: [],
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccinationHistory(testQuery);

      // Assert
      expect(result, isEmpty);
    });

    test('handles response as Map with items key', () async {
      // Arrange
      final responseData = {
        'items': [
          {
            'id': 1,
            'vaccineName': 'BCG',
            'shortDescriptionEn': 'Tuberculosis vaccine',
            'shortDescriptionPl': 'Szczepionka przeciw gruźlicy',
            'vaccinationDate': '2023-05-15T00:00:00.000',
            'dosesNumber': 1,
          },
        ],
        'totalCount': 1,
      };

      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: testQuery.toJson(),
        ),
      ).thenAnswer(
        (_) async => Response(
          data: responseData,
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccinationHistory(testQuery);

      // Assert
      expect(result.length, equals(1));
      expect(result[0].vaccineName, equals('BCG'));
    });

    test('handles response as Map without items key', () async {
      // Arrange
      final responseData = {'totalCount': 0};

      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: testQuery.toJson(),
        ),
      ).thenAnswer(
        (_) async => Response(
          data: responseData,
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccinationHistory(testQuery);

      // Assert
      expect(result, isEmpty);
    });

    test('throws DioException on API error', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: any(named: 'queryParameters'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: {'error': 'Internal server error'},
            requestOptions: RequestOptions(
              path: '/vaccination-records/history',
            ),
          ),
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchVaccinationHistory(testQuery),
        throwsA(isA<DioException>()),
      );
    });

    test('throws DioException on connection timeout', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: any(named: 'queryParameters'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.connectionTimeout,
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchVaccinationHistory(testQuery),
        throwsA(
          isA<DioException>().having(
            (e) => e.type,
            'type',
            DioExceptionType.connectionTimeout,
          ),
        ),
      );
    });

    test('sends correct query parameters', () async {
      // Arrange
      final customQuery = VaccinationRecordOverviewQuery(
        pageNumber: 2,
        pageSize: 20,
        sortingField: 'vaccineName',
        sortingDirection: 'asc',
      );

      when(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: customQuery.toJson(),
        ),
      ).thenAnswer(
        (_) async => Response(
          data: [],
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act
      await dataSource.fetchVaccinationHistory(customQuery);

      // Assert
      verify(
        () => mockDio.get(
          '/vaccination-records/history',
          queryParameters: {
            'pageNumber': 2,
            'pageSize': 20,
            'sortingField': 'vaccineName',
            'sortingDirection': 'asc',
          },
        ),
      ).called(1);
    });
  });

  group(
    'VaccinationRecordRemoteDatasource - fetchVaccinationRecordsByVaccineId',
    () {
      final testQuery = VaccinationRecordDetailsQuery(vaccineId: 1);

      test(
        'returns list of VaccinationRecordDetailsModel on successful API call',
        () async {
          // Arrange
          final responseData = [
            {
              'vaccineId': 1,
              'vaccineName': 'BCG',
              'manufacturer': 'Manufacturer A',
              'vaccineDescriptionPl': 'Opis PL',
              'vaccineDescriptionEn': 'Description EN',
              'shortDescriptionPl': 'Krótki opis',
              'shortDescriptionEn': 'Short desc',
              'creationDate': '2020-01-01T00:00:00.000',
              'diseasesPl': ['Gruźlica'],
              'diseasesEn': ['Tuberculosis'],
              'vaccinationDate': '2023-05-15T00:00:00.000',
              'nextVaccinationDate': null,
              'doseNumber': '1',
              'batch': 'BATCH001',
              'remarks': null,
            },
          ];

          when(
            () => mockDio.get(
              '/vaccination-records/details',
              queryParameters: testQuery.toJson(),
            ),
          ).thenAnswer(
            (_) async => Response(
              data: responseData,
              statusCode: 200,
              requestOptions: RequestOptions(
                path: '/vaccination-records/details',
              ),
            ),
          );

          // Act
          final result = await dataSource.fetchVaccinationRecordsByVaccineId(
            testQuery,
          );

          // Assert
          expect(result.length, equals(1));
          expect(result[0].vaccineId, equals(1));
          expect(result[0].vaccineName, equals('BCG'));
          expect(result[0].manufacturer, equals('Manufacturer A'));
          expect(result[0].doseNumber, equals('1'));
          expect(result[0].batch, equals('BATCH001'));

          verify(
            () => mockDio.get(
              '/vaccination-records/details',
              queryParameters: testQuery.toJson(),
            ),
          ).called(1);
        },
      );

      test('returns empty list when no details found', () async {
        // Arrange
        when(
          () => mockDio.get(
            '/vaccination-records/details',
            queryParameters: testQuery.toJson(),
          ),
        ).thenAnswer(
          (_) async => Response(
            data: [],
            statusCode: 200,
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
        );

        // Act
        final result = await dataSource.fetchVaccinationRecordsByVaccineId(
          testQuery,
        );

        // Assert
        expect(result, isEmpty);
      });

      test('returns multiple records for same vaccine', () async {
        // Arrange
        final responseData = [
          {
            'vaccineId': 1,
            'vaccineName': 'MMR',
            'manufacturer': 'Manufacturer A',
            'vaccineDescriptionPl': 'Opis PL',
            'vaccineDescriptionEn': 'Description EN',
            'shortDescriptionPl': 'Krótki opis',
            'shortDescriptionEn': 'Short desc',
            'creationDate': '2020-01-01T00:00:00.000',
            'diseasesPl': ['Odra', 'Świnka', 'Różyczka'],
            'diseasesEn': ['Measles', 'Mumps', 'Rubella'],
            'vaccinationDate': '2023-05-15T00:00:00.000',
            'nextVaccinationDate': '2024-05-15T00:00:00.000',
            'doseNumber': '1',
            'batch': 'BATCH001',
            'remarks': 'First dose',
          },
          {
            'vaccineId': 1,
            'vaccineName': 'MMR',
            'manufacturer': 'Manufacturer A',
            'vaccineDescriptionPl': 'Opis PL',
            'vaccineDescriptionEn': 'Description EN',
            'shortDescriptionPl': 'Krótki opis',
            'shortDescriptionEn': 'Short desc',
            'creationDate': '2020-01-01T00:00:00.000',
            'diseasesPl': ['Odra', 'Świnka', 'Różyczka'],
            'diseasesEn': ['Measles', 'Mumps', 'Rubella'],
            'vaccinationDate': '2024-05-15T00:00:00.000',
            'nextVaccinationDate': null,
            'doseNumber': '2',
            'batch': 'BATCH002',
            'remarks': 'Second dose',
          },
        ];

        when(
          () => mockDio.get(
            '/vaccination-records/details',
            queryParameters: testQuery.toJson(),
          ),
        ).thenAnswer(
          (_) async => Response(
            data: responseData,
            statusCode: 200,
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
        );

        // Act
        final result = await dataSource.fetchVaccinationRecordsByVaccineId(
          testQuery,
        );

        // Assert
        expect(result.length, equals(2));
        expect(result[0].doseNumber, equals('1'));
        expect(result[1].doseNumber, equals('2'));
      });

      test('throws DioException on API error', () async {
        // Arrange
        when(
          () => mockDio.get(
            '/vaccination-records/details',
            queryParameters: any(named: 'queryParameters'),
          ),
        ).thenThrow(
          DioException(
            type: DioExceptionType.badResponse,
            response: Response(
              statusCode: 404,
              data: {'error': 'Vaccine not found'},
              requestOptions: RequestOptions(
                path: '/vaccination-records/details',
              ),
            ),
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
        );

        // Act & Assert
        expect(
          () => dataSource.fetchVaccinationRecordsByVaccineId(testQuery),
          throwsA(isA<DioException>()),
        );
      });

      test('sends correct vaccineId in query parameters', () async {
        // Arrange
        final customQuery = VaccinationRecordDetailsQuery(vaccineId: 42);

        when(
          () => mockDio.get(
            '/vaccination-records/details',
            queryParameters: customQuery.toJson(),
          ),
        ).thenAnswer(
          (_) async => Response(
            data: [],
            statusCode: 200,
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
        );

        // Act
        await dataSource.fetchVaccinationRecordsByVaccineId(customQuery);

        // Assert
        verify(
          () => mockDio.get(
            '/vaccination-records/details',
            queryParameters: {'vaccineId': 42},
          ),
        ).called(1);
      });
    },
  );
}
