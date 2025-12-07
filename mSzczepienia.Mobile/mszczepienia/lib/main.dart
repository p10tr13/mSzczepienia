import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/di/dependency_injection.dart';
import 'package:mszczepienia/core/di/dependency_injection.dart' as di;
import 'package:mszczepienia/core/router/app_router.dart';
import 'package:mszczepienia/core/theme/app_theme.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp();
  await di.init();
  
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

    return MultiBlocProvider(
      providers: [
        BlocProvider<AuthCubit>(
          create: (context) => sl<AuthCubit>()..checkAppStart(),
        ),
        BlocProvider<CitizenCubit>(
          create: (context) => sl<CitizenCubit>(), 
        ),
      ],
      child: Builder(
        builder: (context) {
          final appRouter = AppRouter(authCubit: context.read<AuthCubit>());
    
          return MaterialApp.router(
            title: 'mSzczepienia',
            theme: AppTheme.lightTheme,
            routerConfig: appRouter.router,
            debugShowCheckedModeBanner: false
          );
        }
      ),
    );
  }
}