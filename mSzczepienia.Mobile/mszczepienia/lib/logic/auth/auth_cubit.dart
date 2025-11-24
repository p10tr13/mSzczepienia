import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/logic/auth/auth_repository.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';


class AuthCubit extends Cubit<AuthState> {
  final AuthRepository _authRepository;

  AuthCubit(this._authRepository) : super(const AuthState()) {
    _checkAppStart();
  }

  // CHECKING IF USER IS LOGGED IN
  Future<void> _checkAppStart() async {
    
    emit(state.copyWith(status: AuthStatus.loading));

    try {
      final user = await _authRepository.getCurrentUser();

      if (user != null) {
        emit(state.copyWith(
          status: AuthStatus.loginSuccess,
          user: user,
        ));
      } else {
        emit(state.copyWith(status: AuthStatus.unauthenticated));
      }
    } catch (_) {
      emit(state.copyWith(status: AuthStatus.unauthenticated));
    }
  }

  /// LOGIN
  Future<void> loginRequested(String email, String password) async {
    
    emit(state.copyWith(status: AuthStatus.loading));

    try {
      final user = await _authRepository.login(email, password);

      // W TYM MIEJSCU POBRANIE DANYCH Z BAZ CITIZEN I ACCOUNT
      // MAPOWANIE NA USERA
      
      emit(state.copyWith(
        status: AuthStatus.loginSuccess,
        user: user,
      ));
    } catch (e) {
      emit(state.copyWith(status: AuthStatus.loginFailed));
    }
  }

  /// LOGOUT
  Future<void> logoutRequested() async {
    
    emit(state.copyWith(status: AuthStatus.loading));

    try {
      await _authRepository.logout();
      emit(const AuthState(status: AuthStatus.signOutSuccess, user: null));
    } catch (e) {
      emit(const AuthState(status: AuthStatus.signOutFailed, user: null));
    }
  }
}