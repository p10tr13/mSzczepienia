import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/router/go_router_refresh_stream.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/authorization/login_page.dart';
import 'package:mszczepienia/ui/authorization/sign_up_page.dart';
import 'package:mszczepienia/ui/home/home_page.dart';

class AppRouter {

  final AuthCubit authCubit;

  late final GoRouter router = GoRouter(
    initialLocation: '/login',
    refreshListenable: GoRouterRefreshStream(authCubit.stream), // odświeżanie automatyczne
    
    redirect: (context, state) {
      final authState = authCubit.state;
      final bool isLoggedIn = authState is Authenticated;
      final bool isAuth = state.uri.toString() == '/login' || state.uri.toString() == '/signup';

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
      GoRoute(
        path: '/',
        builder: (context, state) => const HomePage(),
      ),
      GoRoute(
        path: '/login',
        builder: (context, state) => LoginPage(),
      ),
      GoRoute(
        path: '/signup',
        builder: (context, state) => SignUpPage(),
      ),
    ],
  );

  AppRouter({required this.authCubit});
}