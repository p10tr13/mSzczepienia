import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/di/dependency_injection.dart';
import 'package:mszczepienia/core/di/dependency_injection.dart' as di;
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/router/app_router.dart';
import 'package:mszczepienia/core/theme/app_theme.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_cubit.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp();
  await di.init();

  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  MyApp({super.key});

  late final appRouter = AppRouter(authCubit: null);

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
        BlocProvider(create: (context) => sl<LanguageCubit>()..checkAppStart()),
        BlocProvider(create: (context) => sl<ThemeCubit>()..checkAppStart()),
        BlocProvider<FacilitiesCubit>(
          create: (context) => sl<FacilitiesCubit>(),
        ),
        BlocProvider<VaccinationRecordsCubit>(
          create: (context) => sl<VaccinationRecordsCubit>(),
        ),
        BlocProvider<VaccinationRecordDetailsCubit>(
          create: (context) => sl<VaccinationRecordDetailsCubit>(),
        ),
        BlocProvider<OcrSubmissionCubit>(
          create: (context) => sl<OcrSubmissionCubit>(),
        ),
      ],
      // Define auth state globally
      child: Builder(
        builder: (context) {
          appRouter.authCubit ??= context.read<AuthCubit>();

          // Define theme globally
          return BlocBuilder<ThemeCubit, ThemeState>(
            builder: (context, themeState) {
              final themeData = themeState is DarkTheme
                  ? AppTheme.darkTheme
                  : AppTheme.lightTheme;

              // Define locale globally
              return BlocBuilder<LanguageCubit, LanguageState>(
                builder: (context, languageState) {
                  final locale = languageState is LanguagePolish
                      ? const Locale('pl')
                      : const Locale('en');

                  return MaterialApp.router(
                    title: 'mSzczepienia',
                    locale: locale,
                    localizationsDelegates:
                        AppLocalizations.localizationsDelegates,
                    supportedLocales: AppLocalizations.supportedLocales,
                    theme: themeData,
                    routerConfig: appRouter.router,
                    debugShowCheckedModeBanner: false,
                  );
                },
              );
            },
          );
        },
      ),
    );
  }
}
