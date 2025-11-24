import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  
  @override
  Widget build(BuildContext context) {

    final AuthStatus authStatus =  context.read<AuthCubit>().state.status;

    return Layout(
      child: SingleChildScrollView(
        child: Center(
          child: Column(
            children: [
              const SizedBox(height: 40),
              Text(
                'Logged in', 
                style: TextStyle(
                  fontSize: 28,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 20,),
              Text(
                context.read<AuthCubit>().state.user!.email,
                style: TextStyle(
                  fontSize: 18,
                ),
              ),
              const SizedBox(height: 20,),
              ElevatedButton(
                onPressed: () {
                  context.read<AuthCubit>().logoutRequested();
                }, 
                child: authStatus == AuthStatus.loading ? CircularProgressIndicator(color: Colors.black,) : Text(
                  'Log out',
                  style: TextStyle(color: Colors.black),
                )
              ),
            ],
          ),
        ),
      )
    );
  }
}