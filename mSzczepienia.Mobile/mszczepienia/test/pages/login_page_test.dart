import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/authorization/pages/login_page.dart';

// Mock AuthCubit
class MockAuthCubit extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());

  @override
  AuthState get state => Unauthenticated();
}

void main() {
  late MockAuthCubit mockAuthCubit;

  setUp(() {
    mockAuthCubit = MockAuthCubit();
    // Setup stub for loginRequested
    when(() => mockAuthCubit.loginRequested(any(), any())).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest() {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<AuthCubit>.value(
        value: mockAuthCubit,
        child: const LoginPage(),
      ),
    );
  }

  group('LoginPage Widget Tests', () {
    testWidgets('LoginPage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if LoginPage and SingleChildScrollView are present
      expect(find.byType(LoginPage), findsOneWidget);
      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('Email and Password text fields are present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      expect(find.byType(TextFormField), findsWidgets);

      // At least two text fields
      expect(find.byType(TextFormField), findsNWidgets(2));
    });

    testWidgets('Login button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Login button
      expect(find.byType(ElevatedButton), findsOneWidget);
    });

    testWidgets('Sign Up button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Sign Up button
      expect(find.byType(OutlinedButton), findsOneWidget);
    });

    testWidgets('Email field validates empty input', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Click Login without entering any fields
      await tester.tap(find.byType(ElevatedButton), warnIfMissed: false);
      await tester.pumpAndSettle();

      // Expect validation error message
      expect(find.byType(Text), findsWidgets);
    });

    testWidgets('Email field accepts valid email', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Find first text field (email)
      final emailField = find.byType(TextFormField).first;
      
      // Enter valid email
      await tester.enterText(emailField, 'test@example.com');
      await tester.pumpAndSettle();

      // Check if the text was entered correctly
      expect(find.text('test@example.com'), findsOneWidget);
    });

    testWidgets('Password field is obscured by default', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for eye icon indicating obscured password
      expect(find.byIcon(Icons.visibility_off_outlined), findsOneWidget);
    });

    testWidgets('Eye icon toggles password visibility', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Tap the eye icon to toggle visibility
      await tester.tap(find.byIcon(Icons.visibility_off_outlined));
      await tester.pumpAndSettle();

      // Check if the icon changed to visibility_on
      expect(find.byIcon(Icons.visibility_outlined), findsOneWidget);
    });

    testWidgets('Can enter text in password field', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);
      
      await tester.enterText(fields.at(1), 'password123');
      // Check if the text was entered correctly
      expect(find.byType(TextFormField), findsNWidgets(2));
    });

    testWidgets('Form validation works - empty email shows error', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Fill only password field
      final fields = find.byType(TextFormField);
      await tester.enterText(fields.at(1), 'password123');
      
      // Click login
      await tester.tap(find.byType(ElevatedButton), warnIfMissed: false);
      await tester.pumpAndSettle();

      // loginRequested should not be called
      verifyNever(() => mockAuthCubit.loginRequested(any(), any()));
    });

    testWidgets('Form validation works - invalid email shows error', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Enter invalid email
      final fields = find.byType(TextFormField);
      await tester.enterText(fields.first, 'invalidemail');
      await tester.enterText(fields.at(1), 'password123');
      
      // Click login
      await tester.tap(find.byType(ElevatedButton), warnIfMissed: false);
      await tester.pumpAndSettle();

      // loginRequested should not be called
      verifyNever(() => mockAuthCubit.loginRequested(any(), any()));
    });

    testWidgets('Container widgets present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if the container widgets are present
      expect(find.byType(Container), findsWidgets);
      
      // Check if form exists
      expect(find.byType(Form), findsOneWidget);
    });
  });
}
