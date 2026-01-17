import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_state.dart';

class MockVaccinationRecordRepository extends Mock
    implements VaccinationRecordRepository {}

class FakeVaccinationRecordDetailsQuery extends Fake
    implements VaccinationRecordDetailsQuery {}

void main() {
  late MockVaccinationRecordRepository mockRepository;

  final testDetails1 = VaccinationRecordDetailsModel(
    vaccineId: 1,
    vaccineName: 'Pfizer',
    manufacturer: 'Pfizer-BioNTech',
    vaccineDescriptionPl: 'Szczepionka mRNA',
    vaccineDescriptionEn: 'mRNA vaccine',
    shortDescriptionPl: 'COVID-19',
    shortDescriptionEn: 'COVID-19',
    creationDate: DateTime(2020, 12, 1),
    diseasesPl: ['COVID-19'],
    diseasesEn: ['COVID-19'],
    vaccinationDate: DateTime(2024, 1, 15),
    nextVaccinationDate: DateTime(2024, 7, 15),
    doseNumber: '1',
    batch: 'BATCH001',
    remarks: 'First dose',
  );

  final testDetails2 = VaccinationRecordDetailsModel(
    vaccineId: 1,
    vaccineName: 'Pfizer',
    manufacturer: 'Pfizer-BioNTech',
    vaccineDescriptionPl: 'Szczepionka mRNA',
    vaccineDescriptionEn: 'mRNA vaccine',
    shortDescriptionPl: 'COVID-19',
    shortDescriptionEn: 'COVID-19',
    creationDate: DateTime(2020, 12, 1),
    diseasesPl: ['COVID-19'],
    diseasesEn: ['COVID-19'],
    vaccinationDate: DateTime(2024, 7, 15),
    nextVaccinationDate: null,
    doseNumber: '2',
    batch: 'BATCH002',
    remarks: null,
  );

  final testDetailsList = [testDetails1, testDetails2];

  setUpAll(() {
    registerFallbackValue(FakeVaccinationRecordDetailsQuery());
  });

  setUp(() {
    mockRepository = MockVaccinationRecordRepository();
  });

  group('VaccinationRecordDetailsCubit', () {
    test('initial state is VaccinationRecordDetailsInitial', () {
      final cubit = VaccinationRecordDetailsCubit(mockRepository);
      expect(cubit.state, isA<VaccinationRecordDetailsInitial>());
      cubit.close();
    });

    group('loadVaccinationDetails', () {
      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'emits [VaccinationRecordDetailsLoading, VaccinationRecordDetailsLoaded] when loading succeeds',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => testDetailsList);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(1),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>().having(
            (s) => s.details.length,
            'details.length',
            2,
          ),
        ],
        verify: (_) {
          verify(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).called(1);
        },
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'emits [VaccinationRecordDetailsLoading, VaccinationRecordDetailsLoaded] with single record',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => [testDetails1]);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(1),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>().having(
            (s) => s.details.first.doseNumber,
            'doseNumber',
            '1',
          ),
        ],
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'emits [VaccinationRecordDetailsLoading, VaccinationRecordDetailsLoaded] with empty list when no details found',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => []);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(999),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>().having(
            (s) => s.details.isEmpty,
            'details.isEmpty',
            true,
          ),
        ],
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'emits [VaccinationRecordDetailsLoading, VaccinationRecordDetailsError] when loading fails',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenThrow(Exception('Network error'));
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(1),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsError>().having(
            (s) => s.message,
            'message',
            contains('Network error'),
          ),
        ],
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'passes correct vaccineId to repository',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => testDetailsList);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(42),
        verify: (_) {
          final captured =
              verify(
                    () => mockRepository.getVaccinationRecordsByVaccineId(
                      captureAny(),
                    ),
                  ).captured.first
                  as VaccinationRecordDetailsQuery;
          expect(captured.vaccineId, 42);
        },
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'can call loadVaccinationDetails multiple times',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => testDetailsList);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) async {
          await cubit.loadVaccinationDetails(1);
          await cubit.loadVaccinationDetails(2);
        },
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>(),
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>(),
        ],
        verify: (_) {
          verify(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).called(2);
        },
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'returns details with correct vaccine information',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenAnswer((_) async => testDetailsList);
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(1),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsLoaded>().having(
            (s) => s.details.first.manufacturer,
            'manufacturer',
            'Pfizer-BioNTech',
          ),
        ],
      );

      blocTest<VaccinationRecordDetailsCubit, VaccinationRecordDetailsState>(
        'handles generic error',
        setUp: () {
          when(
            () => mockRepository.getVaccinationRecordsByVaccineId(any()),
          ).thenThrow(Exception('Unknown error'));
        },
        build: () => VaccinationRecordDetailsCubit(mockRepository),
        act: (cubit) => cubit.loadVaccinationDetails(1),
        expect: () => [
          isA<VaccinationRecordDetailsLoading>(),
          isA<VaccinationRecordDetailsError>().having(
            (s) => s.message,
            'message',
            contains('Unknown error'),
          ),
        ],
      );
    });
  });
}
