import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/data/repositories/theme_repository.dart';

// Mock FlutterSecureStorage
class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

void main() {
  late MockFlutterSecureStorage mockStorage;
  late ThemeRepository repository;

  setUp(() {
    mockStorage = MockFlutterSecureStorage();
    repository = ThemeRepository(storage: mockStorage);
  });

  group('ThemeRepository', () {
    group('getCurrentTheme', () {
      test('returns stored theme when available', () async {
        when(
          () => mockStorage.read(key: 'chosen_theme'),
        ).thenAnswer((_) async => 'dark');

        final result = await repository.getCurrentTheme();

        expect(result, 'dark');
        verify(() => mockStorage.read(key: 'chosen_theme')).called(1);
      });

      test('returns light when no theme stored (null)', () async {
        when(
          () => mockStorage.read(key: 'chosen_theme'),
        ).thenAnswer((_) async => null);

        final result = await repository.getCurrentTheme();

        expect(result, 'light');
      });

      test('returns light and deletes key on error', () async {
        when(
          () => mockStorage.read(key: 'chosen_theme'),
        ).thenThrow(Exception('Storage error'));
        when(
          () => mockStorage.delete(key: 'chosen_theme'),
        ).thenAnswer((_) async {});

        final result = await repository.getCurrentTheme();

        expect(result, 'light');
        verify(() => mockStorage.delete(key: 'chosen_theme')).called(1);
      });
    });

    group('setCurrentTheme', () {
      test('saves dark theme to storage', () async {
        when(
          () => mockStorage.write(key: 'dark', value: 'dark'),
        ).thenAnswer((_) async {});

        await repository.setCurrentTheme('dark');

        verify(() => mockStorage.write(key: 'dark', value: 'dark')).called(1);
      });

      test('saves light theme to storage', () async {
        when(
          () => mockStorage.write(key: 'light', value: 'light'),
        ).thenAnswer((_) async {});

        await repository.setCurrentTheme('light');

        verify(() => mockStorage.write(key: 'light', value: 'light')).called(1);
      });

      test('throws exception on storage error', () async {
        when(
          () => mockStorage.write(key: 'dark', value: 'dark'),
        ).thenThrow(Exception('Write error'));

        expect(
          () => repository.setCurrentTheme('dark'),
          throwsA(
            isA<Exception>().having(
              (e) => e.toString(),
              'message',
              contains('Could not save theme preference'),
            ),
          ),
        );
      });
    });
  });
}
