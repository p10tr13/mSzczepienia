import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:mszczepienia/data/models/app_user.dart';

class AuthRepository {

  final FirebaseAuth firebaseAuth = FirebaseAuth.instance;

  final _storage = const FlutterSecureStorage();
  
  static const _userKey = 'user_session_data';


  // Check if user is already logged in
  Future<AppUser?> getCurrentUser() async {
    try {
      final jsonString = await _storage.read(key: _userKey);
      
      if (jsonString != null) {
        return AppUser.fromJson(jsonString);
      }
    } catch (e) {
      // If there is an error delete whatever left
      await _storage.delete(key: _userKey);
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
    // TODO: Get data from db about user

    // Save new user to class AppUser
    final loggedUser = AppUser(
      email: email,
      name: '', surname: '', pesel: '', defaultMedicalFacility: '',
    );
    // Save to secure storage
    await _storage.write(key: _userKey, value: loggedUser.toJson());

    return loggedUser;
  }


  // SIGN UP
  Future<AppUser?> signUp(String pesel, String name, String surname, String defaultMedicalFacility, String email, String password) async {

    
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
      pesel: pesel,
      email: email,
      defaultMedicalFacility: defaultMedicalFacility,
    );
    // Save to secure storage
    await _storage.write(key: _userKey, value: signedUpUser.toJson());

    return signedUpUser;
  }


  // LOG OUT
  Future<void> logout() async {

    firebaseAuth.signOut();
    
    // Delete user from secure storage
    await _storage.delete(key: _userKey);
  }
}