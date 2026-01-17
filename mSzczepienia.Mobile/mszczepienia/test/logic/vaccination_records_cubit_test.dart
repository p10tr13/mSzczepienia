import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_state.dart';

class MockVaccinationRecordRepository extends Mock
    implements VaccinationRecordRepository {}

class FakeVaccinationRecordOverviewQuery extends Fake
    implements VaccinationRecordOverviewQuery {}

void main() {
  late MockVaccinationRecordRepository mockRepository;

  final testRecord1 = VaccinationRecordOverviewModel(
    id: 1,
    vaccineName: 'Pfizer',
    shortDesciriptionEn: 'COVID-19 vaccine',
    shortDescriptionPl: 'Szczepionka COVID-19',
    vaccinationDate: DateTime(2024, 1, 15),
    dosesNumber: 2,
  );

  final testRecord2 = VaccinationRecordOverviewModel(
    id: 2,
    vaccineName: 'Moderna',
    shortDesciriptionEn: 'COVID-19 vaccine',
    shortDescriptionPl: 'Szczepionka COVID-19',
    vaccinationDate: DateTime(2024, 2, 20),
    dosesNumber: 1,
  );

  final testRecords = [testRecord1, testRecord2];

  setUpAll(() {
    registerFallbackValue(FakeVaccinationRecordOverviewQuery());
  });

  setUp(() {
    mockRepository = MockVaccinationRecordRepository();
  });

  group('VaccinationRecordsCubit', () {
    test('initial state is VaccinationRecordsInitial', () {
      final cubit = VaccinationRecordsCubit(mockRepository);
      expect(cubit.state, isA<VaccinationRecordsInitial>());
      cubit.close();
    });

    group('loadHistory', () {
      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'emits [VaccinationRecordsLoading, VaccinationRecordsLoaded] when loadHistory succeeds',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenAnswer((_) async => testRecords);
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) => cubit.loadHistory(1, 10, 'vaccinationDate', 'desc'),
        expect: () => [
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsLoaded>().having(
            (s) => s.records.length,
            'records.length',
            2,
          ),
        ],
        verify: (_) {
          verify(() => mockRepository.getVaccinationHistory(any())).called(1);
        },
      );

      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'emits [VaccinationRecordsLoading, VaccinationRecordsLoaded] with empty list when no records found',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenAnswer((_) async => []);
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) => cubit.loadHistory(1, 10, 'vaccinationDate', 'desc'),
        expect: () => [
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsLoaded>().having(
            (s) => s.records.isEmpty,
            'records.isEmpty',
            true,
          ),
        ],
      );

      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'emits [VaccinationRecordsLoading, VaccinationRecordsError] when loadHistory throws exception',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenThrow(Exception('Network error'));
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) => cubit.loadHistory(1, 10, 'vaccinationDate', 'desc'),
        expect: () => [
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsError>().having(
            (s) => s.message,
            'message',
            contains('Network error'),
          ),
        ],
      );

      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'passes correct parameters to repository',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenAnswer((_) async => testRecords);
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) => cubit.loadHistory(2, 20, 'vaccineName', 'asc'),
        verify: (_) {
          final captured =
              verify(
                    () => mockRepository.getVaccinationHistory(captureAny()),
                  ).captured.first
                  as VaccinationRecordOverviewQuery;
          expect(captured.pageNumber, 2);
          expect(captured.pageSize, 20);
          expect(captured.sortingField, 'vaccineName');
          expect(captured.sortingDirection, 'asc');
        },
      );

      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'can call loadHistory multiple times',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenAnswer((_) async => testRecords);
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) async {
          await cubit.loadHistory(1, 10, 'vaccinationDate', 'desc');
          await cubit.loadHistory(2, 10, 'vaccinationDate', 'desc');
        },
        expect: () => [
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsLoaded>(),
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsLoaded>(),
        ],
        verify: (_) {
          verify(() => mockRepository.getVaccinationHistory(any())).called(2);
        },
      );

      blocTest<VaccinationRecordsCubit, VaccinationRecordsState>(
        'returns records with correct vaccine names',
        setUp: () {
          when(
            () => mockRepository.getVaccinationHistory(any()),
          ).thenAnswer((_) async => testRecords);
        },
        build: () => VaccinationRecordsCubit(mockRepository),
        act: (cubit) => cubit.loadHistory(1, 10, 'vaccinationDate', 'desc'),
        expect: () => [
          isA<VaccinationRecordsLoading>(),
          isA<VaccinationRecordsLoaded>().having(
            (s) => s.records.map((r) => r.vaccineName).toList(),
            'vaccine names',
            ['Pfizer', 'Moderna'],
          ),
        ],
      );
    });
  });
}
