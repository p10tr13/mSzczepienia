import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/ocr_submission_remote_datasource.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';

// Mock Dio
class MockDio extends Mock implements Dio {}

// Fake classes for registerFallbackValue
class FakeFormData extends Fake implements FormData {}

class FakeOptions extends Fake implements Options {}

void main() {
  late OcrSubmissionRemoteDatasource dataSource;
  late MockDio mockDio;

  setUpAll(() {
    registerFallbackValue(FakeFormData());
    registerFallbackValue(FakeOptions());
  });

  setUp(() {
    mockDio = MockDio();
    dataSource = OcrSubmissionRemoteDatasource(mockDio);
  });

  group('OcrSubmissionRemoteDatasource - cancelOcrSubmission', () {
    test('successfully cancels OCR submission', () async {
      // Arrange
      when(() => mockDio.delete('/ocrsubmission/delete')).thenAnswer(
        (_) async => Response(
          statusCode: 200,
          requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
        ),
      );

      // Act
      await dataSource.cancelOcrSubmission();

      // Assert
      verify(() => mockDio.delete('/ocrsubmission/delete')).called(1);
    });

    test('throws DioException on API error', () async {
      // Arrange
      when(() => mockDio.delete('/ocrsubmission/delete')).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: {'error': 'Internal server error'},
            requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
          ),
          requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.cancelOcrSubmission(),
        throwsA(isA<DioException>()),
      );
    });

    test('throws DioException on connection timeout', () async {
      // Arrange
      when(() => mockDio.delete('/ocrsubmission/delete')).thenThrow(
        DioException(
          type: DioExceptionType.connectionTimeout,
          requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.cancelOcrSubmission(),
        throwsA(
          isA<DioException>().having(
            (e) => e.type,
            'type',
            DioExceptionType.connectionTimeout,
          ),
        ),
      );
    });
  });

  group('OcrSubmissionRemoteDatasource - submitOcrSubmission', () {
    test('successfully submits OCR entries', () async {
      // Arrange
      final entries = [
        VaccinationEntryModel(
          id: 0,
          vaccineNameOcr: 'BCG',
          dateOcr: '2023-05-15',
          batchOcr: 'BATCH001',
          vaccineName: 'BCG',
          date: DateTime(2023, 5, 15),
          batch: 'BATCH001',
          status: 'Valid',
        ),
        VaccinationEntryModel(
          id: 1,
          vaccineNameOcr: 'MMR',
          dateOcr: '2023-06-20',
          batchOcr: 'BATCH002',
          vaccineName: 'MMR',
          date: DateTime(2023, 6, 20),
          batch: 'BATCH002',
          status: 'Valid',
        ),
      ];

      when(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).thenAnswer(
        (_) async => Response(
          statusCode: 200,
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        ),
      );

      // Act
      await dataSource.submitOcrSubmission(entries);

      // Assert
      verify(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).called(1);
    });

    test('sends correct JSON data in POST request', () async {
      // Arrange
      final entries = [
        VaccinationEntryModel(
          id: 0,
          vaccineNameOcr: 'BCG',
          dateOcr: '2023-05-15',
          batchOcr: 'BATCH001',
          vaccineName: 'BCG',
          date: DateTime(2023, 5, 15),
          batch: 'BATCH001',
          status: 'Valid',
        ),
      ];

      List<Map<String, dynamic>>? capturedData;

      when(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).thenAnswer((invocation) async {
        capturedData =
            invocation.namedArguments[const Symbol('data')]
                as List<Map<String, dynamic>>?;
        return Response(
          statusCode: 200,
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        );
      });

      // Act
      await dataSource.submitOcrSubmission(entries);

      // Assert
      expect(capturedData, isNotNull);
      expect(capturedData!.length, equals(1));
      expect(capturedData![0]['vaccineName'], equals('BCG'));
      expect(capturedData![0]['batch'], equals('BATCH001'));
    });

    test('handles empty entries list', () async {
      // Arrange
      final List<VaccinationEntryModel> entries = [];

      when(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).thenAnswer(
        (_) async => Response(
          statusCode: 200,
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        ),
      );

      // Act
      await dataSource.submitOcrSubmission(entries);

      // Assert
      verify(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: [],
          options: any(named: 'options'),
        ),
      ).called(1);
    });

    test('throws DioException on API error', () async {
      // Arrange
      final entries = [
        VaccinationEntryModel(
          id: 0,
          vaccineNameOcr: 'BCG',
          dateOcr: '2023-05-15',
          batchOcr: 'BATCH001',
          vaccineName: 'BCG',
          date: DateTime(2023, 5, 15),
          batch: 'BATCH001',
          status: 'Valid',
        ),
      ];

      when(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 400,
            data: {'error': 'Invalid submission'},
            requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
          ),
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.submitOcrSubmission(entries),
        throwsA(isA<DioException>()),
      );
    });

    test('throws DioException on send timeout', () async {
      // Arrange
      final entries = [
        VaccinationEntryModel(
          id: 0,
          vaccineNameOcr: 'BCG',
          dateOcr: '2023-05-15',
          batchOcr: 'BATCH001',
          vaccineName: 'BCG',
          date: DateTime(2023, 5, 15),
          batch: 'BATCH001',
          status: 'Valid',
        ),
      ];

      when(
        () => mockDio.post(
          '/ocrsubmission/submit',
          data: any(named: 'data'),
          options: any(named: 'options'),
        ),
      ).thenThrow(
        DioException(
          type: DioExceptionType.sendTimeout,
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        ),
      );

      // Act & Assert
      expect(
        () => dataSource.submitOcrSubmission(entries),
        throwsA(
          isA<DioException>().having(
            (e) => e.type,
            'type',
            DioExceptionType.sendTimeout,
          ),
        ),
      );
    });
  });

  // Note: processVaccinationBooklet is harder to test because it uses MultipartFile.fromFile
  // which requires actual files. In a real scenario, you would either:
  // 1. Create temporary test files
  // 2. Abstract the file reading logic
  // 3. Use integration tests for this method
}
