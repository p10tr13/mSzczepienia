import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/pages/analyze_page.dart';

// Mock OcrSubmissionCubit for processing loading state
class MockOcrSubmissionCubitProcessingLoading extends Mock
    implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionProcessingLoading());

  @override
  OcrSubmissionState get state => OcrSubmissionProcessingLoading();
}

// Mock OcrSubmissionCubit for submitting loading state
class MockOcrSubmissionCubitSubmittingLoading extends Mock
    implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionSubmittingLoading());

  @override
  OcrSubmissionState get state => OcrSubmissionSubmittingLoading();
}

// Mock OcrSubmissionCubit for cancelling loading state
class MockOcrSubmissionCubitCancellingLoading extends Mock
    implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionCancellingLoading());

  @override
  OcrSubmissionState get state => OcrSubmissionCancellingLoading();
}

// Mock OcrSubmissionCubit for error state
class MockOcrSubmissionCubitError extends Mock implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionError('Error occurred'));

  @override
  OcrSubmissionState get state => OcrSubmissionError('Error occurred');
}

// Mock OcrSubmissionCubit for loaded state with entries
class MockOcrSubmissionCubitLoaded extends Mock implements OcrSubmissionCubit {
  final List<VaccinationEntryModel> _entries = [
    VaccinationEntryModel(
      id: 0,
      vaccineNameOcr: 'BCG',
      dateOcr: '2023-05-15',
      batchOcr: 'BATCH001',
      vaccineName: 'BCG',
      date: DateTime(2023, 5, 15),
      batch: 'BATCH001',
      status: 'Valid',
    ),
    VaccinationEntryModel(
      id: 1,
      vaccineNameOcr: 'MMR',
      dateOcr: '2023-06-20',
      batchOcr: 'BATCH002',
      vaccineName: 'MMR',
      date: DateTime(2023, 6, 20),
      batch: 'BATCH002',
      status: 'Valid',
    ),
  ];

  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionLoaded(_entries));

  @override
  OcrSubmissionState get state => OcrSubmissionLoaded(_entries);
}

// Mock OcrSubmissionCubit for loaded state with empty entries
class MockOcrSubmissionCubitEmpty extends Mock implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionLoaded([]));

  @override
  OcrSubmissionState get state => OcrSubmissionLoaded([]);
}

void main() {
  late MockOcrSubmissionCubitProcessingLoading mockCubitProcessingLoading;
  late MockOcrSubmissionCubitSubmittingLoading mockCubitSubmittingLoading;
  late MockOcrSubmissionCubitCancellingLoading mockCubitCancellingLoading;
  late MockOcrSubmissionCubitError mockCubitError;
  late MockOcrSubmissionCubitLoaded mockCubitLoaded;
  late MockOcrSubmissionCubitEmpty mockCubitEmpty;

  setUp(() {
    mockCubitProcessingLoading = MockOcrSubmissionCubitProcessingLoading();
    mockCubitSubmittingLoading = MockOcrSubmissionCubitSubmittingLoading();
    mockCubitCancellingLoading = MockOcrSubmissionCubitCancellingLoading();
    mockCubitError = MockOcrSubmissionCubitError();
    mockCubitLoaded = MockOcrSubmissionCubitLoaded();
    mockCubitEmpty = MockOcrSubmissionCubitEmpty();

    when(() => mockCubitLoaded.removeEntry(any())).thenReturn(null);
    when(() => mockCubitLoaded.cancelOcrSubmission()).thenAnswer((_) async {});
    when(() => mockCubitLoaded.submitOcrSubmission()).thenAnswer((_) async {});
    when(() => mockCubitEmpty.cancelOcrSubmission()).thenAnswer((_) async {});
    when(() => mockCubitEmpty.submitOcrSubmission()).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({required OcrSubmissionCubit cubit}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<OcrSubmissionCubit>.value(
        value: cubit,
        child: const AnalyzePage(),
      ),
    );
  }

  group('AnalyzePage Widget Tests', () {
    testWidgets('AnalyzePage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(AnalyzePage), findsOneWidget);
    });

    testWidgets('Shows loading page when processing', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitProcessingLoading),
      );
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
      expect(find.textContaining('Analyzing'), findsOneWidget);
    });

    testWidgets('Shows loading page when submitting', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitSubmittingLoading),
      );
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
      expect(find.textContaining('Sending'), findsOneWidget);
    });

    testWidgets('Shows loading page when cancelling', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(cubit: mockCubitCancellingLoading),
      );
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
      expect(find.textContaining('Cancelling'), findsOneWidget);
    });

    testWidgets('Shows error page when error occurs', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitError));
      await tester.pumpAndSettle();

      expect(find.text('An error occurred'), findsOneWidget);
    });

    testWidgets('Page title is present when loaded', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Vaccination Booklet'), findsOneWidget);
    });

    testWidgets('OCR description is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.textContaining('OCR process'), findsOneWidget);
    });

    testWidgets('Shows vaccination entries when loaded', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('BCG'), findsOneWidget);
      expect(find.text('MMR'), findsOneWidget);
    });

    testWidgets('Shows no vaccinations message when empty', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitEmpty));
      await tester.pumpAndSettle();

      expect(find.text('No vaccinations found.'), findsOneWidget);
    });

    testWidgets('Add vaccination button is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Add Vaccination'), findsOneWidget);
    });

    testWidgets('Cancel submission button is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Cancel Submission'), findsOneWidget);
    });

    testWidgets('Submit for verification button is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Submit for Verification'), findsOneWidget);
    });

    testWidgets('Delete icon is present for entries', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.delete_rounded), findsWidgets);
    });

    testWidgets('Edit icon is present for entries', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.edit_rounded), findsWidgets);
    });
  });
}
