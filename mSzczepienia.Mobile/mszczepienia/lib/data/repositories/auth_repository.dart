import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:mszczepienia/core/di/dependency_injection.dart';
import 'package:mszczepienia/core/notifications/notification_service.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/data/repositories/language_repository.dart';

class AuthRepository {
  final FirebaseAuth firebaseAuth;
  final FlutterSecureStorage storage;
  final CitizenRepository citizenRepository;

  static const _userKey = 'user_data';

  AuthRepository(this.firebaseAuth, this.storage, this.citizenRepository);

  // Check if user is already logged in
  Future<CitizenDetailsModel?> getCurrentUser() async {
    try {
      final jsonString = await storage.read(key: _userKey);

      if (jsonString != null) {
        final languageCode = await sl<LanguageRepository>()
            .getCurrentLanguage();
        await sl<NotificationService>().uploadFcmToken(languageCode);
        return CitizenDetailsModel.fromJsonString(jsonString);
      }
    } catch (e) {
      // If there is an error delete whatever left
      await storage.delete(key: _userKey);
    }

    return null;
  }

  // LOG IN
  Future<CitizenDetailsModel?> login(String email, String password) async {
    // FIREBASE LOGIN
    try {
      await firebaseAuth.signInWithEmailAndPassword(
        email: email,
        password: password,
      );
    } on FirebaseAuthException catch (e) {
      if (e.code == 'user-not-found') {
        throw Exception('USER_NOT_FOUND');
      } else {
        throw Exception('INVALID_CREDENTIALS');
      }
    } catch (e) {
      throw Exception('INVALID_CREDENTIALS');
    }

    // DATABASE LOGIN
    try {
      final CitizenDetailsModel citizenDetails = await citizenRepository
          .getCitizenDetails();

      await storage.write(key: _userKey, value: citizenDetails.toJsonString());

      final languageCode = await sl<LanguageRepository>().getCurrentLanguage();
      await sl<NotificationService>().uploadFcmToken(languageCode);
      return citizenDetails;
    } catch (e) {
      await firebaseAuth.signOut();
      throw Exception('Firebase login succeed but failed to load user data');
    }
  }

  // SIGN UP
  Future<CitizenDetailsModel?> signUp(
    String pesel,
    String name,
    String surname,
    String dateOfBirth,
    String defaultMedicalFacility,
    String email,
    String password,
  ) async {
    // FIREBASE SIGN UP
    try {
      await firebaseAuth.createUserWithEmailAndPassword(
        email: email,
        password: password,
      );
    } catch (e) {
      throw Exception('INVALID_CREDENTIALS');
    }

    // DATABASE SIGN UP - POST data to db about new user
    try {
      CitizenDetailsModel newCitizen = CitizenDetailsModel(
        name: name,
        surname: surname,
        email: email,
        dateOfBirth: DateTime.parse(dateOfBirth),
        creationDate: DateTime.now(),
        pesel: pesel,
        facilityName: defaultMedicalFacility,
        submissionStatus: 'NotUploaded',
      );
      await citizenRepository.createCitizenDetails(newCitizen);

      // Refresh token to include custom claims (role) set by backend
      await firebaseAuth.currentUser?.getIdToken(true);

      // Save to secure storage
      await storage.write(key: _userKey, value: newCitizen.toJsonString());

      final languageCode = await sl<LanguageRepository>().getCurrentLanguage();
      await sl<NotificationService>().uploadFcmToken(languageCode);
      return newCitizen;
    } catch (e) {
      // If failed to register citizen, delete firebase user
      await firebaseAuth.currentUser?.delete();
      throw Exception(
        'Firebase sign up succeed but failed to register user in database',
      );
    }
  }

  // LOG OUT
  Future<void> logout() async {
    firebaseAuth.signOut();

    // Delete user from secure storage
    await storage.delete(key: _userKey);
  }
}
