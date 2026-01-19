import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/data/models/citizen_submission_status.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';

// Mock Datasource
class MockCitizenRemoteDataSource extends Mock
    implements CitizenRemoteDataSource {}

// Fake classes for registerFallbackValue
class FakeCitizenDetailsModel extends Fake implements CitizenDetailsModel {}

void main() {
  late CitizenRepository repository;
  late MockCitizenRemoteDataSource mockDatasource;

  setUpAll(() {
    registerFallbackValue(FakeCitizenDetailsModel());
  });

  setUp(() {
    mockDatasource = MockCitizenRemoteDataSource();
    repository = CitizenRepository(mockDatasource);
  });

  group('CitizenRepository - getCitizenDetails', () {
    final testCitizenDetails = CitizenDetailsModel(
      name: 'John',
      surname: 'Doe',
      email: 'john@example.com',
      dateOfBirth: DateTime(1990, 1, 15),
      creationDate: DateTime(2024, 1, 1),
      pesel: '12345678901',
      facilityName: 'Test Hospital',
      submissionStatus: 'approved',
    );

    test('returns CitizenDetailsModel on success', () async {
      // Arrange
      when(
        () => mockDatasource.fetchCitizenDetails(),
      ).thenAnswer((_) async => testCitizenDetails);

      // Act
      final result = await repository.getCitizenDetails();

      // Assert
      expect(result.name, equals('John'));
      expect(result.surname, equals('Doe'));
      expect(result.email, equals('john@example.com'));
      expect(result.pesel, equals('12345678901'));
      expect(result.facilityName, equals('Test Hospital'));
      expect(result.submissionStatus, equals('approved'));

      verify(() => mockDatasource.fetchCitizenDetails()).called(1);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.fetchCitizenDetails()).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 404,
            data: 'User not found',
            requestOptions: RequestOptions(path: '/citizens/citizen-details'),
          ),
          requestOptions: RequestOptions(path: '/citizens/citizen-details'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.getCitizenDetails(),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('User not found'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(() => mockDatasource.fetchCitizenDetails()).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(path: '/citizens/citizen-details'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.getCitizenDetails(),
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
        () => mockDatasource.fetchCitizenDetails(),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.getCitizenDetails(),
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

  group('CitizenRepository - createCitizenDetails', () {
    final testCitizenDetails = CitizenDetailsModel(
      name: 'John',
      surname: 'Doe',
      email: 'john@example.com',
      dateOfBirth: DateTime(1990, 1, 15),
      creationDate: DateTime(2024, 1, 1),
      pesel: '12345678901',
      facilityName: 'Test Hospital',
      submissionStatus: '',
    );

    test('successfully creates citizen details', () async {
      // Arrange
      when(
        () => mockDatasource.createCitizenDetails(testCitizenDetails),
      ).thenAnswer((_) async {});

      // Act
      await repository.createCitizenDetails(testCitizenDetails);

      // Assert
      verify(
        () => mockDatasource.createCitizenDetails(testCitizenDetails),
      ).called(1);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.createCitizenDetails(any())).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 400,
            data: 'Invalid data',
            requestOptions: RequestOptions(
              path: '/citizens/new-citizen-details',
            ),
          ),
          requestOptions: RequestOptions(path: '/citizens/new-citizen-details'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.createCitizenDetails(testCitizenDetails),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Invalid data'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(() => mockDatasource.createCitizenDetails(any())).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(
              path: '/citizens/new-citizen-details',
            ),
          ),
        );

        // Act & Assert
        expect(
          () => repository.createCitizenDetails(testCitizenDetails),
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
        () => mockDatasource.createCitizenDetails(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.createCitizenDetails(testCitizenDetails),
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

  group('CitizenRepository - getCitizenSubmissionStatus', () {
    test('returns CitizenSubmissionStatus on success', () async {
      // Arrange
      final testStatus = CitizenSubmissionStatus(submissionStatus: 'Accepted');

      when(
        () => mockDatasource.fetchCitizenSubmissionStatus(),
      ).thenAnswer((_) async => testStatus);

      // Act
      final result = await repository.getCitizenSubmissionStatus();

      // Assert
      expect(result.submissionStatus, equals('Accepted'));

      verify(() => mockDatasource.fetchCitizenSubmissionStatus()).called(1);
    });

    test('returns pending status', () async {
      // Arrange
      final testStatus = CitizenSubmissionStatus(submissionStatus: 'Pending');

      when(
        () => mockDatasource.fetchCitizenSubmissionStatus(),
      ).thenAnswer((_) async => testStatus);

      // Act
      final result = await repository.getCitizenSubmissionStatus();

      // Assert
      expect(result.submissionStatus, equals('Pending'));
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.fetchCitizenSubmissionStatus()).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: 'Server Error',
            requestOptions: RequestOptions(path: '/citizens/submission-status'),
          ),
          requestOptions: RequestOptions(path: '/citizens/submission-status'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.getCitizenSubmissionStatus(),
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
        when(() => mockDatasource.fetchCitizenSubmissionStatus()).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(path: '/citizens/submission-status'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.getCitizenSubmissionStatus(),
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
        () => mockDatasource.fetchCitizenSubmissionStatus(),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.getCitizenSubmissionStatus(),
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
