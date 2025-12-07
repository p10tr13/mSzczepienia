import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:mszczepienia/data/internal/app_user.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';

class AuthRepository {
  final FirebaseAuth firebaseAuth;
  final FlutterSecureStorage storage;
  final CitizenRepository citizenRepository;

  static const _userKey = 'user_data';

  AuthRepository(
    this.firebaseAuth, 
    this.storage, 
    this.citizenRepository
  );

  // Check if user is already logged in
  Future<AppUser?> getCurrentUser() async {
    try {
      final jsonString = await storage.read(key: _userKey);
      
      if (jsonString != null) {
        return AppUser.fromJson(jsonString);
      }
    } catch (e) {
      // If there is an error delete whatever left
      await storage.delete(key: _userKey);
    }
    return null;
  }

  // LOG IN
  Future<AppUser?> login(String email, String password) async {

    // FIREBASE LOGIN
    try {
      await firebaseAuth.signInWithEmailAndPassword(
        email: email, 
        password: password
      );
    } catch (e) {
      throw Exception('Incorrect credentials');
    }

    // DATABASE LOGIN
    try {
      final CitizenDetailsModel citizenDetails = await citizenRepository.getCitizenDetails();

      final loggedUser = AppUser(
        name: citizenDetails.name,
        surname: citizenDetails.surname,
        email: email,
        pesel: citizenDetails.peselHash, 
        dateOfBirth: citizenDetails.dateOfBirth,
        creationDate: citizenDetails.creationDate,
        defaultMedicalFacility: citizenDetails.facilityName,
      );

      await storage.write(key: _userKey, value: loggedUser.toJson());

      return loggedUser;

    } catch (e) {
      await firebaseAuth.signOut();
      throw Exception('Firebase login succeed but failed to load user data');
    }
  }


  // SIGN UP
  Future<AppUser?> signUp(String pesel, String name, String surname, String dateOfBirth, String defaultMedicalFacility, String email, String password) async {

    
    // FIREBASE SIGN UP
    try {
      await firebaseAuth.createUserWithEmailAndPassword(
        email: email, 
        password: password
      );

    } catch (e) {
      throw Exception('Incorrect credentials');
    }

    // DATABASE SIGN UP - POST data to db about new user
    
    // Save new user to class AppUser
    final signedUpUser = AppUser(
      name: name, 
      surname: surname, 
      email: email,
      pesel: pesel,
      dateOfBirth: DateTime.parse(dateOfBirth),
      creationDate: DateTime.now(),
      defaultMedicalFacility: defaultMedicalFacility,
    );
    // Save to secure storage
    await storage.write(key: _userKey, value: signedUpUser.toJson());

    return signedUpUser;
  }


  // LOG OUT
  Future<void> logout() async {

    firebaseAuth.signOut();
    
    // Delete user from secure storage
    await storage.delete(key: _userKey);
  }
}