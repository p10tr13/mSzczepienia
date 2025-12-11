import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/logic/language/language_repository.dart';

// Mock FlutterSecureStorage
class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

void main() {
  late MockFlutterSecureStorage mockStorage;
  late LanguageRepository repository;

  setUp(() {
    mockStorage = MockFlutterSecureStorage();
    repository = LanguageRepository(storage: mockStorage);
  });

  group('LanguageRepository', () {
    group('getCurrentLanguage', () {
      test('returns stored language when available', () async {
        when(() => mockStorage.read(key: 'chosen_language'))
            .thenAnswer((_) async => 'pl');

        final result = await repository.getCurrentLanguage();

        expect(result, 'pl');
        verify(() => mockStorage.read(key: 'chosen_language')).called(1);
      });

      test('returns en when no language stored (null)', () async {
        when(() => mockStorage.read(key: 'chosen_language'))
            .thenAnswer((_) async => null);

        final result = await repository.getCurrentLanguage();

        expect(result, 'en');
      });

      test('returns en and deletes key on error', () async {
        when(() => mockStorage.read(key: 'chosen_language'))
            .thenThrow(Exception('Storage error'));
        when(() => mockStorage.delete(key: 'chosen_language'))
            .thenAnswer((_) async {});

        final result = await repository.getCurrentLanguage();

        expect(result, 'en');
        verify(() => mockStorage.delete(key: 'chosen_language')).called(1);
      });
    });

    group('setCurrentLanguage', () {
      test('saves language to storage', () async {
        when(() => mockStorage.write(key: 'chosen_language', value: 'pl'))
            .thenAnswer((_) async {});

        await repository.setCurrentLanguage('pl');

        verify(() => mockStorage.write(key: 'chosen_language', value: 'pl'))
            .called(1);
      });

      test('saves English language to storage', () async {
        when(() => mockStorage.write(key: 'chosen_language', value: 'en'))
            .thenAnswer((_) async {});

        await repository.setCurrentLanguage('en');

        verify(() => mockStorage.write(key: 'chosen_language', value: 'en'))
            .called(1);
      });

      test('throws exception on storage error', () async {
        when(() => mockStorage.write(key: 'chosen_language', value: 'pl'))
            .thenThrow(Exception('Write error'));

        expect(
          () => repository.setCurrentLanguage('pl'),
          throwsA(isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('Could not save language preference'),
          )),
        );
      });
    });
  });
}
