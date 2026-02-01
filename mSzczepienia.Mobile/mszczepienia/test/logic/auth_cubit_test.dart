import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/data/repositories/auth_repository.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';

// Mock AuthRepository
class MockAuthRepository extends Mock implements AuthRepository {}

void main() {
  late MockAuthRepository mockAuthRepository;

  final testUser = CitizenDetailsModel(
    name: 'John',
    surname: 'Doe',
    email: 'john@example.com',
    dateOfBirth: DateTime(1990, 1, 15),
    creationDate: DateTime(2024, 1, 1),
    pesel: '12345678901',
    facilityName: 'Test Hospital',
    submissionStatus: 'NotUploaded',
  );

  setUp(() {
    mockAuthRepository = MockAuthRepository();
  });

  group('AuthCubit - checkAppStart', () {
    // Note: blocTest starts recording after build(), so AuthLoading is often missed
    // because it's emitted synchronously in the constructor before recording starts

    blocTest<AuthCubit, AuthState>(
      'emits Authenticated when user is logged in',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => testUser);
      },
      build: () => AuthCubit(mockAuthRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Authenticated>().having((s) => s.user.name, 'user.name', 'John'),
      ],
      verify: (_) {
        verify(() => mockAuthRepository.getCurrentUser()).called(1);
      },
    );

    blocTest<AuthCubit, AuthState>(
      'emits Unauthenticated when no user is logged in',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
      },
      build: () => AuthCubit(mockAuthRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<Unauthenticated>()],
    );

    blocTest<AuthCubit, AuthState>(
      'emits Unauthenticated on error',
      setUp: () {
        // Use thenAnswer to throw asynchronously, matching real behavior
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => throw Exception('Storage error'));
      },
      build: () => AuthCubit(mockAuthRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<Unauthenticated>()],
    );
  });

  group('AuthCubit - loginRequested', () {
    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, Authenticated] on successful login',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.login('john@example.com', 'password123'),
        ).thenAnswer((_) async => testUser);
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.loginRequested(
          'john@example.com',
          'password123',
          'Incorrect credentials',
          'User not registered',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<Authenticated>().having(
          (s) => s.user.email,
          'user.email',
          'john@example.com',
        ),
      ],
      verify: (_) {
        verify(
          () => mockAuthRepository.login('john@example.com', 'password123'),
        ).called(1);
      },
    );

    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, AuthError] when login returns null',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.login('john@example.com', 'wrongpassword'),
        ).thenAnswer((_) async => null);
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.loginRequested(
          'john@example.com',
          'wrongpassword',
          'Incorrect credentials',
          'User not registered',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<AuthError>().having(
          (s) => s.message,
          'message',
          'Incorrect credentials',
        ),
      ],
    );

    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, AuthError] with incorrect credentials on login exception',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.login('john@example.com', 'password123'),
        ).thenThrow(Exception('INVALID_CREDENTIALS'));
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.loginRequested(
          'john@example.com',
          'password123',
          'Incorrect credentials',
          'User not registered',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<AuthError>().having(
          (s) => s.message,
          'message',
          'Incorrect credentials',
        ),
      ],
    );

    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, AuthError] with user not registered on USER_NOT_FOUND exception',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.login('unknown@example.com', 'password123'),
        ).thenThrow(Exception('USER_NOT_FOUND'));
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.loginRequested(
          'unknown@example.com',
          'password123',
          'Incorrect credentials',
          'User not registered',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<AuthError>().having(
          (s) => s.message,
          'message',
          'User not registered',
        ),
      ],
    );
  });

  group('AuthCubit - signUpRequested', () {
    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, Authenticated] on successful sign up',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.signUp(
            any(),
            any(),
            any(),
            any(),
            any(),
            any(),
            any(),
          ),
        ).thenAnswer((_) async => testUser);
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.signUpRequested(
          '12345678901',
          'John',
          'Doe',
          '1990-01-15',
          'Test Hospital',
          'john@example.com',
          'password123',
          'error',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<Authenticated>(),
      ],
      verify: (_) {
        verify(
          () => mockAuthRepository.signUp(
            '12345678901',
            'John',
            'Doe',
            '1990-01-15',
            'Test Hospital',
            'john@example.com',
            'password123',
          ),
        ).called(1);
      },
    );

    blocTest<AuthCubit, AuthState>(
      'emits [Unauthenticated, AuthLoading, AuthError] when sign up returns null',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => null);
        when(
          () => mockAuthRepository.signUp(
            any(),
            any(),
            any(),
            any(),
            any(),
            any(),
            any(),
          ),
        ).thenAnswer((_) async => null);
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.signUpRequested(
          '12345678901',
          'John',
          'Doe',
          '1990-01-15',
          'Test Hospital',
          'john@example.com',
          'password123',
          'Sign up error',
        );
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Unauthenticated>(),
        isA<AuthLoading>(),
        isA<AuthError>().having((s) => s.message, 'message', 'Sign up error'),
      ],
    );
  });

  group('AuthCubit - logoutRequested', () {
    blocTest<AuthCubit, AuthState>(
      'emits [Authenticated, AuthLoading, Unauthenticated] on successful logout',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => testUser);
        when(() => mockAuthRepository.logout()).thenAnswer((_) async {});
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.logoutRequested();
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Authenticated>(),
        isA<AuthLoading>(),
        isA<Unauthenticated>(),
      ],
      verify: (_) {
        verify(() => mockAuthRepository.logout()).called(1);
      },
    );

    blocTest<AuthCubit, AuthState>(
      'emits [Authenticated, AuthLoading, AuthError] on logout exception',
      setUp: () {
        when(
          () => mockAuthRepository.getCurrentUser(),
        ).thenAnswer((_) async => testUser);
        when(
          () => mockAuthRepository.logout(),
        ).thenThrow(Exception('Logout failed'));
      },
      build: () => AuthCubit(mockAuthRepository),
      act: (cubit) async {
        await Future.delayed(const Duration(milliseconds: 50));
        await cubit.logoutRequested();
      },
      wait: const Duration(milliseconds: 100),
      expect: () => [
        isA<Authenticated>(),
        isA<AuthLoading>(),
        isA<AuthError>().having(
          (s) => s.message,
          'message',
          contains('Logout failed'),
        ),
      ],
    );
  });
}
