import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/account/account_page.dart';

// Mock AuthCubit for unauthenticated state
class MockAuthCubitUnauthenticated extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());

  @override
  AuthState get state => Unauthenticated();
}

// Mock AuthCubit for authenticated state
class MockAuthCubitAuthenticated extends Mock implements AuthCubit {
  final CitizenDetailsModel _user = CitizenDetailsModel(
    name: 'John',
    surname: 'Doe',
    email: 'john.doe@example.com',
    dateOfBirth: DateTime(1990, 1, 15),
    creationDate: DateTime(2024, 1, 1),
    pesel: '90011512345',
    facilityName: 'Test Hospital',
    submissionStatus: 'NotUploaded',
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

    // Setup stubs
    when(() => mockAuthCubitAuthenticated.logoutRequested())
        .thenAnswer((_) async {});
    when(() => mockAuthCubitUnauthenticated.logoutRequested())
        .thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({bool authenticated = true}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<AuthCubit>.value(
        value: authenticated
            ? mockAuthCubitAuthenticated
            : mockAuthCubitUnauthenticated,
        child: const AccountPage(),
      ),
    );
  }

  Future<void> pumpWidgetWithSize(WidgetTester tester, Widget widget) async {
    tester.view.physicalSize = const Size(800, 1600);
    tester.view.devicePixelRatio = 1.0;
    addTearDown(() {
      tester.view.resetPhysicalSize();
      tester.view.resetDevicePixelRatio();
    });
    await tester.pumpWidget(widget);
  }

  group('AccountPage Widget Tests', () {
    testWidgets('AccountPage renders correctly', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if the page loaded correctly
      expect(find.byType(AccountPage), findsOneWidget);
      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('Your Account title is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Your Account title
      expect(find.text('Your Account'), findsOneWidget);
    });

    testWidgets('User name is displayed when authenticated',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Check if user's name is displayed
      expect(find.text('John'), findsOneWidget);
    });

    testWidgets('User surname is displayed when authenticated',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Check if user's surname is displayed
      expect(find.text('Doe'), findsOneWidget);
    });

    testWidgets('User email is displayed when authenticated',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Check if user's email is displayed
      expect(find.text('john.doe@example.com'), findsOneWidget);
    });

    testWidgets('User PESEL is displayed when authenticated',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Check if user's PESEL is displayed
      expect(find.text('90011512345'), findsOneWidget);
    });

    testWidgets('User medical facility is displayed when authenticated',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Check if user's medical facility is displayed
      expect(find.text('Test Hospital'), findsOneWidget);
    });

    testWidgets('Name label is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Name label
      expect(find.text('Name'), findsOneWidget);
    });

    testWidgets('Surname label is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Surname label
      expect(find.text('Surname'), findsOneWidget);
    });

    testWidgets('Email label is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Email label
      expect(find.text('Email'), findsOneWidget);
    });

    testWidgets('Date of birth label is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Date of birth label
      expect(find.text('Date of birth'), findsOneWidget);
    });

    testWidgets('PESEL label is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for PESEL label
      expect(find.text('PESEL'), findsOneWidget);
    });

    testWidgets('Medical Facility label is present',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Medical Facility label
      expect(find.text('Medical Facility'), findsOneWidget);
    });

    testWidgets('Call Support option is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Call Support option
      expect(find.text('Call Support'), findsOneWidget);
    });

    testWidgets('Log Out option is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for Log Out option
      expect(find.text('Log Out'), findsOneWidget);
    });

    testWidgets('Phone icon is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for phone icon
      expect(find.byIcon(Icons.phone), findsOneWidget);
    });

    testWidgets('Logout icon is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for logout icon
      expect(find.byIcon(Icons.logout), findsOneWidget);
    });

    testWidgets('Person icon is present', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check for person icon
      expect(find.byIcon(Icons.person), findsOneWidget);
    });

    testWidgets('Tapping Log Out calls logoutRequested',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(
          tester, createWidgetUnderTest(authenticated: true));
      await tester.pumpAndSettle();

      // Tap on Log Out option
      await tester.tap(find.text('Log Out'));
      await tester.pumpAndSettle();

      // Verify logoutRequested was called
      verify(() => mockAuthCubitAuthenticated.logoutRequested()).called(1);
    });
    
    testWidgets('InkWell widgets are present for Call Support and Log Out',
        (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Should have at least 2 InkWell for Call Support and Log Out (may have more due to nested widgets)
      expect(find.byType(InkWell), findsAtLeastNWidgets(2));
    });
  });
}
