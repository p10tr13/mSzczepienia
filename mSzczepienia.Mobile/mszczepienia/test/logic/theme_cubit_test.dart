import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/data/repositories/theme_repository.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';

// Mock ThemeRepository
class MockThemeRepository extends Mock implements ThemeRepository {}

void main() {
  late MockThemeRepository mockRepository;

  setUp(() {
    mockRepository = MockThemeRepository();
  });

  group('ThemeCubit', () {
    test('initial state is LightTheme', () {
      when(
        () => mockRepository.getCurrentTheme(),
      ).thenAnswer((_) async => 'light');

      final cubit = ThemeCubit(mockRepository);

      expect(cubit.state, isA<LightTheme>());
    });

    blocTest<ThemeCubit, ThemeState>(
      'checkAppStart emits DarkTheme when stored theme is dark',
      setUp: () {
        when(
          () => mockRepository.getCurrentTheme(),
        ).thenAnswer((_) async => 'dark');
      },
      build: () => ThemeCubit(mockRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<DarkTheme>()],
    );

    blocTest<ThemeCubit, ThemeState>(
      'checkAppStart emits LightTheme when stored theme is light',
      setUp: () {
        when(
          () => mockRepository.getCurrentTheme(),
        ).thenAnswer((_) async => 'light');
      },
      build: () => ThemeCubit(mockRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<LightTheme>()],
    );

    test('checkAppStart emits LightTheme on error', () async {
      when(
        () => mockRepository.getCurrentTheme(),
      ).thenThrow(Exception('Storage error'));

      final cubit = ThemeCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));

      // State should remain LightTheme (default) after error
      expect(cubit.state, isA<LightTheme>());
    });

    test('switchToDark emits DarkTheme and saves to repository', () async {
      when(
        () => mockRepository.getCurrentTheme(),
      ).thenAnswer((_) async => 'light');
      when(
        () => mockRepository.setCurrentTheme('dark'),
      ).thenAnswer((_) async {});

      final cubit = ThemeCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<LightTheme>());

      cubit.switchToDark();

      expect(cubit.state, isA<DarkTheme>());
      verify(() => mockRepository.setCurrentTheme('dark')).called(1);
    });

    test('switchToLight emits LightTheme and saves to repository', () async {
      when(
        () => mockRepository.getCurrentTheme(),
      ).thenAnswer((_) async => 'dark');
      when(
        () => mockRepository.setCurrentTheme('light'),
      ).thenAnswer((_) async {});

      final cubit = ThemeCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<DarkTheme>());

      cubit.switchToLight();

      expect(cubit.state, isA<LightTheme>());
      verify(() => mockRepository.setCurrentTheme('light')).called(1);
    });

    test('can switch from Light to Dark and back', () async {
      when(
        () => mockRepository.getCurrentTheme(),
      ).thenAnswer((_) async => 'light');
      when(
        () => mockRepository.setCurrentTheme(any()),
      ).thenAnswer((_) async {});

      final cubit = ThemeCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<LightTheme>());

      cubit.switchToDark();
      expect(cubit.state, isA<DarkTheme>());

      cubit.switchToLight();
      expect(cubit.state, isA<LightTheme>());
    });
  });
}
