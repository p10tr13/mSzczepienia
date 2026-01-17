import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/medical_facility_model.dart';
import 'package:mszczepienia/data/repositories/medical_facility_repository.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_state.dart';

// Mock MedicalFacilityRepository
class MockMedicalFacilityRepository extends Mock
    implements MedicalFacilityRepository {}

void main() {
  late MockMedicalFacilityRepository mockRepository;

  final testFacilities = [
    MedicalFacilityModel(facilityId: 1, facilityName: 'Hospital A'),
    MedicalFacilityModel(facilityId: 2, facilityName: 'Clinic B'),
    MedicalFacilityModel(facilityId: 3, facilityName: 'Medical Center C'),
  ];

  final query = 'szpital';

  setUp(() {
    mockRepository = MockMedicalFacilityRepository();
  });

  group('FacilitiesCubit', () {
    test('initial state is FacilitiesInitial', () {
      when(
        () => mockRepository.getMedicalFacilities(query),
      ).thenAnswer((_) async => []);

      final cubit = FacilitiesCubit(mockRepository);

      expect(cubit.state, isA<FacilitiesInitial>());
    });

    blocTest<FacilitiesCubit, FacilitiesState>(
      'emits [FacilitiesLoading, FacilitiesLoaded] when loadFacilities succeeds',
      setUp: () {
        when(
          () => mockRepository.getMedicalFacilities(query),
        ).thenAnswer((_) async => testFacilities);
      },
      build: () => FacilitiesCubit(mockRepository),
      act: (cubit) => cubit.loadFacilities(query),
      expect: () => [
        isA<FacilitiesLoading>(),
        isA<FacilitiesLoaded>().having(
          (s) => s.facilities.length,
          'facilities.length',
          3,
        ),
      ],
      verify: (_) {
        verify(() => mockRepository.getMedicalFacilities(query)).called(1);
      },
    );

    blocTest<FacilitiesCubit, FacilitiesState>(
      'emits [FacilitiesLoading, FacilitiesLoaded] with empty list when no facilities',
      setUp: () {
        when(
          () => mockRepository.getMedicalFacilities(query),
        ).thenAnswer((_) async => []);
      },
      build: () => FacilitiesCubit(mockRepository),
      act: (cubit) => cubit.loadFacilities(query),
      expect: () => [
        isA<FacilitiesLoading>(),
        isA<FacilitiesLoaded>().having(
          (s) => s.facilities.isEmpty,
          'facilities.isEmpty',
          true,
        ),
      ],
    );

    blocTest<FacilitiesCubit, FacilitiesState>(
      'emits [FacilitiesLoading, FacilitiesError] when loadFacilities fails',
      setUp: () {
        when(
          () => mockRepository.getMedicalFacilities(query),
        ).thenThrow(Exception('Network error'));
      },
      build: () => FacilitiesCubit(mockRepository),
      act: (cubit) => cubit.loadFacilities(query),
      expect: () => [
        isA<FacilitiesLoading>(),
        isA<FacilitiesError>().having(
          (s) => s.message,
          'message',
          contains('Network error'),
        ),
      ],
    );

    blocTest<FacilitiesCubit, FacilitiesState>(
      'FacilitiesLoaded contains correct facility names',
      setUp: () {
        when(
          () => mockRepository.getMedicalFacilities(query),
        ).thenAnswer((_) async => testFacilities);
      },
      build: () => FacilitiesCubit(mockRepository),
      act: (cubit) => cubit.loadFacilities(query),
      expect: () => [
        isA<FacilitiesLoading>(),
        isA<FacilitiesLoaded>().having(
          (s) => s.facilities.map((f) => f.facilityName).toList(),
          'facility names',
          ['Hospital A', 'Clinic B', 'Medical Center C'],
        ),
      ],
    );

    blocTest<FacilitiesCubit, FacilitiesState>(
      'can reload facilities after error',
      setUp: () {
        var callCount = 0;
        when(() => mockRepository.getMedicalFacilities(query)).thenAnswer((
          _,
        ) async {
          callCount++;
          if (callCount == 1) {
            throw Exception('First call failed');
          }
          return testFacilities;
        });
      },
      build: () => FacilitiesCubit(mockRepository),
      act: (cubit) async {
        await cubit.loadFacilities(query); // First call - fails
        await cubit.loadFacilities(query); // Second call - succeeds
      },
      expect: () => [
        isA<FacilitiesLoading>(),
        isA<FacilitiesError>(),
        isA<FacilitiesLoading>(),
        isA<FacilitiesLoaded>(),
      ],
    );
  });
}
