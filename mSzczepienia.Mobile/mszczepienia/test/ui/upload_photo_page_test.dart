import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_state.dart';
import 'package:mszczepienia/ui/submission_process/upload_photo_page.dart';

// Mock OcrSubmissionCubit for initial state
class MockOcrSubmissionCubitInitial extends Mock implements OcrSubmissionCubit {
  @override
  Stream<OcrSubmissionState> get stream => Stream.value(OcrSubmissionInitial());

  @override
  OcrSubmissionState get state => OcrSubmissionInitial();
}

void main() {
  late MockOcrSubmissionCubitInitial mockCubitInitial;

  setUp(() {
    mockCubitInitial = MockOcrSubmissionCubitInitial();
  });

  Widget createWidgetUnderTest({
    required OcrSubmissionCubit cubit,
    String imagePaths = '',
  }) {
    return MaterialApp(
      locale: const Locale('en'),
      localizationsDelegates: AppLocalizations.localizationsDelegates,
      supportedLocales: AppLocalizations.supportedLocales,
      home: BlocProvider<OcrSubmissionCubit>.value(
        value: cubit,
        child: UploadPhotoPage(imagePaths: imagePaths),
      ),
    );
  }

  group('UploadPhotoPage Widget Tests', () {
    testWidgets('UploadPhotoPage renders correctly', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.byType(UploadPhotoPage), findsOneWidget);
    });

    testWidgets('Page title is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.text('Vaccination Booklet'), findsOneWidget);
    });

    testWidgets('Add photo button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.byIcon(Icons.add_a_photo_rounded), findsOneWidget);
    });

    testWidgets('Analyze button is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.text('Analyze'), findsOneWidget);
    });

    testWidgets('SingleChildScrollView is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.byType(SingleChildScrollView), findsOneWidget);
    });

    testWidgets('OutlinedButton for adding photo is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.byType(OutlinedButton), findsOneWidget);
    });

    testWidgets('ElevatedButton for analyze is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      expect(find.byType(ElevatedButton), findsOneWidget);
    });

    testWidgets('No images displayed when imagePaths is empty', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(cubit: mockCubitInitial));
      await tester.pumpAndSettle();

      // No Image.file widgets should be present when there are no images
      expect(find.byType(Image), findsNothing);
    });
  });
}
