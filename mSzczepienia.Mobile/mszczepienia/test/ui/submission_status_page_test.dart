import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/citizen_submission_status.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/citizen/citizen_state.dart';
import 'package:mszczepienia/ui/submission_process/submission_status_page.dart';

// Mock CitizenCubit for loading state
class MockCitizenCubitLoading extends Mock implements CitizenCubit {
  @override
  Stream<CitizenState> get stream => Stream.value(CitizenLoading());

  @override
  CitizenState get state => CitizenLoading();
}

// Mock CitizenCubit for NotUploaded status
class MockCitizenCubitNotUploaded extends Mock implements CitizenCubit {
  final CitizenSubmissionStatus _status = CitizenSubmissionStatus(
    submissionStatus: 'NotUploaded',
  );

  @override
  Stream<CitizenState> get stream => Stream.value(CitizenLoaded(_status));

  @override
  CitizenState get state => CitizenLoaded(_status);
}

// Mock CitizenCubit for Pending status
class MockCitizenCubitPending extends Mock implements CitizenCubit {
  final CitizenSubmissionStatus _status = CitizenSubmissionStatus(
    submissionStatus: 'Pending',
  );

  @override
  Stream<CitizenState> get stream => Stream.value(CitizenLoaded(_status));

  @override
  CitizenState get state => CitizenLoaded(_status);
}

// Mock CitizenCubit for Accepted status
class MockCitizenCubitAccepted extends Mock implements CitizenCubit {
  final CitizenSubmissionStatus _status = CitizenSubmissionStatus(
    submissionStatus: 'Accepted',
  );

  @override
  Stream<CitizenState> get stream => Stream.value(CitizenLoaded(_status));

  @override
  CitizenState get state => CitizenLoaded(_status);
}

// Mock CitizenCubit for Rejected status
class MockCitizenCubitRejected extends Mock implements CitizenCubit {
  final CitizenSubmissionStatus _status = CitizenSubmissionStatus(
    submissionStatus: 'Rejected',
  );

  @override
  Stream<CitizenState> get stream => Stream.value(CitizenLoaded(_status));

  @override
  CitizenState get state => CitizenLoaded(_status);
}

// Mock CitizenCubit for error state
class MockCitizenCubitError extends Mock implements CitizenCubit {
  @override
  Stream<CitizenState> get stream =>
      Stream.value(CitizenError('Error loading status'));

  @override
  CitizenState get state => CitizenError('Error loading status');
}

void main() {
  late MockCitizenCubitLoading mockCubitLoading;
  late MockCitizenCubitNotUploaded mockCubitNotUploaded;
  late MockCitizenCubitPending mockCubitPending;
  late MockCitizenCubitAccepted mockCubitAccepted;
  late MockCitizenCubitRejected mockCubitRejected;
  late MockCitizenCubitError mockCubitError;

  setUp(() {
    mockCubitLoading = MockCitizenCubitLoading();
    mockCubitNotUploaded = MockCitizenCubitNotUploaded();
    mockCubitPending = MockCitizenCubitPending();
    mockCubitAccepted = MockCitizenCubitAccepted();
    mockCubitRejected = MockCitizenCubitRejected();
    mockCubitError = MockCitizenCubitError();

    // Setup stubs for getCitizenStatus
    when(() => mockCubitLoading.getCitizenStatus()).thenAnswer((_) async {});
    when(
      () => mockCubitNotUploaded.getCitizenStatus(),
    ).thenAnswer((_) async {});
    when(() => mockCubitPending.getCitizenStatus()).thenAnswer((_) async {});
    when(() => mockCubitAccepted.getCitizenStatus()).thenAnswer((_) async {});
    when(() => mockCubitRejected.getCitizenStatus()).thenAnswer((_) async {});
    when(() => mockCubitError.getCitizenStatus()).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({required CitizenCubit cubit}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<CitizenCubit>.value(
        value: cubit,
        child: const SubmissionStatusPage(),
      ),
    );
  }

  group('SubmissionStatusPage Widget Tests', () {
    testWidgets('SubmissionStatusPage renders correctly', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.byType(SubmissionStatusPage), findsOneWidget);
    });

    testWidgets('Page title is present', (WidgetTester tester) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.text('Vaccination Booklet'), findsOneWidget);
    });

    testWidgets('Upload new photo option is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.text('Upload New Photo'), findsOneWidget);
    });

    testWidgets('Submission status label is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.text('Submission Status'), findsOneWidget);
    });

    testWidgets('Shows loading indicator when loading', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('Shows NotUploaded status', (WidgetTester tester) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.text('Not Uploaded'), findsOneWidget);
    });

    testWidgets('Shows Pending status', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitPending));
      await tester.pumpAndSettle();

      expect(find.text('Pending Verification'), findsOneWidget);
    });

    testWidgets('Shows Accepted status', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitAccepted));
      await tester.pumpAndSettle();

      expect(find.text('Approved'), findsOneWidget);
    });

    testWidgets('Shows Rejected status', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitRejected));
      await tester.pumpAndSettle();

      expect(find.text('Rejected'), findsOneWidget);
    });

    testWidgets('Camera icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.camera_alt_rounded), findsOneWidget);
    });

    testWidgets('SingleChildScrollView is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitNotUploaded),
      );
      await tester.pumpAndSettle();

      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });
  });
}
