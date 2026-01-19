import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';
import 'package:mszczepienia/ui/settings/settings_page.dart';

// Mock AuthCubit
class MockAuthCubit extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());

  @override
  AuthState get state => Unauthenticated();
}

// Mock ThemeCubit
class MockThemeCubit extends Mock implements ThemeCubit {
  ThemeState _state = LightTheme();

  @override
  Stream<ThemeState> get stream => Stream.value(_state);

  @override
  ThemeState get state => _state;

  void setState(ThemeState newState) {
    _state = newState;
  }
}

// Mock LanguageCubit
class MockLanguageCubit extends Mock implements LanguageCubit {
  LanguageState _state = LanguageEnglish();

  @override
  Stream<LanguageState> get stream => Stream.value(_state);

  @override
  LanguageState get state => _state;

  void setState(LanguageState newState) {
    _state = newState;
  }
}

void main() {
  late MockAuthCubit mockAuthCubit;
  late MockThemeCubit mockThemeCubit;
  late MockLanguageCubit mockLanguageCubit;

  setUp(() {
    mockAuthCubit = MockAuthCubit();
    mockThemeCubit = MockThemeCubit();
    mockLanguageCubit = MockLanguageCubit();

    // Setup stubs
    when(() => mockThemeCubit.switchToDark()).thenReturn(null);
    when(() => mockThemeCubit.switchToLight()).thenReturn(null);
    when(() => mockLanguageCubit.switchToPolish()).thenReturn(null);
    when(() => mockLanguageCubit.switchToEnglish()).thenReturn(null);
  });

  Widget createWidgetUnderTest() {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: MultiBlocProvider(
        providers: [
          BlocProvider<AuthCubit>.value(value: mockAuthCubit),
          BlocProvider<ThemeCubit>.value(value: mockThemeCubit),
          BlocProvider<LanguageCubit>.value(value: mockLanguageCubit),
        ],
        child: const SettingsPage(),
      ),
    );
  }

  group('SettingsPage Widget Tests', () {
    testWidgets('SettingsPage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if the page loaded correctly
      expect(find.byType(SettingsPage), findsOneWidget);
      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('Settings title is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Settings title
      expect(find.text('Settings'), findsOneWidget);
    });

    testWidgets('Theme option is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Theme option
      expect(find.text('Theme'), findsOneWidget);
    });

    testWidgets('Language option is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Language option
      expect(find.text('Language'), findsOneWidget);
    });

    testWidgets('Theme icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for theme icon
      expect(find.byIcon(Icons.color_lens_rounded), findsOneWidget);
    });

    testWidgets('Language icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for language icon
      expect(find.byIcon(Icons.pin_drop_rounded), findsOneWidget);
    });

    testWidgets('Theme option shows dark mode text in light theme', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // In light theme, should show option to change to dark mode
      expect(find.text('Change to dark mode'), findsOneWidget);
    });

    testWidgets('Tapping theme option calls switchToDark when in light mode', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Tap on theme option by tapping on Theme text
      await tester.tap(find.text('Theme'));
      await tester.pumpAndSettle();

      // Verify switchToDark was called
      verify(() => mockThemeCubit.switchToDark()).called(1);
    });

    testWidgets(
      'Tapping language option calls switchToPolish when in English',
      (WidgetTester tester) async {
        await tester.pumpWidget(createWidgetUnderTest());
        await tester.pumpAndSettle();

        // Tap on language option by tapping on Language text
        await tester.tap(find.text('Language'));
        await tester.pumpAndSettle();

        // Verify switchToPolish was called
        verify(() => mockLanguageCubit.switchToPolish()).called(1);
      },
    );

    testWidgets('InkWell widgets are present for both options', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Should have at least 2 InkWell for theme and language (may have more due to nested widgets)
      expect(find.byType(InkWell), findsAtLeastNWidgets(2));
    });
  });
}
