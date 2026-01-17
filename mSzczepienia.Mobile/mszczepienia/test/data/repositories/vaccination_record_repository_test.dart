import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/vaccination_record_remote_datasource.dart';
import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';

// Mock Datasource
class MockVaccinationRecordRemoteDatasource extends Mock
    implements VaccinationRecordRemoteDatasource {}

// Fake classes for registerFallbackValue
class FakeVaccinationRecordOverviewQuery extends Fake
    implements VaccinationRecordOverviewQuery {}

class FakeVaccinationRecordDetailsQuery extends Fake
    implements VaccinationRecordDetailsQuery {}

void main() {
  late VaccinationRecordRepository repository;
  late MockVaccinationRecordRemoteDatasource mockDatasource;

  setUpAll(() {
    registerFallbackValue(FakeVaccinationRecordOverviewQuery());
    registerFallbackValue(FakeVaccinationRecordDetailsQuery());
  });

  setUp(() {
    mockDatasource = MockVaccinationRecordRemoteDatasource();
    repository = VaccinationRecordRepository(mockDatasource);
  });

  group('VaccinationRecordRepository - getVaccinationHistory', () {
    final testQuery = VaccinationRecordOverviewQuery(
      pageNumber: 1,
      pageSize: 10,
      sortingField: 'vaccinationDate',
      sortingDirection: 'desc',
    );

    test('returns list of VaccinationRecordOverviewModel on success', () async {
      // Arrange
      final records = [
        VaccinationRecordOverviewModel(
          id: 1,
          vaccineName: 'BCG',
          shortDesciriptionEn: 'Tuberculosis vaccine',
          shortDescriptionPl: 'Szczepionka przeciw gruźlicy',
          vaccinationDate: DateTime(2023, 5, 15),
          dosesNumber: 1,
        ),
        VaccinationRecordOverviewModel(
          id: 2,
          vaccineName: 'MMR',
          shortDesciriptionEn: 'MMR vaccine',
          shortDescriptionPl: 'Szczepionka MMR',
          vaccinationDate: DateTime(2023, 6, 20),
          dosesNumber: 2,
        ),
      ];

      when(
        () => mockDatasource.fetchVaccinationHistory(testQuery),
      ).thenAnswer((_) async => records);

      // Act
      final result = await repository.getVaccinationHistory(testQuery);

      // Assert
      expect(result.length, equals(2));
      expect(result[0].id, equals(1));
      expect(result[0].vaccineName, equals('BCG'));
      expect(result[1].id, equals(2));
      expect(result[1].vaccineName, equals('MMR'));

      verify(() => mockDatasource.fetchVaccinationHistory(testQuery)).called(1);
    });

    test('returns empty list when no records found', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccinationHistory(testQuery),
      ).thenAnswer((_) async => []);

      // Act
      final result = await repository.getVaccinationHistory(testQuery);

      // Assert
      expect(result, isEmpty);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.fetchVaccinationHistory(any())).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: 'Internal Server Error',
            requestOptions: RequestOptions(
              path: '/vaccination-records/history',
            ),
          ),
          requestOptions: RequestOptions(path: '/vaccination-records/history'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.getVaccinationHistory(testQuery),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Internal Server Error'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(() => mockDatasource.fetchVaccinationHistory(any())).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(
              path: '/vaccination-records/history',
            ),
          ),
        );

        // Act & Assert
        expect(
          () => repository.getVaccinationHistory(testQuery),
          throwsA(
            isA<Exception>().having(
              (e) => e.toString(),
              'message',
              contains('Connection Error'),
            ),
          ),
        );
      },
    );

    test('throws exception with Unknown Error on general exception', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccinationHistory(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.getVaccinationHistory(testQuery),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Unknown Error'),
          ),
        ),
      );
    });
  });

  group('VaccinationRecordRepository - getVaccinationRecordsByVaccineId', () {
    final testQuery = VaccinationRecordDetailsQuery(vaccineId: 1);

    test('returns list of VaccinationRecordDetailsModel on success', () async {
      // Arrange
      final records = [
        VaccinationRecordDetailsModel(
          vaccineId: 1,
          vaccineName: 'BCG',
          manufacturer: 'Manufacturer A',
          vaccineDescriptionPl: 'Opis PL',
          vaccineDescriptionEn: 'Description EN',
          shortDescriptionPl: 'Krótki opis',
          shortDescriptionEn: 'Short desc',
          creationDate: DateTime(2020, 1, 1),
          diseasesPl: ['Gruźlica'],
          diseasesEn: ['Tuberculosis'],
          vaccinationDate: DateTime(2023, 5, 15),
          doseNumber: '1',
          batch: 'BATCH001',
        ),
      ];

      when(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(testQuery),
      ).thenAnswer((_) async => records);

      // Act
      final result = await repository.getVaccinationRecordsByVaccineId(
        testQuery,
      );

      // Assert
      expect(result.length, equals(1));
      expect(result[0].vaccineId, equals(1));
      expect(result[0].vaccineName, equals('BCG'));
      expect(result[0].doseNumber, equals('1'));

      verify(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(testQuery),
      ).called(1);
    });

    test('returns empty list when no details found', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(testQuery),
      ).thenAnswer((_) async => []);

      // Act
      final result = await repository.getVaccinationRecordsByVaccineId(
        testQuery,
      );

      // Assert
      expect(result, isEmpty);
    });

    test('returns multiple records for same vaccine', () async {
      // Arrange
      final records = [
        VaccinationRecordDetailsModel(
          vaccineId: 1,
          vaccineName: 'MMR',
          manufacturer: 'Manufacturer A',
          vaccineDescriptionPl: 'Opis PL',
          vaccineDescriptionEn: 'Description EN',
          shortDescriptionPl: 'Krótki opis',
          shortDescriptionEn: 'Short desc',
          creationDate: DateTime(2020, 1, 1),
          diseasesPl: ['Odra', 'Świnka', 'Różyczka'],
          diseasesEn: ['Measles', 'Mumps', 'Rubella'],
          vaccinationDate: DateTime(2023, 5, 15),
          doseNumber: '1',
          batch: 'BATCH001',
        ),
        VaccinationRecordDetailsModel(
          vaccineId: 1,
          vaccineName: 'MMR',
          manufacturer: 'Manufacturer A',
          vaccineDescriptionPl: 'Opis PL',
          vaccineDescriptionEn: 'Description EN',
          shortDescriptionPl: 'Krótki opis',
          shortDescriptionEn: 'Short desc',
          creationDate: DateTime(2020, 1, 1),
          diseasesPl: ['Odra', 'Świnka', 'Różyczka'],
          diseasesEn: ['Measles', 'Mumps', 'Rubella'],
          vaccinationDate: DateTime(2024, 5, 15),
          doseNumber: '2',
          batch: 'BATCH002',
        ),
      ];

      when(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(testQuery),
      ).thenAnswer((_) async => records);

      // Act
      final result = await repository.getVaccinationRecordsByVaccineId(
        testQuery,
      );

      // Assert
      expect(result.length, equals(2));
      expect(result[0].doseNumber, equals('1'));
      expect(result[1].doseNumber, equals('2'));
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(any()),
      ).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 404,
            data: 'Vaccine not found',
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
          requestOptions: RequestOptions(path: '/vaccination-records/details'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.getVaccinationRecordsByVaccineId(testQuery),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Vaccine not found'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(
          () => mockDatasource.fetchVaccinationRecordsByVaccineId(any()),
        ).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(
              path: '/vaccination-records/details',
            ),
          ),
        );

        // Act & Assert
        expect(
          () => repository.getVaccinationRecordsByVaccineId(testQuery),
          throwsA(
            isA<Exception>().having(
              (e) => e.toString(),
              'message',
              contains('Connection Error'),
            ),
          ),
        );
      },
    );

    test('throws exception with Unknown Error on general exception', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccinationRecordsByVaccineId(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.getVaccinationRecordsByVaccineId(testQuery),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Unknown Error'),
          ),
        ),
      );
    });
  });
}
