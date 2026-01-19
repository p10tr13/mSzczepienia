import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_cubit.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_state.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/pages/add_entry_page.dart';

// Mock OcrSubmissionCubit for loaded state
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
  ];

  @override
  Stream<OcrSubmissionState> get stream =>
      Stream.value(OcrSubmissionLoaded(_entries));

  @override
  OcrSubmissionState get state => OcrSubmissionLoaded(_entries);
}

// Mock VaccinesCubit
class MockVaccinesCubit extends Mock implements VaccinesCubit {
  @override
  Stream<VaccinesState> get stream => Stream.value(VaccinesInitial());

  @override
  VaccinesState get state => VaccinesInitial();
}

class FakeVaccinationEntryModel extends Fake implements VaccinationEntryModel {}

void main() {
  late MockOcrSubmissionCubitLoaded mockCubitLoaded;
  late MockVaccinesCubit mockVaccinesCubit;

  setUpAll(() {
    registerFallbackValue(FakeVaccinationEntryModel());
  });

  setUp(() {
    mockCubitLoaded = MockOcrSubmissionCubitLoaded();
    mockVaccinesCubit = MockVaccinesCubit();

    when(() => mockCubitLoaded.addEntry(any())).thenReturn(null);
    when(() => mockVaccinesCubit.loadVaccines(any())).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({required OcrSubmissionCubit cubit}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: MultiBlocProvider(
        providers: [
          BlocProvider<OcrSubmissionCubit>.value(value: cubit),
          BlocProvider<VaccinesCubit>.value(value: mockVaccinesCubit),
        ],
        child: const AddEntryPage(),
      ),
    );
  }

  group('AddEntryPage Widget Tests', () {
    testWidgets('AddEntryPage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(AddEntryPage), findsOneWidget);
    });

    testWidgets('Page title is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Add Vaccination'), findsOneWidget);
    });

    testWidgets('Vaccination date field is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Vaccination Date'), findsOneWidget);
    });

    testWidgets('Batch number field is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Batch Number'), findsOneWidget);
    });

    testWidgets('Save changes button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Save Changes'), findsOneWidget);
    });

    testWidgets('SingleChildScrollView is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('Form is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(Form), findsOneWidget);
    });

    testWidgets('ElevatedButton for save is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(ElevatedButton), findsOneWidget);
    });
  });
}
