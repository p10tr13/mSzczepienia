import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/vaccine_remote_datasource.dart';

// Mock Dio
class MockDio extends Mock implements Dio {}

void main() {
  late VaccineRemoteDatasource dataSource;
  late MockDio mockDio;

  setUp(() {
    mockDio = MockDio();
    dataSource = VaccineRemoteDatasource(mockDio);
  });

  group('VaccineRemoteDatasource - fetchVaccineNames', () {
    test('returns list of VaccineNameModel on successful API call', () async {
      // Arrange
      final responseData = [
        {'vaccineId': 1, 'vaccineName': 'BCG'},
        {'vaccineId': 2, 'vaccineName': 'MMR'},
        {'vaccineId': 3, 'vaccineName': 'Hepatitis B'},
      ];

      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': 'test'},
        ),
      ).thenAnswer(
        (_) async => Response(
          data: responseData,
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccineNames('test');

      // Assert
      expect(result.length, equals(3));
      expect(result[0].vaccineId, equals(1));
      expect(result[0].vaccineName, equals('BCG'));
      expect(result[1].vaccineId, equals(2));
      expect(result[1].vaccineName, equals('MMR'));
      expect(result[2].vaccineId, equals(3));
      expect(result[2].vaccineName, equals('Hepatitis B'));

      verify(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': 'test'},
        ),
      ).called(1);
    });

    test('returns empty list when no vaccines found', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': 'nonexistent'},
        ),
      ).thenAnswer(
        (_) async => Response(
          data: [],
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccineNames('nonexistent');

      // Assert
      expect(result, isEmpty);
    });

    test('passes query parameter correctly', () async {
      // Arrange
      const searchQuery = 'hepatitis';

      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': searchQuery},
        ),
      ).thenAnswer(
        (_) async => Response(
          data: [
            {'vaccineId': 1, 'vaccineName': 'Hepatitis A'},
          ],
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act
      await dataSource.fetchVaccineNames(searchQuery);

      // Assert
      verify(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': searchQuery},
        ),
      ).called(1);
    });

    test('throws DioException on API error', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: any(named: 'queryParameters'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: {'error': 'Internal server error'},
            requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
          ),
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchVaccineNames('test'),
        throwsA(isA<DioException>()),
      );
    });

    test('throws DioException on connection timeout', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: any(named: 'queryParameters'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.connectionTimeout,
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchVaccineNames('test'),
        throwsA(
          isA<DioException>().having(
            (e) => e.type,
            'type',
            DioExceptionType.connectionTimeout,
          ),
        ),
      );
    });

    test('handles empty query string', () async {
      // Arrange
      when(
        () => mockDio.get(
          '/vaccines/vaccine-names',
          queryParameters: {'query': ''},
        ),
      ).thenAnswer(
        (_) async => Response(
          data: [
            {'vaccineId': 1, 'vaccineName': 'BCG'},
            {'vaccineId': 2, 'vaccineName': 'MMR'},
          ],
          statusCode: 200,
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act
      final result = await dataSource.fetchVaccineNames('');

      // Assert
      expect(result.length, equals(2));
    });
  });
}
