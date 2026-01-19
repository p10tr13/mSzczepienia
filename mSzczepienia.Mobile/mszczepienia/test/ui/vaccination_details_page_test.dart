import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_state.dart';
import 'package:mszczepienia/ui/vaccination_details/vaccination_details_page.dart';

// Mock VaccinationRecordDetailsCubit for loading state
class MockVaccinationRecordDetailsCubitLoading extends Mock
    implements VaccinationRecordDetailsCubit {
  @override
  Stream<VaccinationRecordDetailsState> get stream =>
      Stream.value(VaccinationRecordDetailsLoading());

  @override
  VaccinationRecordDetailsState get state => VaccinationRecordDetailsLoading();
}

// Mock VaccinationRecordDetailsCubit for loaded state
class MockVaccinationRecordDetailsCubitLoaded extends Mock
    implements VaccinationRecordDetailsCubit {
  final List<VaccinationRecordDetailsModel> _details = [
    VaccinationRecordDetailsModel(
      vaccineId: 1,
      vaccineName: 'BCG',
      manufacturer: 'Test Manufacturer',
      vaccineDescriptionPl: 'Opis szczepionki BCG',
      vaccineDescriptionEn: 'BCG vaccine description',
      shortDescriptionPl: 'Szczepionka przeciw gruźlicy',
      shortDescriptionEn: 'Tuberculosis vaccine',
      creationDate: DateTime(2020, 1, 1),
      diseasesPl: ['Gruźlica'],
      diseasesEn: ['Tuberculosis'],
      vaccinationDate: DateTime(2023, 5, 15),
      nextVaccinationDate: null,
      doseNumber: '1',
      batch: 'BATCH001',
      remarks: null,
    ),
  ];

  @override
  Stream<VaccinationRecordDetailsState> get stream =>
      Stream.value(VaccinationRecordDetailsLoaded(_details));

  @override
  VaccinationRecordDetailsState get state =>
      VaccinationRecordDetailsLoaded(_details);
}

// Mock VaccinationRecordDetailsCubit for error state
class MockVaccinationRecordDetailsCubitError extends Mock
    implements VaccinationRecordDetailsCubit {
  @override
  Stream<VaccinationRecordDetailsState> get stream =>
      Stream.value(VaccinationRecordDetailsError('Error loading details'));

  @override
  VaccinationRecordDetailsState get state =>
      VaccinationRecordDetailsError('Error loading details');
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
  late MockVaccinationRecordDetailsCubitLoading mockCubitLoading;
  late MockVaccinationRecordDetailsCubitLoaded mockCubitLoaded;
  late MockVaccinationRecordDetailsCubitError mockCubitError;
  late MockLanguageCubit mockLanguageCubit;

  setUp(() {
    mockCubitLoading = MockVaccinationRecordDetailsCubitLoading();
    mockCubitLoaded = MockVaccinationRecordDetailsCubitLoaded();
    mockCubitError = MockVaccinationRecordDetailsCubitError();
    mockLanguageCubit = MockLanguageCubit();

    // Setup stubs for loadVaccinationDetails
    when(
      () => mockCubitLoading.loadVaccinationDetails(any()),
    ).thenAnswer((_) async {});
    when(
      () => mockCubitLoaded.loadVaccinationDetails(any()),
    ).thenAnswer((_) async {});
    when(
      () => mockCubitError.loadVaccinationDetails(any()),
    ).thenAnswer((_) async {});
  });

  Widget createWidgetUnderTest({
    required VaccinationRecordDetailsCubit cubit,
    int vaccineId = 1,
  }) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: MultiBlocProvider(
        providers: [
          BlocProvider<VaccinationRecordDetailsCubit>.value(value: cubit),
          BlocProvider<LanguageCubit>.value(value: mockLanguageCubit),
        ],
        child: VaccinationDetailsPage(vaccineId: vaccineId),
      ),
    );
  }

  group('VaccinationDetailsPage Widget Tests', () {
    testWidgets('VaccinationDetailsPage renders correctly', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(VaccinationDetailsPage), findsOneWidget);
    });

    testWidgets('Page title is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('Your Vaccinations'), findsOneWidget);
    });

    testWidgets('Shows loading indicator when loading', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoading));
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsWidgets);
    });

    testWidgets('Shows error message when error occurs', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitError));
      await tester.pumpAndSettle();

      expect(find.text('Error loading vaccination details.'), findsWidgets);
    });

    testWidgets('Shows vaccine name when loaded', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.text('BCG'), findsWidgets);
    });

    testWidgets(
      'Shows vaccine description in English when language is English',
      (WidgetTester tester) async {
        await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
        await tester.pumpAndSettle();

        expect(find.text('Tuberculosis vaccine'), findsOneWidget);
      },
    );

    testWidgets('Shows vaccine icon', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.vaccines_rounded), findsOneWidget);
    });

    testWidgets('SingleChildScrollView is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitLoaded));
      await tester.pumpAndSettle();

      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });
  });
}
