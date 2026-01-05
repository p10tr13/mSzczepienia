import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/router/go_router_refresh_stream.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/account/account_page.dart';
import 'package:mszczepienia/ui/authorization/pages/login_page.dart';
import 'package:mszczepienia/ui/authorization/pages/sign_up_page.dart';
import 'package:mszczepienia/ui/home/home_page.dart';
import 'package:mszczepienia/ui/settings/settings_page.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/analyze_page.dart';
import 'package:mszczepienia/ui/submission_process/submission_status_page.dart';
import 'package:mszczepienia/ui/submission_process/upload_photo_page.dart';
import 'package:mszczepienia/ui/vaccination_details/vaccination_details_page.dart';
import 'package:mszczepienia/ui/vaccination_history/vaccination_history_page.dart';

class AppRouter {
  AuthCubit? authCubit;

  late final GoRouter router = GoRouter(
    initialLocation: '/login',
    refreshListenable: GoRouterRefreshStream(
      authCubit!.stream,
    ), // odświeżanie automatyczne

    redirect: (context, state) {
      final authState = authCubit!.state;
      final bool isLoggedIn = authState is Authenticated;
      final bool isAuth =
          state.uri.toString() == '/login' || state.uri.toString() == '/signup';

      // LOGIKA PRZEKIEROWAŃ:

      // 1. Jeśli nie jest zalogowany i nie jest na stronie logowania/rejestracji -> idź do logowania
      if (!isLoggedIn && !isAuth) {
        return '/login';
      }

      // 2. Jeśli jest zalogowany i próbuje wejść na login/signup -> idź na stronę główną
      if (isLoggedIn && isAuth) {
        return '/';
      }

      return null;
    },

    routes: [
      GoRoute(path: '/', builder: (context, state) => const HomePage()),
      GoRoute(
        path: '/submission-status/analyze',
        builder: (context, state) => const AnalyzePage(),
      ),
      GoRoute(
        path: '/submission-status',
        builder: (context, state) => const SubmissionStatusPage(),
      ),
      GoRoute(
        path: '/upload-photo',
        builder: (context, state) {
          final imagePaths = state.uri.queryParameters['imagePaths'];
          return UploadPhotoPage(imagePaths: imagePaths ?? '');
        },
      ),
      GoRoute(
        path: '/history/details',
        builder: (context, state) {
          final vaccineId = state.uri.queryParameters['vaccineId'];
          return VaccinationDetailsPage(vaccineId: int.parse(vaccineId!));
        },
      ),
      GoRoute(
        path: '/history',
        builder: (context, state) => const VaccinationHistoryPage(),
      ),
      GoRoute(
        path: '/settings',
        builder: (context, state) => const SettingsPage(),
      ),
      GoRoute(
        path: '/account',
        builder: (context, state) => const AccountPage(),
      ),
      GoRoute(path: '/login', builder: (context, state) => LoginPage()),
      GoRoute(path: '/signup', builder: (context, state) => SignUpPage()),
    ],
  );

  AppRouter({this.authCubit});
}
