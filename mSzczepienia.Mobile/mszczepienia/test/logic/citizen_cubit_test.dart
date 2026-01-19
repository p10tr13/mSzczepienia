import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/citizen_submission_status.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/citizen/citizen_state.dart';

class MockCitizenRepository extends Mock implements CitizenRepository {}

void main() {
  late MockCitizenRepository mockRepository;

  final testStatus = CitizenSubmissionStatus(submissionStatus: 'Approved');
  final pendingStatus = CitizenSubmissionStatus(submissionStatus: 'Pending');
  final rejectedStatus = CitizenSubmissionStatus(submissionStatus: 'Rejected');

  setUp(() {
    mockRepository = MockCitizenRepository();
  });

  group('CitizenCubit', () {
    test('initial state is CitizenInitial', () {
      final cubit = CitizenCubit(mockRepository);
      expect(cubit.state, isA<CitizenInitial>());
      cubit.close();
    });

    group('getCitizenStatus', () {
      blocTest<CitizenCubit, CitizenState>(
        'emits [CitizenLoading, CitizenLoaded] when getCitizenSubmissionStatus succeeds with Approved status',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenAnswer((_) async => testStatus);
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) => cubit.getCitizenStatus(),
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenLoaded>().having(
            (s) => s.status.submissionStatus,
            'submissionStatus',
            'Approved',
          ),
        ],
        verify: (_) {
          verify(() => mockRepository.getCitizenSubmissionStatus()).called(1);
        },
      );

      blocTest<CitizenCubit, CitizenState>(
        'emits [CitizenLoading, CitizenLoaded] when getCitizenSubmissionStatus succeeds with Pending status',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenAnswer((_) async => pendingStatus);
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) => cubit.getCitizenStatus(),
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenLoaded>().having(
            (s) => s.status.submissionStatus,
            'submissionStatus',
            'Pending',
          ),
        ],
      );

      blocTest<CitizenCubit, CitizenState>(
        'emits [CitizenLoading, CitizenLoaded] when getCitizenSubmissionStatus succeeds with Rejected status',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenAnswer((_) async => rejectedStatus);
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) => cubit.getCitizenStatus(),
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenLoaded>().having(
            (s) => s.status.submissionStatus,
            'submissionStatus',
            'Rejected',
          ),
        ],
      );

      blocTest<CitizenCubit, CitizenState>(
        'emits [CitizenLoading, CitizenError] when getCitizenSubmissionStatus throws exception',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenThrow(Exception('Network error'));
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) => cubit.getCitizenStatus(),
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenError>().having(
            (s) => s.message,
            'message',
            contains('Network error'),
          ),
        ],
      );

      blocTest<CitizenCubit, CitizenState>(
        'emits [CitizenLoading, CitizenError] when repository throws generic error',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenThrow(Exception('Unknown error'));
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) => cubit.getCitizenStatus(),
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenError>().having(
            (s) => s.message,
            'message',
            contains('Unknown error'),
          ),
        ],
      );

      blocTest<CitizenCubit, CitizenState>(
        'can call getCitizenStatus multiple times',
        setUp: () {
          when(
            () => mockRepository.getCitizenSubmissionStatus(),
          ).thenAnswer((_) async => testStatus);
        },
        build: () => CitizenCubit(mockRepository),
        act: (cubit) async {
          await cubit.getCitizenStatus();
          await cubit.getCitizenStatus();
        },
        expect: () => [
          isA<CitizenLoading>(),
          isA<CitizenLoaded>(),
          isA<CitizenLoading>(),
          isA<CitizenLoaded>(),
        ],
        verify: (_) {
          verify(() => mockRepository.getCitizenSubmissionStatus()).called(2);
        },
      );
    });
  });
}
