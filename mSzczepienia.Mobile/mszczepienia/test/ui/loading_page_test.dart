import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/pages/loading_page.dart';

void main() {
  Widget createWidgetUnderTest({required String message}) {
    return MaterialApp(
      home: Scaffold(body: LoadingPage(message: message)),
    );
  }

  group('LoadingPage Widget Tests', () {
    testWidgets('LoadingPage renders correctly', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(message: 'Loading...'));
      await tester.pump();

      expect(find.byType(LoadingPage), findsOneWidget);
    });

    testWidgets('Displays provided message', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(message: 'Test message'));
      await tester.pump();

      expect(find.text('Test message'), findsOneWidget);
    });

    testWidgets('CircularProgressIndicator is present', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(createWidgetUnderTest(message: 'Loading...'));
      await tester.pump();

      expect(find.byType(CircularProgressIndicator), findsOneWidget);
    });

    testWidgets('Center widget is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(message: 'Loading...'));
      await tester.pump();

      expect(find.byType(Center), findsOneWidget);
    });

    testWidgets('Displays custom analyzing message', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(
          message:
              'Analyzing your photos... The process may take up to 5 minutes.',
        ),
      );
      await tester.pump();

      expect(find.textContaining('Analyzing'), findsOneWidget);
    });

    testWidgets('Displays custom sending message', (WidgetTester tester) async {
      await tester.pumpWidget(
        createWidgetUnderTest(
          message: 'Sending your submission for verification...',
        ),
      );
      await tester.pump();

      expect(find.textContaining('Sending'), findsOneWidget);
    });

    testWidgets('Displays custom cancelling message', (
      WidgetTester tester,
    ) async {
      await tester.pumpWidget(
        createWidgetUnderTest(message: 'Cancelling your submission...'),
      );
      await tester.pump();

      expect(find.textContaining('Cancelling'), findsOneWidget);
    });

    testWidgets('Column widget is present', (WidgetTester tester) async {
      await tester.pumpWidget(createWidgetUnderTest(message: 'Loading...'));
      await tester.pump();

      expect(find.byType(Column), findsOneWidget);
    });
  });
}
