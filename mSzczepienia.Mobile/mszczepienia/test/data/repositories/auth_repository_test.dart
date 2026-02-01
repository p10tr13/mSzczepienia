import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:get_it/get_it.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/notifications/notification_service.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/data/repositories/auth_repository.dart';
import 'package:mszczepienia/data/repositories/language_repository.dart';

// Mocks
class MockFirebaseAuth extends Mock implements FirebaseAuth {}

class MockUserCredential extends Mock implements UserCredential {}

class MockUser extends Mock implements User {}

class MockFlutterSecureStorage extends Mock implements FlutterSecureStorage {}

class MockCitizenRepository extends Mock implements CitizenRepository {}

class MockLanguageRepository extends Mock implements LanguageRepository {}

class MockNotificationService extends Mock implements NotificationService {}

void main() {
  late AuthRepository authRepository;
  late MockFirebaseAuth mockFirebaseAuth;
  late MockFlutterSecureStorage mockStorage;
  late MockCitizenRepository mockCitizenRepository;
  late MockLanguageRepository mockLanguageRepository;
  late MockNotificationService mockNotificationService;

  setUpAll(() {
    // Register fallback values for mocktail
    registerFallbackValue(
      CitizenDetailsModel(
        name: 'Test',
        surname: 'User',
        email: 'test@example.com',
        dateOfBirth: DateTime(1990, 1, 1),
        creationDate: DateTime(2024, 1, 1),
        pesel: '00000000000',
        facilityName: 'Test Facility',
        submissionStatus: '',
      ),
    );
  });

  setUp(() {
    mockFirebaseAuth = MockFirebaseAuth();
    mockStorage = MockFlutterSecureStorage();
    mockCitizenRepository = MockCitizenRepository();
    mockLanguageRepository = MockLanguageRepository();
    mockNotificationService = MockNotificationService();

    // Register mocks in GetIt
    final sl = GetIt.instance;
    if (sl.isRegistered<LanguageRepository>()) {
      sl.unregister<LanguageRepository>();
    }
    if (sl.isRegistered<NotificationService>()) {
      sl.unregister<NotificationService>();
    }
    sl.registerSingleton<LanguageRepository>(mockLanguageRepository);
    sl.registerSingleton<NotificationService>(mockNotificationService);

    // Setup default stubs for GetIt dependencies
    when(
      () => mockLanguageRepository.getCurrentLanguage(),
    ).thenAnswer((_) async => 'en');
    when(
      () => mockNotificationService.uploadFcmToken(any()),
    ).thenAnswer((_) async {});

    // Setup default stub for storage delete (used in error handling)
    when(
      () => mockStorage.delete(key: any(named: 'key')),
    ).thenAnswer((_) async {});

    authRepository = AuthRepository(
      mockFirebaseAuth,
      mockStorage,
      mockCitizenRepository,
    );
  });

  tearDown(() {
    final sl = GetIt.instance;
    if (sl.isRegistered<LanguageRepository>()) {
      sl.unregister<LanguageRepository>();
    }
    if (sl.isRegistered<NotificationService>()) {
      sl.unregister<NotificationService>();
    }
  });

  group('AuthRepository - getCurrentUser', () {
    test('returns AppUser when user data exists in storage', () async {
      // Arrange
      const userJson =
          '{"name":"John","surname":"Doe","email":"john@example.com","pesel":"12345678901","dateOfBirth":"1990-01-15T00:00:00.000","creationDate":"2024-01-01T00:00:00.000","facilityName":"Test Hospital","submissionStatus":"approved"}';

      when(
        () => mockStorage.read(key: 'user_data'),
      ).thenAnswer((_) async => userJson);

      // Act
      final result = await authRepository.getCurrentUser();

      // Assert
      expect(result, isNotNull);
      expect(result!.name, equals('John'));
      expect(result.surname, equals('Doe'));
      expect(result.email, equals('john@example.com'));
      expect(result.pesel, equals('12345678901'));
      verify(() => mockStorage.read(key: 'user_data')).called(1);
    });

    test('returns null when no user data in storage', () async {
      // Arrange
      when(
        () => mockStorage.read(key: 'user_data'),
      ).thenAnswer((_) async => null);

      // Act
      final result = await authRepository.getCurrentUser();

      // Assert
      expect(result, isNull);
    });

    test('returns null and deletes storage on error', () async {
      // Arrange
      when(
        () => mockStorage.read(key: 'user_data'),
      ).thenThrow(Exception('Storage error'));
      when(() => mockStorage.delete(key: 'user_data')).thenAnswer((_) async {});

      // Act
      final result = await authRepository.getCurrentUser();

      // Assert
      expect(result, isNull);
      verify(() => mockStorage.delete(key: 'user_data')).called(1);
    });
  });

  group('AuthRepository - login', () {
    final testCitizenDetails = CitizenDetailsModel(
      name: 'John',
      surname: 'Doe',
      email: 'john@example.com',
      dateOfBirth: DateTime(1990, 1, 15),
      creationDate: DateTime(2024, 1, 1),
      pesel: '12345678901',
      facilityName: 'Test Hospital',
      submissionStatus: 'approved',
    );

    test('returns AppUser on successful login', () async {
      // Arrange
      final mockUserCredential = MockUserCredential();

      when(
        () => mockFirebaseAuth.signInWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).thenAnswer((_) async => mockUserCredential);

      when(() => mockFirebaseAuth.signOut()).thenAnswer((_) async {});

      when(
        () => mockCitizenRepository.getCitizenDetails(),
      ).thenAnswer((_) async => testCitizenDetails);

      when(
        () => mockStorage.write(
          key: 'user_data',
          value: any(named: 'value'),
        ),
      ).thenAnswer((_) async {});

      // Act
      final result = await authRepository.login(
        'john@example.com',
        'password123',
      );

      // Assert
      expect(result, isNotNull);
      expect(result!.name, equals('John'));
      expect(result.surname, equals('Doe'));
      expect(result.email, equals('john@example.com'));
      expect(result.pesel, equals('12345678901'));
      expect(result.facilityName, equals('Test Hospital'));

      verify(
        () => mockFirebaseAuth.signInWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).called(1);
      verify(() => mockCitizenRepository.getCitizenDetails()).called(1);
      verify(
        () => mockStorage.write(
          key: 'user_data',
          value: any(named: 'value'),
        ),
      ).called(1);
    });

    test('throws exception on Firebase auth failure', () async {
      // Arrange
      when(
        () => mockFirebaseAuth.signInWithEmailAndPassword(
          email: 'john@example.com',
          password: 'wrongpassword',
        ),
      ).thenThrow(FakeFirebaseAuthException.fromCode('wrong-password'));

      // Act & Assert
      expect(
        () => authRepository.login('john@example.com', 'wrongpassword'),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('INVALID_CREDENTIALS'),
          ),
        ),
      );
    });

    test('throws USER_NOT_FOUND exception when user does not exist', () async {
      // Arrange
      when(
        () => mockFirebaseAuth.signInWithEmailAndPassword(
          email: 'unknown@example.com',
          password: 'password123',
        ),
      ).thenThrow(FakeFirebaseAuthException.fromCode('user-not-found'));

      // Act & Assert
      expect(
        () => authRepository.login('unknown@example.com', 'password123'),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('USER_NOT_FOUND'),
          ),
        ),
      );
    });

    test('throws exception and signs out on citizen details failure', () async {
      // Arrange
      final mockUserCredential = MockUserCredential();

      when(
        () => mockFirebaseAuth.signInWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).thenAnswer((_) async => mockUserCredential);

      when(
        () => mockCitizenRepository.getCitizenDetails(),
      ).thenThrow(Exception('API Error'));

      when(() => mockFirebaseAuth.signOut()).thenAnswer((_) async {});

      // Act & Assert
      expect(
        () => authRepository.login('john@example.com', 'password123'),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('failed to load user data'),
          ),
        ),
      );
    });
  });

  group('AuthRepository - signUp', () {
    test('returns AppUser on successful sign up', () async {
      // Arrange
      final mockUserCredential = MockUserCredential();
      final mockUser = MockUser();

      when(
        () => mockFirebaseAuth.createUserWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).thenAnswer((_) async => mockUserCredential);

      when(() => mockFirebaseAuth.currentUser).thenReturn(mockUser);
      when(() => mockUser.getIdToken(true)).thenAnswer((_) async => 'token');
      when(() => mockUser.delete()).thenAnswer((_) async {});

      when(
        () => mockCitizenRepository.createCitizenDetails(any()),
      ).thenAnswer((_) async {});

      when(
        () => mockStorage.write(
          key: 'user_data',
          value: any(named: 'value'),
        ),
      ).thenAnswer((_) async {});

      // Act
      final result = await authRepository.signUp(
        '12345678901',
        'John',
        'Doe',
        '1990-01-15',
        'Test Hospital',
        'john@example.com',
        'password123',
      );

      // Assert
      expect(result, isNotNull);
      expect(result!.name, equals('John'));
      expect(result.surname, equals('Doe'));
      expect(result.email, equals('john@example.com'));
      expect(result.pesel, equals('12345678901'));
      expect(result.facilityName, equals('Test Hospital'));

      verify(
        () => mockFirebaseAuth.createUserWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).called(1);
      verify(() => mockCitizenRepository.createCitizenDetails(any())).called(1);
      verify(
        () => mockStorage.write(
          key: 'user_data',
          value: any(named: 'value'),
        ),
      ).called(1);
    });

    test('throws exception on Firebase sign up failure', () async {
      // Arrange
      when(
        () => mockFirebaseAuth.createUserWithEmailAndPassword(
          email: 'john@example.com',
          password: 'password123',
        ),
      ).thenThrow(FakeFirebaseAuthException.fromCode('email-already-in-use'));

      // Act & Assert
      expect(
        () => authRepository.signUp(
          '12345678901',
          'John',
          'Doe',
          '1990-01-15',
          'Test Hospital',
          'john@example.com',
          'password123',
        ),
        throwsA(
          isA<Exception>().having(
            (e) => e.toString(),
            'message',
            contains('INVALID_CREDENTIALS'),
          ),
        ),
      );
    });
  });

  group('AuthRepository - logout', () {
    test('signs out and deletes user data from storage', () async {
      // Arrange
      when(() => mockFirebaseAuth.signOut()).thenAnswer((_) async {});
      when(() => mockStorage.delete(key: 'user_data')).thenAnswer((_) async {});

      // Act
      await authRepository.logout();

      // Assert
      verify(() => mockFirebaseAuth.signOut()).called(1);
      verify(() => mockStorage.delete(key: 'user_data')).called(1);
    });
  });
}

// Helper class for Firebase exceptions
class FakeFirebaseAuthException extends FirebaseAuthException {
  FakeFirebaseAuthException.fromCode(String code)
    : super(code: code, message: _getMessageFromCode(code));

  static String _getMessageFromCode(String code) {
    switch (code) {
      case 'wrong-password':
        return 'Wrong password provided.';
      case 'user-not-found':
        return 'No user found with this email.';
      case 'email-already-in-use':
        return 'Email is already in use.';
      default:
        return 'An unknown error occurred.';
    }
  }

  @override
  String toString() => 'FakeFirebaseAuthException: [$code] $message';
}
