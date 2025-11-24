import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/theme/app_theme.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_gate.dart';
import 'package:mszczepienia/logic/auth/auth_repository.dart';
//import 'package:mszczepienia/logic/router/app_router.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp();
  
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
    ]);

    return RepositoryProvider(
      create: (context) => AuthRepository(),
      child: BlocProvider(
        create: (context) => AuthCubit(context.read<AuthRepository>()),
        child: MaterialApp(
          title: 'mSzczepienia',
          theme: AppTheme.lightTheme,
          
          home: const AuthGate(), 
          
          routes: {
          },
        ),
      ),
    );
  }
}