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
    
    try {
      await firebaseAuth.signInWithEmailAndPassword(
        email: email, 
        password: password
      );
    } catch (e) {
      throw Exception('Incorrect credentials');
    }
    
    // Save new user to class AppUser
    final loggedUser = AppUser(
      email: email,
    );

    // Save to secure storage
    await _storage.write(key: _userKey, value: loggedUser.toJson());

    return loggedUser;
  }


  // LOG OUT
  Future<void> logout() async {

    firebaseAuth.signOut();
    
    // Delete user from secure storage
    await _storage.delete(key: _userKey);
  }
}