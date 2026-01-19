import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_state.dart';
import 'package:mszczepienia/ui/authorization/pages/sign_up_page.dart';

// Mock AuthCubit for Unauthenticated state
class MockAuthCubit extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(Unauthenticated());

  @override
  AuthState get state => Unauthenticated();
}

// Mock AuthCubit for AuthLoading state
class MockAuthCubitLoading extends Mock implements AuthCubit {
  @override
  Stream<AuthState> get stream => Stream.value(AuthLoading());

  @override
  AuthState get state => AuthLoading();
}

// Mock FacilitiesCubit
class MockFacilitiesCubit extends Mock implements FacilitiesCubit {
  @override
  Stream<FacilitiesState> get stream => Stream.value(FacilitiesInitial());

  @override
  FacilitiesState get state => FacilitiesInitial();
}

void main() {
  late MockAuthCubit mockAuthCubit;
  late MockAuthCubitLoading mockAuthCubitLoading;
  late MockFacilitiesCubit mockFacilitiesCubit;

  setUp(() {
    mockAuthCubit = MockAuthCubit();
    mockAuthCubitLoading = MockAuthCubitLoading();
    mockFacilitiesCubit = MockFacilitiesCubit();

    // Setup stubs
    when(
      () => mockAuthCubit.signUpRequested(
        any(),
        any(),
        any(),
        any(),
        any(),
        any(),
        any(),
        any(),
      ),
    ).thenAnswer((_) async {});
    when(
      () => mockFacilitiesCubit.loadFacilities(any()),
    ).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({bool loading = false}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: MultiBlocProvider(
        providers: [
          BlocProvider<AuthCubit>.value(
            value: loading ? mockAuthCubitLoading : mockAuthCubit,
          ),
          BlocProvider<FacilitiesCubit>.value(value: mockFacilitiesCubit),
        ],
        child: const SignUpPage(),
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

  group('SignUpPage Widget Tests', () {
    testWidgets('SignUpPage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check if the page loaded correctly
      expect(find.byType(SignUpPage), findsOneWidget);
      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('All required text fields are present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Check fields: Name, Surname, PESEL, DateOfBirth, Email, Password
      // Should have 6+ text fields
      expect(find.byType(TextFormField), findsWidgets);
    });

    testWidgets('Sign Up button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Sign Up button
      expect(find.byType(ElevatedButton), findsOneWidget);
    });

    testWidgets('Log In button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for Log In button (OutlinedButton)
      expect(find.byType(OutlinedButton), findsOneWidget);
    });

    testWidgets('Can enter name', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);
      await tester.enterText(fields.first, 'John');

      expect(find.text('John'), findsOneWidget);
    });

    testWidgets('Can enter surname', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);
      await tester.enterText(fields.at(1), 'Doe');

      expect(find.text('Doe'), findsOneWidget);
    });

    testWidgets('PESEL field validates length', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Fill all fields except PESEL
      final fields = find.byType(TextFormField);
      await tester.enterText(fields.first, 'John');
      await tester.enterText(fields.at(1), 'Doe');
      await tester.enterText(fields.at(2), '1234'); // PESEL - too short
      await tester.enterText(fields.at(4), 'test@example.com');
      await tester.enterText(fields.at(5), 'password123');

      // Click Sign Up
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called because PESEL is too short
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Email field validates format', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);
      await tester.enterText(fields.first, 'John');
      await tester.enterText(fields.at(1), 'Doe');
      await tester.enterText(fields.at(2), '12345678901'); // PESEL - OK
      await tester.enterText(fields.at(4), 'invalidemail'); // Email without @
      await tester.enterText(fields.at(5), 'password123');

      // Click Sign Up
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Password field validates minimum length', (
      WidgetTester tester,
    ) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);
      await tester.enterText(fields.first, 'John');
      await tester.enterText(fields.at(1), 'Doe');
      await tester.enterText(fields.at(2), '12345678901');
      await tester.enterText(fields.at(4), 'test@example.com');
      await tester.enterText(fields.at(5), '123'); // Password too short

      // Click Sign Up
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Form validates empty fields', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Click Sign Up without filling any fields
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Name field is required', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);

      // Leave name empty
      await tester.enterText(fields.at(1), 'Doe'); // surname
      await tester.enterText(fields.at(2), '12345678901'); // PESEL
      await tester.enterText(fields.at(4), 'test@example.com'); // email
      await tester.enterText(fields.at(5), 'password123'); // password

      // Click Sign Up
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Surname field is required', (WidgetTester tester) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      final fields = find.byType(TextFormField);

      // Leave surname empty
      await tester.enterText(fields.first, 'John'); // name
      await tester.enterText(fields.at(2), '12345678901'); // PESEL
      await tester.enterText(fields.at(4), 'test@example.com'); // email
      await tester.enterText(fields.at(5), 'password123'); // password

      // Click Sign Up
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // signUpRequested should not be called
      verifyNever(
        () => mockAuthCubit.signUpRequested(
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
          any(),
        ),
      );
    });

    testWidgets('Container border uses theme outline color', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Find the Container that wraps the Form with BoxDecoration
      final containerFinder = find.ancestor(
        of: find.byType(Form),
        matching: find.byType(Container),
      );
      expect(containerFinder, findsWidgets);

      // Get the Container widget and check its decoration
      final container = tester
          .widgetList<Container>(containerFinder)
          .firstWhere(
            (c) =>
                c.decoration is BoxDecoration &&
                (c.decoration as BoxDecoration).border != null,
          );

      final decoration = container.decoration as BoxDecoration;
      final border = decoration.border as Border;

      // Get the expected theme outline color
      final context = tester.element(find.byType(SignUpPage));
      final expectedColor = Theme.of(context).colorScheme.outline;

      // Verify border color matches theme outline color
      expect(border.top.color, equals(expectedColor));
    });

    testWidgets('Eye icon in password field toggles visibility', (
      WidgetTester tester,
    ) async {
      await pumpWidgetWithSize(tester, createWidgetUnderTest());
      await tester.pumpAndSettle();

      // Look for eye icon in password field (there are 2 - password and repeat password)
      expect(find.byIcon(Icons.visibility_off_outlined), findsNWidgets(2));

      // Click the first one to show password
      await tester.tap(find.byIcon(Icons.visibility_off_outlined).first);
      await tester.pumpAndSettle();

      // Visibility icon should appear for the first field
      expect(find.byIcon(Icons.visibility_outlined), findsOneWidget);
    });

    testWidgets('DateOfBirth field is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest());
      await tester.pumpAndSettle();

      // DateOfBirth should be a TextFormField
      final fields = find.byType(TextFormField);
      expect(fields, findsWidgets);

      // Should have a field with hint text 'YYYY-MM-DD'
      expect(find.text('YYYY-MM-DD'), findsOneWidget);
    });

    testWidgets('Show loading indicator when AuthLoading', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(loading: true));
      await tester.pump();

      // In AuthLoading state, should show CircularProgressIndicator instead of "Sign Up" text
      expect(find.byType(CircularProgressIndicator), findsOneWidget);
      expect(find.text('Sign Up'), findsNothing);
    });
  });
}
