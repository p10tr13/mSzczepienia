import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';

// Mock Dio
class MockDio extends Mock implements Dio {}

void main() {
  late CitizenRemoteDataSource dataSource;
  late MockDio mockDio;

  setUp(() {
    mockDio = MockDio();
    dataSource = CitizenRemoteDataSource(mockDio);
  });

  group('CitizenRemoteDataSource - fetchCitizenDetails', () {
    test('returns CitizenDetailsModel on successful API call', () async {
      // Arrange
      final responseData = {
        'name': 'John',
        'surname': 'Doe',
        'email': 'john@example.com',
        'dateOfBirth': '1990-01-15T00:00:00.000',
        'creationDate': '2024-01-01T00:00:00.000',
        'pesel': '12345678901',
        'facilityName': 'Test Hospital',
        'submissionStatus': 'approved',
      };

      when(() => mockDio.get('/citizens/citizen-details')).thenAnswer(
        (_) async => Response(
          data: responseData,
          statusCode: 200,
          requestOptions: RequestOptions(path: '/citizens/citizen-details'),
        ),
      );

      // Act
      final result = await dataSource.fetchCitizenDetails();

      // Assert
      expect(result.name, equals('John'));
      expect(result.surname, equals('Doe'));
      expect(result.email, equals('john@example.com'));
      expect(result.pesel, equals('12345678901'));
      expect(result.facilityName, equals('Test Hospital'));
      expect(result.submissionStatus, equals('approved'));
      expect(result.dateOfBirth, equals(DateTime(1990, 1, 15)));

      verify(() => mockDio.get('/citizens/citizen-details')).called(1);
    });

    test('throws DioException on API error', () async {
      // Arrange
      when(() => mockDio.get('/citizens/citizen-details')).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 404,
            data: {'error': 'User not found'},
            requestOptions: RequestOptions(path: '/citizens/citizen-details'),
          ),
          requestOptions: RequestOptions(path: '/citizens/citizen-details'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchCitizenDetails(),
        throwsA(isA<DioException>()),
      );
    });

    test('throws DioException on connection timeout', () async {
      // Arrange
      when(() => mockDio.get('/citizens/citizen-details')).thenThrow(
        DioException(
          type: DioExceptionType.connectionTimeout,
          requestOptions: RequestOptions(path: '/citizens/citizen-details'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.fetchCitizenDetails(),
        throwsA(isA<DioException>().having(
          (e) => e.type,
          'type',
          DioExceptionType.connectionTimeout,
        )),
      );
    });
  });

  group('CitizenRemoteDataSource - createCitizenDetails', () {
    test('successfully creates citizen details', () async {
      // Arrange
      final citizenDetails = CitizenDetailsModel(
        name: 'John',
        surname: 'Doe',
        email: 'john@example.com',
        dateOfBirth: DateTime(1990, 1, 15),
        creationDate: DateTime(2024, 1, 1),
        pesel: '12345678901',
        facilityName: 'Test Hospital',
        submissionStatus: '',
      );

      when(() => mockDio.post(
            '/citizens/new-citizen-details',
            data: any(named: 'data'),
          )).thenAnswer(
        (_) async => Response(
          statusCode: 201,
          requestOptions: RequestOptions(path: '/citizens/new-citizen-details'),
        ),
      );

      // Act
      await dataSource.createCitizenDetails(citizenDetails);

      // Assert
      verify(() => mockDio.post(
            '/citizens/new-citizen-details',
            data: any(named: 'data'),
          )).called(1);
    });

    test('sends correct data in POST request', () async {
      // Arrange
      final citizenDetails = CitizenDetailsModel(
        name: 'John',
        surname: 'Doe',
        email: 'john@example.com',
        dateOfBirth: DateTime(1990, 1, 15),
        creationDate: DateTime(2024, 1, 1),
        pesel: '12345678901',
        facilityName: 'Test Hospital',
        submissionStatus: '',
      );

      Map<String, dynamic>? capturedData;

      when(() => mockDio.post(
            '/citizens/new-citizen-details',
            data: any(named: 'data'),
          )).thenAnswer((invocation) async {
        capturedData = invocation.namedArguments[const Symbol('data')];
        return Response(
          statusCode: 201,
          requestOptions: RequestOptions(path: '/citizens/new-citizen-details'),
        );
      });

      // Act
      await dataSource.createCitizenDetails(citizenDetails);

      // Assert
      expect(capturedData, isNotNull);
      expect(capturedData!['name'], equals('John'));
      expect(capturedData!['surname'], equals('Doe'));
      expect(capturedData!['email'], equals('john@example.com'));
      expect(capturedData!['pesel'], equals('12345678901'));
      expect(capturedData!['facilityName'], equals('Test Hospital'));
    });

    test('throws DioException on server error', () async {
      // Arrange
      final citizenDetails = CitizenDetailsModel(
        name: 'John',
        surname: 'Doe',
        email: 'john@example.com',
        dateOfBirth: DateTime(1990, 1, 15),
        creationDate: DateTime(2024, 1, 1),
        pesel: '12345678901',
        facilityName: 'Test Hospital',
        submissionStatus: '',
      );

      when(() => mockDio.post(
            '/citizens/new-citizen-details',
            data: any(named: 'data'),
          )).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: {'error': 'Internal server error'},
            requestOptions:
                RequestOptions(path: '/citizens/new-citizen-details'),
          ),
          requestOptions:
              RequestOptions(path: '/citizens/new-citizen-details'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.createCitizenDetails(citizenDetails),
        throwsA(isA<DioException>()),
      );
    });
  });
}
