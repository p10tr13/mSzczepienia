import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/home/home_page.dart';
import 'package:mszczepienia/ui/login/login_page.dart';

class AuthGate extends StatelessWidget {
  const AuthGate({super.key});

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, state) {        
        if (state.status == AuthStatus.loginSuccess || state.status == AuthStatus.signInSuccess) {
          return HomePage();
        }

        return LoginPage();
      },
    );
  }
}