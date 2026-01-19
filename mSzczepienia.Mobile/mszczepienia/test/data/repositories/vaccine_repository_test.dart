import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/vaccine_remote_datasource.dart';
import 'package:mszczepienia/data/models/vaccine_name_model.dart';
import 'package:mszczepienia/data/repositories/vaccine_repository.dart';

// Mock Datasource
class MockVaccineRemoteDatasource extends Mock
    implements VaccineRemoteDatasource {}

void main() {
  late VaccineRepository repository;
  late MockVaccineRemoteDatasource mockDatasource;

  setUp(() {
    mockDatasource = MockVaccineRemoteDatasource();
    repository = VaccineRepository(mockDatasource);
  });

  group('VaccineRepository - getVaccineNames', () {
    test('returns list of VaccineNameModel on success', () async {
      // Arrange
      final vaccineNames = [
        VaccineNameModel(vaccineId: 1, vaccineName: 'BCG'),
        VaccineNameModel(vaccineId: 2, vaccineName: 'MMR'),
      ];

      when(
        () => mockDatasource.fetchVaccineNames('test'),
      ).thenAnswer((_) async => vaccineNames);

      // Act
      final result = await repository.getVaccineNames('test');

      // Assert
      expect(result.length, equals(2));
      expect(result[0].vaccineId, equals(1));
      expect(result[0].vaccineName, equals('BCG'));
      expect(result[1].vaccineId, equals(2));
      expect(result[1].vaccineName, equals('MMR'));

      verify(() => mockDatasource.fetchVaccineNames('test')).called(1);
    });

    test('returns empty list when no vaccines found', () async {
      // Arrange
      when(
        () => mockDatasource.fetchVaccineNames('nonexistent'),
      ).thenAnswer((_) async => []);

      // Act
      final result = await repository.getVaccineNames('nonexistent');

      // Assert
      expect(result, isEmpty);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.fetchVaccineNames(any())).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: 'Server Error',
            requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
          ),
          requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.getVaccineNames('test'),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Server Error'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(() => mockDatasource.fetchVaccineNames(any())).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(path: '/vaccines/vaccine-names'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.getVaccineNames('test'),
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
        () => mockDatasource.fetchVaccineNames(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.getVaccineNames('test'),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Unknown Error'),
          ),
        ),
      );
    });

    test('passes query string correctly to datasource', () async {
      // Arrange
      const searchQuery = 'hepatitis';
      when(
        () => mockDatasource.fetchVaccineNames(searchQuery),
      ).thenAnswer((_) async => []);

      // Act
      await repository.getVaccineNames(searchQuery);

      // Assert
      verify(() => mockDatasource.fetchVaccineNames(searchQuery)).called(1);
    });
  });
}
