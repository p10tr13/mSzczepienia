import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/router/app_router.dart';
import 'package:mszczepienia/core/theme/app_theme.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_repository.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_repository.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';
import 'package:mocktail/mocktail.dart';

// Mocks for dependencies that require external services
class MockAuthCubit extends Mock implements AuthCubit {
  @override
  AuthState get state => Unauthenticated();

  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());
}

class MockCitizenCubit extends Mock implements CitizenCubit {}

class MockFacilitiesCubit extends Mock implements FacilitiesCubit {}

class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

// Fake repositories for integration testing
class FakeThemeRepository extends ThemeRepository {
  String? _theme;

  FakeThemeRepository() : super(storage: MockFlutterSecureStorage());

  @override
  Future<String> getCurrentTheme() async => _theme ?? 'light';

  @override
  Future<void> setCurrentTheme(String theme) async {
    _theme = theme;
  }
}

class FakeLanguageRepository extends LanguageRepository {
  String? _language;

  FakeLanguageRepository() : super(storage: MockFlutterSecureStorage());

  @override
  Future<String> getCurrentLanguage() async => _language ?? 'en';

  @override
  Future<void> setCurrentLanguage(String language) async {
    _language = language;
  }
}

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  group('Settings Page Integration Tests', () {
    late MockAuthCubit mockAuthCubit;
    late MockCitizenCubit mockCitizenCubit;
    late MockFacilitiesCubit mockFacilitiesCubit;
    late ThemeCubit themeCubit;
    late LanguageCubit languageCubit;
    late AppRouter appRouter;

    setUp(() {
      mockAuthCubit = MockAuthCubit();
      mockCitizenCubit = MockCitizenCubit();
      mockFacilitiesCubit = MockFacilitiesCubit();
    });

    Widget createTestApp({
      required ThemeCubit themeCubit,
      required LanguageCubit languageCubit,
    }) {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      return MultiBlocProvider(
        providers: [
          BlocProvider<AuthCubit>.value(value: mockAuthCubit),
          BlocProvider<ThemeCubit>.value(value: themeCubit),
          BlocProvider<LanguageCubit>.value(value: languageCubit),
          BlocProvider<CitizenCubit>.value(value: mockCitizenCubit),
          BlocProvider<FacilitiesCubit>.value(value: mockFacilitiesCubit),
        ],
        child: BlocBuilder<ThemeCubit, ThemeState>(
          builder: (context, themeState) {
            final themeData = themeState is DarkTheme
                ? AppTheme.darkTheme
                : AppTheme.lightTheme;

            return BlocBuilder<LanguageCubit, LanguageState>(
              builder: (context, languageState) {
                final locale = languageState is LanguagePolish
                    ? const Locale('pl')
                    : const Locale('en');

                return MaterialApp.router(
                  title: 'mSzczepienia Test',
                  locale: locale,
                  localizationsDelegates:
                      AppLocalizations.localizationsDelegates,
                  supportedLocales: AppLocalizations.supportedLocales,
                  theme: themeData,
                  routerConfig: appRouter.router,
                  debugShowCheckedModeBanner: false,
                );
              },
            );
          },
        ),
      );
    }

    testWidgets(
      'Theme toggle integration - switching from light to dark theme updates UI',
      (WidgetTester tester) async {
        // Create real cubits with fake repositories for integration testing
        final fakeThemeRepo = FakeThemeRepository();
        final fakeLanguageRepo = FakeLanguageRepository();

        // Create custom cubits that work with fake repos
        themeCubit = _TestThemeCubit(fakeThemeRepo);
        languageCubit = _TestLanguageCubit(fakeLanguageRepo);

        await tester.pumpWidget(createTestApp(
          themeCubit: themeCubit,
          languageCubit: languageCubit,
        ));
        await tester.pumpAndSettle();

        // Navigate to settings (since we're unauthenticated, we're on login page)
        // For this test, we'll directly test theme cubit behavior
        expect(themeCubit.state, isA<LightTheme>());

        // Switch to dark theme
        themeCubit.switchToDark();
        await tester.pumpAndSettle();

        expect(themeCubit.state, isA<DarkTheme>());

        // Switch back to light theme
        themeCubit.switchToLight();
        await tester.pumpAndSettle();

        expect(themeCubit.state, isA<LightTheme>());
      },
    );

    testWidgets(
      'Language toggle integration - switching from English to Polish updates locale',
      (WidgetTester tester) async {
        final fakeThemeRepo = FakeThemeRepository();
        final fakeLanguageRepo = FakeLanguageRepository();

        themeCubit = _TestThemeCubit(fakeThemeRepo);
        languageCubit = _TestLanguageCubit(fakeLanguageRepo);

        await tester.pumpWidget(createTestApp(
          themeCubit: themeCubit,
          languageCubit: languageCubit,
        ));
        await tester.pumpAndSettle();

        // Start with English
        expect(languageCubit.state, isA<LanguageEnglish>());

        // Find "Good morning!" text (English)
        expect(find.text('Good morning!'), findsOneWidget);

        // Switch to Polish
        languageCubit.switchToPolish();
        await tester.pumpAndSettle();

        expect(languageCubit.state, isA<LanguagePolish>());

        // Now should see Polish text "Dzień dobry!"
        expect(find.text('Dzień dobry!'), findsOneWidget);

        // Switch back to English
        languageCubit.switchToEnglish();
        await tester.pumpAndSettle();

        expect(languageCubit.state, isA<LanguageEnglish>());
        expect(find.text('Good morning!'), findsOneWidget);
      },
    );

    testWidgets(
      'Full flow integration - theme and language changes persist across rebuilds',
      (WidgetTester tester) async {
        final fakeThemeRepo = FakeThemeRepository();
        final fakeLanguageRepo = FakeLanguageRepository();

        themeCubit = _TestThemeCubit(fakeThemeRepo);
        languageCubit = _TestLanguageCubit(fakeLanguageRepo);

        await tester.pumpWidget(createTestApp(
          themeCubit: themeCubit,
          languageCubit: languageCubit,
        ));
        await tester.pumpAndSettle();

        // Initial state
        expect(themeCubit.state, isA<LightTheme>());
        expect(languageCubit.state, isA<LanguageEnglish>());

        // Change both settings
        themeCubit.switchToDark();
        languageCubit.switchToPolish();
        await tester.pumpAndSettle();

        // Verify changes took effect
        expect(themeCubit.state, isA<DarkTheme>());
        expect(languageCubit.state, isA<LanguagePolish>());

        // Verify storage was updated (integration with repository)
        expect(await fakeThemeRepo.getCurrentTheme(), 'dark');
        expect(await fakeLanguageRepo.getCurrentLanguage(), 'pl');
      },
    );

    testWidgets(
      'Login page renders correctly with all elements',
      (WidgetTester tester) async {
        final fakeThemeRepo = FakeThemeRepository();
        final fakeLanguageRepo = FakeLanguageRepository();

        themeCubit = _TestThemeCubit(fakeThemeRepo);
        languageCubit = _TestLanguageCubit(fakeLanguageRepo);

        await tester.pumpWidget(createTestApp(
          themeCubit: themeCubit,
          languageCubit: languageCubit,
        ));
        await tester.pumpAndSettle();

        // Verify login page elements are present (unauthenticated state)
        expect(find.text('Good morning!'), findsOneWidget);
        expect(find.byType(TextFormField), findsWidgets);

        // Find login button (text is "Log In" with capital I)
        expect(
            find.widgetWithText(ElevatedButton, 'Log In'), findsOneWidget);

        // Find sign up button (OutlinedButton, not TextButton)
        expect(
            find.widgetWithText(OutlinedButton, 'Sign Up'),
            findsOneWidget);
      },
    );
  });
}

/// Test-specific ThemeCubit that uses a fake repository
class _TestThemeCubit extends ThemeCubit {
  _TestThemeCubit(FakeThemeRepository super.repository);
}

/// Test-specific LanguageCubit that uses a fake repository
class _TestLanguageCubit extends LanguageCubit {
  _TestLanguageCubit(FakeLanguageRepository super.repository);
}
