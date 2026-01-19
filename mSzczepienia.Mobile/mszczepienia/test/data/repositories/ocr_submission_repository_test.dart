import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/datasources/ocr_submission_remote_datasource.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/data/repositories/ocr_submission_repository.dart';

// Mock Datasource
class MockOcrSubmissionRemoteDatasource extends Mock
    implements OcrSubmissionRemoteDatasource {}

// Fake classes for registerFallbackValue
class FakeOcrSubmissionModel extends Fake implements OcrSubmissionModel {}

class FakeVaccinationEntryModel extends Fake implements VaccinationEntryModel {}

void main() {
  late OcrSubmissionRepository repository;
  late MockOcrSubmissionRemoteDatasource mockDatasource;

  setUpAll(() {
    registerFallbackValue(FakeOcrSubmissionModel());
    registerFallbackValue(<VaccinationEntryModel>[]);
  });

  setUp(() {
    mockDatasource = MockOcrSubmissionRemoteDatasource();
    repository = OcrSubmissionRepository(mockDatasource);
  });

  group('OcrSubmissionRepository - cancelOcrSubmission', () {
    test('successfully cancels OCR submission', () async {
      // Arrange
      when(() => mockDatasource.cancelOcrSubmission()).thenAnswer((_) async {});

      // Act
      await repository.cancelOcrSubmission();

      // Assert
      verify(() => mockDatasource.cancelOcrSubmission()).called(1);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.cancelOcrSubmission()).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: 'Server Error',
            requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
          ),
          requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.cancelOcrSubmission(),
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
        when(() => mockDatasource.cancelOcrSubmission()).thenThrow(
          DioException(
            type: DioExceptionType.connectionTimeout,
            requestOptions: RequestOptions(path: '/ocrsubmission/delete'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.cancelOcrSubmission(),
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
        () => mockDatasource.cancelOcrSubmission(),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.cancelOcrSubmission(),
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

  group('OcrSubmissionRepository - submitOcrSubmission', () {
    final testEntries = [
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

    test('successfully submits OCR entries', () async {
      // Arrange
      when(
        () => mockDatasource.submitOcrSubmission(testEntries),
      ).thenAnswer((_) async {});

      // Act
      await repository.submitOcrSubmission(testEntries);

      // Assert
      verify(() => mockDatasource.submitOcrSubmission(testEntries)).called(1);
    });

    test('handles empty entries list', () async {
      // Arrange
      final List<VaccinationEntryModel> emptyEntries = [];
      when(
        () => mockDatasource.submitOcrSubmission(emptyEntries),
      ).thenAnswer((_) async {});

      // Act
      await repository.submitOcrSubmission(emptyEntries);

      // Assert
      verify(() => mockDatasource.submitOcrSubmission(emptyEntries)).called(1);
    });

    test('throws exception with error message on DioException', () async {
      // Arrange
      when(() => mockDatasource.submitOcrSubmission(any())).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 400,
            data: 'Invalid submission data',
            requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
          ),
          requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.submitOcrSubmission(testEntries),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Invalid submission data'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        when(() => mockDatasource.submitOcrSubmission(any())).thenThrow(
          DioException(
            type: DioExceptionType.sendTimeout,
            requestOptions: RequestOptions(path: '/ocrsubmission/submit'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.submitOcrSubmission(testEntries),
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
        () => mockDatasource.submitOcrSubmission(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.submitOcrSubmission(testEntries),
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

  group('OcrSubmissionRepository - processVaccinationBooklet', () {
    test('throws exception with error message on DioException', () async {
      // Arrange
      final query = FakeOcrSubmissionModel();
      when(() => mockDatasource.processVaccinationBooklet(any())).thenThrow(
        DioException(
          type: DioExceptionType.badResponse,
          response: Response(
            statusCode: 500,
            data: 'Processing failed',
            requestOptions: RequestOptions(path: '/ocrsubmission/process'),
          ),
          requestOptions: RequestOptions(path: '/ocrsubmission/process'),
        ),
      );

      // Act & Assert
      expect(
        () => repository.processVaccinationBooklet(query),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Processing failed'),
          ),
        ),
      );
    });

    test(
      'throws exception with Connection Error when no response data',
      () async {
        // Arrange
        final query = FakeOcrSubmissionModel();
        when(() => mockDatasource.processVaccinationBooklet(any())).thenThrow(
          DioException(
            type: DioExceptionType.receiveTimeout,
            requestOptions: RequestOptions(path: '/ocrsubmission/process'),
          ),
        );

        // Act & Assert
        expect(
          () => repository.processVaccinationBooklet(query),
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
      final query = FakeOcrSubmissionModel();
      when(
        () => mockDatasource.processVaccinationBooklet(any()),
      ).thenThrow(Exception('Some error'));

      // Act & Assert
      expect(
        () => repository.processVaccinationBooklet(query),
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
