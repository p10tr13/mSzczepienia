import 'package:bloc_test/bloc_test.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:get_it/get_it.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/notifications/notification_service.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/data/repositories/language_repository.dart';
import 'package:mszczepienia/logic/language/language_state.dart';

// Mock LanguageRepository
class MockLanguageRepository extends Mock implements LanguageRepository {}

// Mock NotificationService
class MockNotificationService extends Mock implements NotificationService {}

void main() {
  late MockLanguageRepository mockRepository;
  late MockNotificationService mockNotificationService;

  setUp(() {
    mockRepository = MockLanguageRepository();
    mockNotificationService = MockNotificationService();

    // Register mock NotificationService in GetIt
    final sl = GetIt.instance;
    if (sl.isRegistered<NotificationService>()) {
      sl.unregister<NotificationService>();
    }
    sl.registerSingleton<NotificationService>(mockNotificationService);

    // Setup default stub for uploadFcmToken
    when(
      () => mockNotificationService.uploadFcmToken(any()),
    ).thenAnswer((_) async {});
  });

  tearDown(() {
    final sl = GetIt.instance;
    if (sl.isRegistered<NotificationService>()) {
      sl.unregister<NotificationService>();
    }
  });

  group('LanguageCubit', () {
    test('initial state is LanguageEnglish', () {
      when(
        () => mockRepository.getCurrentLanguage(),
      ).thenAnswer((_) async => 'en');

      final cubit = LanguageCubit(mockRepository);

      expect(cubit.state, isA<LanguageEnglish>());
    });

    blocTest<LanguageCubit, LanguageState>(
      'checkAppStart emits LanguagePolish when stored language is pl',
      setUp: () {
        when(
          () => mockRepository.getCurrentLanguage(),
        ).thenAnswer((_) async => 'pl');
      },
      build: () => LanguageCubit(mockRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<LanguagePolish>()],
    );

    blocTest<LanguageCubit, LanguageState>(
      'checkAppStart emits LanguageEnglish when stored language is en',
      setUp: () {
        when(
          () => mockRepository.getCurrentLanguage(),
        ).thenAnswer((_) async => 'en');
      },
      build: () => LanguageCubit(mockRepository),
      wait: const Duration(milliseconds: 100),
      expect: () => [isA<LanguageEnglish>()],
    );

    test('checkAppStart emits LanguageEnglish on error', () async {
      when(
        () => mockRepository.getCurrentLanguage(),
      ).thenThrow(Exception('Storage error'));

      final cubit = LanguageCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));

      // State should remain LanguageEnglish (default) after error
      expect(cubit.state, isA<LanguageEnglish>());
    });

    test(
      'switchToPolish emits LanguagePolish and saves to repository',
      () async {
        when(
          () => mockRepository.getCurrentLanguage(),
        ).thenAnswer((_) async => 'en');
        when(
          () => mockRepository.setCurrentLanguage('pl'),
        ).thenAnswer((_) async {});

        final cubit = LanguageCubit(mockRepository);

        // Wait for checkAppStart to complete
        await Future.delayed(const Duration(milliseconds: 100));
        expect(cubit.state, isA<LanguageEnglish>());

        cubit.switchToPolish();

        // Wait for async operations to complete
        await Future.delayed(const Duration(milliseconds: 100));

        expect(cubit.state, isA<LanguagePolish>());
        verify(() => mockRepository.setCurrentLanguage('pl')).called(1);
      },
    );

    test(
      'switchToEnglish emits LanguageEnglish and saves to repository',
      () async {
        when(
          () => mockRepository.getCurrentLanguage(),
        ).thenAnswer((_) async => 'pl');
        when(
          () => mockRepository.setCurrentLanguage('en'),
        ).thenAnswer((_) async {});

        final cubit = LanguageCubit(mockRepository);

        // Wait for checkAppStart to complete
        await Future.delayed(const Duration(milliseconds: 100));
        expect(cubit.state, isA<LanguagePolish>());

        cubit.switchToEnglish();

        // Wait for async operations to complete
        await Future.delayed(const Duration(milliseconds: 100));

        expect(cubit.state, isA<LanguageEnglish>());
        verify(() => mockRepository.setCurrentLanguage('en')).called(1);
      },
    );

    test('can switch from English to Polish and back', () async {
      when(
        () => mockRepository.getCurrentLanguage(),
      ).thenAnswer((_) async => 'en');
      when(
        () => mockRepository.setCurrentLanguage(any()),
      ).thenAnswer((_) async {});

      final cubit = LanguageCubit(mockRepository);

      // Wait for checkAppStart to complete
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<LanguageEnglish>());

      cubit.switchToPolish();
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<LanguagePolish>());

      cubit.switchToEnglish();
      await Future.delayed(const Duration(milliseconds: 100));
      expect(cubit.state, isA<LanguageEnglish>());
    });
  });
}
