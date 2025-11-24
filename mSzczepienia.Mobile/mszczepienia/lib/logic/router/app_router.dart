import 'package:go_router/go_router.dart';

class AppRouter {
  late final GoRouter router;
}

// class AppRouter {
//   // Potrzebujemy dostępu do AuthProvider, aby decydować o przekierowaniach
//   //final AuthProvider authProvider;

//   //AppRouter(this.authProvider);

//   late final GoRouter router = GoRouter(
//     // refreshListenable: To klucz! Router automatycznie odświeży się (uruchomi redirect),
//     // gdy AuthProvider powiadomi o zmianach (np. po login() lub logout()).
//     refreshListenable: authProvider, 
    
//     initialLocation: '/home', // Gdzie chcemy trafić domyślnie
    
//     // Mapa tras
//     routes: [
//       GoRoute(
//         path: '/login',
//         name: 'login',
//         builder: (context, state) => const LoginScreen(),
//       ),
//       GoRoute(
//         path: '/home',
//         name: 'home',
//         builder: (context, state) => const HomeScreen(),
//       ),
//     ],

//     // LOGIKA PRZEKIEROWAŃ (GUARD)
//     // Ta funkcja uruchamia się przy każdej zmianie nawigacji lub zmianie w authProvider
//     redirect: (BuildContext context, GoRouterState state) {
//       final bool loggedIn = authProvider.isLoggedIn;
//       final bool isLoggingIn = state.matchedLocation == '/login';

//       // Sytuacja 1: Użytkownik NIE jest zalogowany, a próbuje wejść gdzieś indziej niż login
//       if (!loggedIn && !isLoggingIn) {
//         return '/login';
//       }

//       // Sytuacja 2: Użytkownik JEST zalogowany, ale jest na ekranie logowania
//       // (chcemy go przerzucić od razu do Home)
//       if (loggedIn && isLoggingIn) {
//         return '/home';
//       }

//       // Brak przekierowania (pozwól wejść tam, gdzie chciał)
//       return null;
//     },
//   );
// }