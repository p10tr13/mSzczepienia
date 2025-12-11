import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/medical_facility_remote_datasource.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';

// Mock Dio
class MockDio extends Mock implements Dio {}

void main() {
  late MockDio mockDio;
  late MedicalFacilityRemoteDatasource datasource;

  setUp(() {
    mockDio = MockDio();
    datasource = MedicalFacilityRemoteDatasource(mockDio);
  });

  group('MedicalFacilityRemoteDatasource', () {
    group('fetchMedicalFacilities', () {
      test('returns list of MedicalFacilityModel on successful response',
          () async {
        final responseData = [
          {'facilityId': 1, 'facilityName': 'Hospital A'},
          {'facilityId': 2, 'facilityName': 'Clinic B'},
          {'facilityId': 3, 'facilityName': 'Medical Center C'},
        ];

        when(() => mockDio.get('/facilities/facility-names')).thenAnswer(
          (_) async => Response(
            data: responseData,
            statusCode: 200,
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        final result = await datasource.fetchMedicalFacilities();

        expect(result, isA<List<MedicalFacilityModel>>());
        expect(result.length, 3);
        expect(result[0].facilityId, 1);
        expect(result[0].facilityName, 'Hospital A');
        expect(result[1].facilityId, 2);
        expect(result[1].facilityName, 'Clinic B');
        expect(result[2].facilityId, 3);
        expect(result[2].facilityName, 'Medical Center C');
      });

      test('returns empty list when response data is empty', () async {
        when(() => mockDio.get('/facilities/facility-names')).thenAnswer(
          (_) async => Response(
            data: [],
            statusCode: 200,
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        final result = await datasource.fetchMedicalFacilities();

        expect(result, isEmpty);
      });

      test('correctly parses facility with special characters in name',
          () async {
        final responseData = [
          {'facilityId': 1, 'facilityName': 'Szpital Św. Jana'},
          {'facilityId': 2, 'facilityName': 'Klinika "Zdrowie"'},
        ];

        when(() => mockDio.get('/facilities/facility-names')).thenAnswer(
          (_) async => Response(
            data: responseData,
            statusCode: 200,
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        final result = await datasource.fetchMedicalFacilities();

        expect(result[0].facilityName, 'Szpital Św. Jana');
        expect(result[1].facilityName, 'Klinika "Zdrowie"');
      });

      test('throws DioException on network error', () async {
        when(() => mockDio.get('/facilities/facility-names')).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        expect(
          () => datasource.fetchMedicalFacilities(),
          throwsA(isA<DioException>()),
        );
      });

      test('throws DioException on server error (500)', () async {
        when(() => mockDio.get('/facilities/facility-names')).thenThrow(
          DioException(
            type: DioExceptionType.badResponse,
            response: Response(
              statusCode: 500,
              data: 'Internal Server Error',
              requestOptions:
                  RequestOptions(path: '/facilities/facility-names'),
            ),
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        expect(
          () => datasource.fetchMedicalFacilities(),
          throwsA(isA<DioException>()),
        );
      });

      test('throws DioException on 404 not found', () async {
        when(() => mockDio.get('/facilities/facility-names')).thenThrow(
          DioException(
            type: DioExceptionType.badResponse,
            response: Response(
              statusCode: 404,
              data: 'Not Found',
              requestOptions:
                  RequestOptions(path: '/facilities/facility-names'),
            ),
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        expect(
          () => datasource.fetchMedicalFacilities(),
          throwsA(isA<DioException>()),
        );
      });

      test('calls correct endpoint', () async {
        when(() => mockDio.get('/facilities/facility-names')).thenAnswer(
          (_) async => Response(
            data: [],
            statusCode: 200,
            requestOptions: RequestOptions(path: '/facilities/facility-names'),
          ),
        );

        await datasource.fetchMedicalFacilities();

        verify(() => mockDio.get('/facilities/facility-names')).called(1);
      });
    });
  });
}
