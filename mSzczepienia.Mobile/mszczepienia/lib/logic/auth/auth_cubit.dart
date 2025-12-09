import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/logic/auth/auth_repository.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';

class AuthCubit extends Cubit<AuthState> {
  final AuthRepository _authRepository;

  AuthCubit(this._authRepository) : super(AuthInitial()) {
    checkAppStart();
  }

  // CHECKING IF USER IS LOGGED IN
  Future<void> checkAppStart() async {
    emit(AuthLoading());
    try {
      final user = await _authRepository.getCurrentUser();

      if (user != null) {
        emit(Authenticated(user));
      } else {
        emit(Unauthenticated());
      }
    } catch (_) {
      emit(Unauthenticated());
    }
  }

  /// LOGIN
  Future<void> loginRequested(String email, String password) async {
    emit(AuthLoading());
    try {
      final user = await _authRepository.login(email, password);

      if (user != null) {
        emit(Authenticated(user));
      } else {
        emit(AuthError('Incorrect credentials'));
      }
    } catch (e) {
      emit(AuthError(e.toString()));
    }
  }

  /// SIGN UP
  Future<void> signUpRequested(
    String pesel,
    String name,
    String surname,
    String dateOfBirth,
    String defaultMedicalFacility,
    String email,
    String password,
  ) async {
    emit(AuthLoading());

    try {
      final user = await _authRepository.signUp(
        pesel,
        name,
        surname,
        dateOfBirth,
        defaultMedicalFacility,
        email,
        password,
      );

      if (user != null) {
        emit(Authenticated(user));
      } else {
        emit(AuthError('Sign up error'));
      }
    } catch (e) {
      emit(AuthError(e.toString()));
    }
  }

  /// LOGOUT
  Future<void> logoutRequested() async {
    emit(AuthLoading());

    try {
      await _authRepository.logout();
      emit(Unauthenticated());
    } catch (e) {
      emit(AuthError(e.toString()));
    }
  }
}
