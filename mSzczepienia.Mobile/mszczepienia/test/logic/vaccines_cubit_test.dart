import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/vaccine_name_model.dart';
import 'package:mszczepienia/data/repositories/vaccine_repository.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_cubit.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_state.dart';

class MockVaccineRepository extends Mock implements VaccineRepository {}

void main() {
  late MockVaccineRepository mockRepository;

  final testVaccine1 = VaccineNameModel(vaccineId: 1, vaccineName: 'Pfizer');
  final testVaccine2 = VaccineNameModel(vaccineId: 2, vaccineName: 'Moderna');
  final testVaccine3 = VaccineNameModel(
    vaccineId: 3,
    vaccineName: 'AstraZeneca',
  );

  final testVaccines = [testVaccine1, testVaccine2, testVaccine3];

  setUp(() {
    mockRepository = MockVaccineRepository();
  });

  group('VaccinesCubit', () {
    test('initial state is VaccinesInitial', () {
      final cubit = VaccinesCubit(mockRepository);
      expect(cubit.state, isA<VaccinesInitial>());
      cubit.close();
    });

    group('loadVaccines', () {
      blocTest<VaccinesCubit, VaccinesState>(
        'emits [VaccinesLoading, VaccinesLoaded] when loadVaccines succeeds',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames(any()),
          ).thenAnswer((_) async => testVaccines);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines(''),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.length,
            'vaccines.length',
            3,
          ),
        ],
        verify: (_) {
          verify(() => mockRepository.getVaccineNames('')).called(1);
        },
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'emits [VaccinesLoading, VaccinesLoaded] with filtered results',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames('Pfi'),
          ).thenAnswer((_) async => [testVaccine1]);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines('Pfi'),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.first.vaccineName,
            'vaccineName',
            'Pfizer',
          ),
        ],
        verify: (_) {
          verify(() => mockRepository.getVaccineNames('Pfi')).called(1);
        },
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'emits [VaccinesLoading, VaccinesLoaded] with empty list when no vaccines match query',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames('XYZ'),
          ).thenAnswer((_) async => []);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines('XYZ'),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.isEmpty,
            'vaccines.isEmpty',
            true,
          ),
        ],
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'emits [VaccinesLoading, VaccinesError] when loadVaccines throws exception',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames(any()),
          ).thenThrow(Exception('Network error'));
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines(''),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesError>().having(
            (s) => s.message,
            'message',
            contains('Network error'),
          ),
        ],
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'passes correct query to repository',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames(any()),
          ).thenAnswer((_) async => testVaccines);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines('covid'),
        verify: (_) {
          verify(() => mockRepository.getVaccineNames('covid')).called(1);
        },
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'can call loadVaccines multiple times with different queries',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames('Pfi'),
          ).thenAnswer((_) async => [testVaccine1]);
          when(
            () => mockRepository.getVaccineNames('Mod'),
          ).thenAnswer((_) async => [testVaccine2]);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) async {
          await cubit.loadVaccines('Pfi');
          await cubit.loadVaccines('Mod');
        },
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.first.vaccineName,
            'vaccineName',
            'Pfizer',
          ),
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.first.vaccineName,
            'vaccineName',
            'Moderna',
          ),
        ],
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'returns vaccines with correct ids',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames(any()),
          ).thenAnswer((_) async => testVaccines);
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines(''),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesLoaded>().having(
            (s) => s.vaccines.map((v) => v.vaccineId).toList(),
            'vaccine ids',
            [1, 2, 3],
          ),
        ],
      );

      blocTest<VaccinesCubit, VaccinesState>(
        'handles generic error',
        setUp: () {
          when(
            () => mockRepository.getVaccineNames(any()),
          ).thenThrow(Exception('Unknown error'));
        },
        build: () => VaccinesCubit(mockRepository),
        act: (cubit) => cubit.loadVaccines('test'),
        expect: () => [
          isA<VaccinesLoading>(),
          isA<VaccinesError>().having(
            (s) => s.message,
            'message',
            contains('Unknown error'),
          ),
        ],
      );
    });
  });
}
