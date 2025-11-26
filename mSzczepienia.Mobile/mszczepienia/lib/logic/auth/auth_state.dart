import 'package:equatable/equatable.dart';
import 'package:mszczepienia/data/models/app_user.dart';

enum AuthStatus {
  unauthenticated, 
  loginSuccess, 
  loginFailed, 
  signUpSuccess, 
  signUpFailed, 
  signOutSuccess,
  signOutFailed,
  loading,
}

class AuthState extends Equatable {
  final AuthStatus status;
  final AppUser? user;

  const AuthState({
    this.status = AuthStatus.unauthenticated,
    this.user,
  });

  AuthState copyWith({
    AuthStatus? status,
    AppUser? user,
  }) {
    return AuthState(
      status: status ?? this.status,
      user: user ?? this.user,
    );
  }

  @override
  List<Object?> get props => [status, user];
}