import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/medical_facility_remote_datasource.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';
import 'package:mszczepienia/data/repositories/medical_facility_repository.dart';

// Mock MedicalFacilityRemoteDatasource
class MockMedicalFacilityRemoteDatasource extends Mock
    implements MedicalFacilityRemoteDatasource {}

void main() {
  late MockMedicalFacilityRemoteDatasource mockDatasource;
  late MedicalFacilityRepository repository;

  final testFacilities = [
    MedicalFacilityModel(facilityId: 1, facilityName: 'Hospital A'),
    MedicalFacilityModel(facilityId: 2, facilityName: 'Clinic B'),
  ];

  final query = 'szpital';

  setUp(() {
    mockDatasource = MockMedicalFacilityRemoteDatasource();
    repository = MedicalFacilityRepository(mockDatasource);
  });

  group('MedicalFacilityRepository', () {
    group('getMedicalFacilities', () {
      test('returns list of facilities on success', () async {
        when(
          () => mockDatasource.fetchMedicalFacilities(query),
        ).thenAnswer((_) async => testFacilities);

        final result = await repository.getMedicalFacilities(query);

        expect(result.length, 2);
        expect(result[0].facilityId, 1);
        expect(result[0].facilityName, 'Hospital A');
        expect(result[1].facilityId, 2);
        expect(result[1].facilityName, 'Clinic B');
        verify(() => mockDatasource.fetchMedicalFacilities(query)).called(1);
      });

      test('returns empty list when no facilities', () async {
        when(
          () => mockDatasource.fetchMedicalFacilities(query),
        ).thenAnswer((_) async => []);

        final result = await repository.getMedicalFacilities(query);

        expect(result, isEmpty);
      });

      test(
        'throws exception with message on DioException with response data',
        () async {
          when(() => mockDatasource.fetchMedicalFacilities(query)).thenThrow(
            DioException(
              requestOptions: RequestOptions(
                path: '/facilities/facility-names',
              ),
              response: Response(
                requestOptions: RequestOptions(
                  path: '/facilities/facility-names',
                ),
                data: 'Server Error Message',
              ),
            ),
          );

          expect(
            () => repository.getMedicalFacilities(query),
            throwsA(
              isA<Exception>().having(
                (e) => e.toString(),
                'message',
                contains('Server Error Message'),
              ),
            ),
          );
        },
      );

      test(
        'throws exception with Connection Error on DioException without response',
        () async {
          when(() => mockDatasource.fetchMedicalFacilities(query)).thenThrow(
            DioException(
              requestOptions: RequestOptions(
                path: '/facilities/facility-names',
              ),
            ),
          );

          expect(
            () => repository.getMedicalFacilities(query),
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

      test(
        'throws exception with Unknown Error on generic exception',
        () async {
          when(
            () => mockDatasource.fetchMedicalFacilities(query),
          ).thenThrow(Exception('Some random error'));

          expect(
            () => repository.getMedicalFacilities(query),
            throwsA(
              isA<Exception>().having(
                (e) => e.toString(),
                'message',
                contains('Unknown Error'),
              ),
            ),
          );
        },
      );

      test(
        'throws exception with Unknown Error on non-Exception error',
        () async {
          when(
            () => mockDatasource.fetchMedicalFacilities(query),
          ).thenThrow('String error');

          expect(
            () => repository.getMedicalFacilities(query),
            throwsA(
              isA<Exception>().having(
                (e) => e.toString(),
                'message',
                contains('Unknown Error'),
              ),
            ),
          );
        },
      );
    });
  });
}
