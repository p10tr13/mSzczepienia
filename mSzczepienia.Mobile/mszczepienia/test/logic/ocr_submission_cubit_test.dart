import 'dart:io';

import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/data/repositories/ocr_submission_repository.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';

class MockOcrSubmissionRepository extends Mock
    implements OcrSubmissionRepository {}

class MockFile extends Mock implements File {}

class FakeOcrSubmissionModel extends Fake implements OcrSubmissionModel {}

void main() {
  late MockOcrSubmissionRepository mockRepository;
  late List<File> mockImages;

  final testEntry1 = VaccinationEntryModel(
    id: 1,
    vaccineNameOcr: 'Pfizer OCR',
    dateOcr: '2024-01-15',
    batchOcr: 'BATCH001',
    vaccineName: 'Pfizer',
    date: DateTime(2024, 1, 15),
    batch: 'BATCH001',
    status: 'Valid',
  );

  final testEntry2 = VaccinationEntryModel(
    id: 2,
    vaccineNameOcr: 'Moderna OCR',
    dateOcr: '2024-02-20',
    batchOcr: 'BATCH002',
    vaccineName: 'Moderna',
    date: DateTime(2024, 2, 20),
    batch: 'BATCH002',
    status: 'Valid',
  );

  final testEntries = [testEntry1, testEntry2];

  setUpAll(() {
    registerFallbackValue(FakeOcrSubmissionModel());
    registerFallbackValue(<VaccinationEntryModel>[]);
  });

  setUp(() {
    mockRepository = MockOcrSubmissionRepository();
    mockImages = [MockFile(), MockFile()];
  });

  group('OcrSubmissionCubit', () {
    test('initial state is OcrSubmissionInitial', () {
      final cubit = OcrSubmissionCubit(mockRepository);
      expect(cubit.state, isA<OcrSubmissionInitial>());
      cubit.close();
    });

    group('processVaccinationBooklet', () {
      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionProcessingLoading, OcrSubmissionLoaded] when processing succeeds',
        setUp: () {
          when(
            () => mockRepository.processVaccinationBooklet(any()),
          ).thenAnswer((_) async => testEntries);
        },
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.processVaccinationBooklet(mockImages),
        expect: () => [
          isA<OcrSubmissionProcessingLoading>(),
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries.length,
            'entries.length',
            2,
          ),
        ],
        verify: (_) {
          verify(
            () => mockRepository.processVaccinationBooklet(any()),
          ).called(1);
        },
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionProcessingLoading, OcrSubmissionLoaded] with empty list when no entries found',
        setUp: () {
          when(
            () => mockRepository.processVaccinationBooklet(any()),
          ).thenAnswer((_) async => []);
        },
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.processVaccinationBooklet(mockImages),
        expect: () => [
          isA<OcrSubmissionProcessingLoading>(),
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries.isEmpty,
            'entries.isEmpty',
            true,
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionProcessingLoading, OcrSubmissionError] when processing fails',
        setUp: () {
          when(
            () => mockRepository.processVaccinationBooklet(any()),
          ).thenThrow(Exception('OCR processing failed'));
        },
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.processVaccinationBooklet(mockImages),
        expect: () => [
          isA<OcrSubmissionProcessingLoading>(),
          isA<OcrSubmissionError>().having(
            (s) => s.message,
            'message',
            contains('OCR processing failed'),
          ),
        ],
      );
    });

    group('cancelOcrSubmission', () {
      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionCancellingLoading, OcrSubmissionInitial] when cancellation succeeds',
        setUp: () {
          when(
            () => mockRepository.cancelOcrSubmission(),
          ).thenAnswer((_) async {});
        },
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.cancelOcrSubmission(),
        expect: () => [
          isA<OcrSubmissionCancellingLoading>(),
          isA<OcrSubmissionInitial>(),
        ],
        verify: (_) {
          verify(() => mockRepository.cancelOcrSubmission()).called(1);
        },
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionCancellingLoading, OcrSubmissionError] when cancellation fails',
        setUp: () {
          when(
            () => mockRepository.cancelOcrSubmission(),
          ).thenThrow(Exception('Cancellation failed'));
        },
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.cancelOcrSubmission(),
        expect: () => [
          isA<OcrSubmissionCancellingLoading>(),
          isA<OcrSubmissionError>().having(
            (s) => s.message,
            'message',
            contains('Cancellation failed'),
          ),
        ],
      );
    });

    group('submitOcrSubmission', () {
      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionSubmittingLoading, OcrSubmissionInitial] when submission succeeds',
        setUp: () {
          when(
            () => mockRepository.processVaccinationBooklet(any()),
          ).thenAnswer((_) async => testEntries);
          when(
            () => mockRepository.submitOcrSubmission(any()),
          ).thenAnswer((_) async {});
        },
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.submitOcrSubmission(),
        expect: () => [
          isA<OcrSubmissionSubmittingLoading>(),
          isA<OcrSubmissionInitial>(),
        ],
        verify: (_) {
          verify(() => mockRepository.submitOcrSubmission(any())).called(1);
        },
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits [OcrSubmissionSubmittingLoading, OcrSubmissionError] when submission fails',
        setUp: () {
          when(
            () => mockRepository.submitOcrSubmission(any()),
          ).thenThrow(Exception('Submission failed'));
        },
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.submitOcrSubmission(),
        expect: () => [
          isA<OcrSubmissionSubmittingLoading>(),
          isA<OcrSubmissionError>().having(
            (s) => s.message,
            'message',
            contains('Submission failed'),
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'does nothing when state is not OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.submitOcrSubmission(),
        expect: () => [],
      );
    });

    group('addEntry', () {
      final newEntry = VaccinationEntryModel(
        id: 3,
        vaccineNameOcr: 'AstraZeneca OCR',
        dateOcr: '2024-03-10',
        batchOcr: 'BATCH003',
        vaccineName: 'AstraZeneca',
        date: DateTime(2024, 3, 10),
        batch: 'BATCH003',
        status: 'Valid',
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits OcrSubmissionLoaded with new entry added when state is OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.addEntry(newEntry),
        expect: () => [
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries.length,
            'entries.length',
            3,
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'does nothing when state is not OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.addEntry(newEntry),
        expect: () => [],
      );
    });

    group('updateEntry', () {
      final updatedEntry = VaccinationEntryModel(
        id: 1,
        vaccineNameOcr: 'Updated OCR',
        dateOcr: '2024-01-20',
        batchOcr: 'BATCH_UPDATED',
        vaccineName: 'Updated Vaccine',
        date: DateTime(2024, 1, 20),
        batch: 'BATCH_UPDATED',
        status: 'Updated',
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits OcrSubmissionLoaded with updated entry when state is OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.updateEntry(0, updatedEntry),
        expect: () => [
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries[0].vaccineName,
            'entries[0].vaccineName',
            'Updated Vaccine',
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'does nothing when state is not OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.updateEntry(0, updatedEntry),
        expect: () => [],
      );
    });

    group('removeEntry', () {
      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'emits OcrSubmissionLoaded with entry removed when state is OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.removeEntry(0),
        expect: () => [
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries.length,
            'entries.length',
            1,
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'removes correct entry by index',
        build: () => OcrSubmissionCubit(mockRepository),
        seed: () => OcrSubmissionLoaded(testEntries),
        act: (cubit) => cubit.removeEntry(1),
        expect: () => [
          isA<OcrSubmissionLoaded>().having(
            (s) => s.entries[0].vaccineName,
            'entries[0].vaccineName',
            'Pfizer',
          ),
        ],
      );

      blocTest<OcrSubmissionCubit, OcrSubmissionState>(
        'does nothing when state is not OcrSubmissionLoaded',
        build: () => OcrSubmissionCubit(mockRepository),
        act: (cubit) => cubit.removeEntry(0),
        expect: () => [],
      );
    });
  });
}
