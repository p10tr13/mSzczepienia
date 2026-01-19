import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_state.dart';
import 'package:mszczepienia/ui/vaccination_history/vaccination_history_page.dart';

// Mock VaccinationRecordsCubit for loading state
class MockVaccinationRecordsCubitLoading extends Mock
    implements VaccinationRecordsCubit {
  @override
  Stream<VaccinationRecordsState> get stream =>
      Stream.value(VaccinationRecordsLoading());

  @override
  VaccinationRecordsState get state => VaccinationRecordsLoading();
}

// Mock VaccinationRecordsCubit for loaded state
class MockVaccinationRecordsCubitLoaded extends Mock
    implements VaccinationRecordsCubit {
  final List<VaccinationRecordOverviewModel> _records = [
    VaccinationRecordOverviewModel(
      id: 1,
      vaccineName: 'BCG',
      shortDesciriptionEn: 'Tuberculosis vaccine',
      shortDescriptionPl: 'Szczepionka przeciw gruźlicy',
      vaccinationDate: DateTime(2023, 5, 15),
      dosesNumber: 1,
    ),
    VaccinationRecordOverviewModel(
      id: 2,
      vaccineName: 'MMR',
      shortDesciriptionEn: 'Measles, Mumps, Rubella vaccine',
      shortDescriptionPl: 'Szczepionka przeciw odrze, śwince, różyczce',
      vaccinationDate: DateTime(2023, 6, 20),
      dosesNumber: 2,
    ),
  ];

  @override
  Stream<VaccinationRecordsState> get stream =>
      Stream.value(VaccinationRecordsLoaded(_records));

  @override
  VaccinationRecordsState get state => VaccinationRecordsLoaded(_records);
}

// Mock VaccinationRecordsCubit for error state
class MockVaccinationRecordsCubitError extends Mock
    implements VaccinationRecordsCubit {
  @override
  Stream<VaccinationRecordsState> get stream =>
      Stream.value(VaccinationRecordsError('Error loading records'));

  @override
  VaccinationRecordsState get state =>
      VaccinationRecordsError('Error loading records');
}

// Mock VaccinationRecordsCubit for empty list state
class MockVaccinationRecordsCubitEmpty extends Mock
    implements VaccinationRecordsCubit {
  @override
  Stream<VaccinationRecordsState> get stream =>
      Stream.value(VaccinationRecordsLoaded([]));

  @override
  VaccinationRecordsState get state => VaccinationRecordsLoaded([]);
}

void main() {
  late MockVaccinationRecordsCubitLoading mockCubitLoading;
  late MockVaccinationRecordsCubitLoaded mockCubitLoaded;
  late MockVaccinationRecordsCubitError mockCubitError;
  late MockVaccinationRecordsCubitEmpty mockCubitEmpty;

  setUp(() {
    mockCubitLoading = MockVaccinationRecordsCubitLoading();
    mockCubitLoaded = MockVaccinationRecordsCubitLoaded();
    mockCubitError = MockVaccinationRecordsCubitError();
    mockCubitEmpty = MockVaccinationRecordsCubitEmpty();

    // Setup stubs for loadHistory
    when(
      () => mockCubitLoading.loadHistory(any(), any(), any(), any()),
    ).thenAnswer((_) async {});
    when(
      () => mockCubitLoaded.loadHistory(any(), any(), any(), any()),
    ).thenAnswer((_) async {});
    when(
      () => mockCubitError.loadHistory(any(), any(), any(), any()),
    ).thenAnswer((_) async {});
    when(
      () => mockCubitEmpty.loadHistory(any(), any(), any(), any()),
    ).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({required VaccinationRecordsCubit cubit}) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<VaccinationRecordsCubit>.value(
        value: cubit,
        child: const VaccinationHistoryPage(),
      ),
    );
  }

  group('VaccinationHistoryPage Widget Tests', () {
    testWidgets('VaccinationHistoryPage renders correctly', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byType(VaccinationHistoryPage), findsOneWidget);
    });

    testWidgets('Page title is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.text('Your Vaccinations'), findsOneWidget);
    });

    testWidgets('Shows loading indicator when loading', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsWidgets);
    });

    testWidgets('Sorting dropdown is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byType(DropdownMenu<String>), findsOneWidget);
    });

    testWidgets('Sorting options are available', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      // Default sorting option should be visible
      expect(find.text('Alphabetical (A-Z)'), findsWidgets);
    });

    testWidgets('Sort icon is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byIcon(Icons.sort_rounded), findsWidgets);
    });
  });
}
