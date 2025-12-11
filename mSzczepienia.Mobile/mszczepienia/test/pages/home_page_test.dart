import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/internal/app_user.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/home/home_page.dart';

// Mock AuthCubit for unauthenticated state
class MockAuthCubitUnauthenticated extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());

  @override
  AuthState get state => Unauthenticated();
}

// Mock AuthCubit for authenticated state
class MockAuthCubitAuthenticated extends Mock implements AuthCubit {
  final AppUser _user = AppUser(
    name: 'John',
    surname: 'Doe',
    email: 'john.doe@example.com',
    dateOfBirth: DateTime(1990, 1, 15),
    creationDate: DateTime(2024, 1, 1),
    pesel: '90011512345',
    defaultMedicalFacility: 'Test Hospital',
  );

  @override
  Stream<AuthState> get stream => Stream.value(Authenticated(_user));

  @override
  AuthState get state => Authenticated(_user);
}

void main() {
  late MockAuthCubitUnauthenticated mockAuthCubitUnauthenticated;
  late MockAuthCubitAuthenticated mockAuthCubitAuthenticated;

  setUp(() {
    mockAuthCubitUnauthenticated = MockAuthCubitUnauthenticated();
    mockAuthCubitAuthenticated = MockAuthCubitAuthenticated();
  });

  Widget createWidgetUnderTest({bool authenticated = false}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<AuthCubit>.value(
        value: authenticated
            ? mockAuthCubitAuthenticated
            : mockAuthCubitUnauthenticated,
        child: const HomePage(),
      ),
    );
  }

  group('HomePage Widget Tests', () {
    testWidgets('HomePage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if the page loaded correctly
      expect(find.byType(HomePage), findsOneWidget);
      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('Shows greeting for unauthenticated user',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(authenticated: false));
      await tester.pumpAndSettle();

      // Should show generic greeting
      expect(find.text('Good morning!'), findsOneWidget);
    });

    testWidgets('Shows personalized greeting for authenticated user',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Should show personalized greeting with user's name
      expect(find.text('Good morning, John'), findsOneWidget);
    });

    testWidgets('Vaccination Booklet option is present',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Vaccination Booklet option
      expect(find.text('Vaccination Booklet'), findsOneWidget);
    });

    testWidgets('Vaccination History option is present',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Vaccination History option
      expect(find.text('Vaccination History'), findsOneWidget);
    });

    testWidgets('Settings option is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Settings option
      expect(find.text('Settings'), findsOneWidget);
    });

    testWidgets('Your Account option is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Your Account option
      expect(find.text('Your Account'), findsOneWidget);
    });

    testWidgets('Camera icon for Vaccination Booklet is present',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for camera icon
      expect(find.byIcon(Icons.camera_alt_rounded), findsOneWidget);
    });

    testWidgets('History icon for Vaccination History is present',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for history icon
      expect(find.byIcon(Icons.history_rounded), findsOneWidget);
    });

    testWidgets('Settings icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for settings icon
      expect(find.byIcon(Icons.settings_rounded), findsOneWidget);
    });

    testWidgets('Account icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for account icon
      expect(find.byIcon(Icons.account_circle_rounded), findsOneWidget);
    });

    testWidgets('All four menu options are tappable',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      expect(find.byType(InkWell), findsAtLeastNWidgets(4));
    });

    testWidgets('Menu option descriptions are present',
        (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for descriptions
      expect(find.text('Add your vaccination booklet'), findsOneWidget);
      expect(find.text('View your vaccination history'), findsOneWidget);
      expect(find.text('Change application settings'), findsOneWidget);
      expect(find.text('View your account details'), findsOneWidget);
    });
  });
}
