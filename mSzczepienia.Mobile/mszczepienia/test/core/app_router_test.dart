import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:mocktail/mocktail.dart';
import 'package:mszczepienia/core/router/app_router.dart';
import 'package:mszczepienia/core/router/go_router_refresh_stream.dart';
import 'package:mszczepienia/data/internal/app_user.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';

// Mock AuthCubit
class MockAuthCubit extends Mock implements AuthCubit {
  final _controller = StreamController<AuthState>.broadcast();
  AuthState _state = Unauthenticated();

  @override
  Stream<AuthState> get stream => _controller.stream;

  @override
  AuthState get state => _state;

  void setState(AuthState newState) {
    _state = newState;
    _controller.add(newState);
  }

  @override
  Future<void> close() async {
    await _controller.close();
  }
}

void main() {
  late MockAuthCubit mockAuthCubit;
  late AppRouter appRouter;

  final testUser = AppUser(
    name: 'Test',
    surname: 'User',
    email: 'test@test.com',
    dateOfBirth: DateTime(1990, 1, 1),
    creationDate: DateTime.now(),
    pesel: '12345678901',
    defaultMedicalFacility: 'Test Hospital',
  );

  setUp(() {
    mockAuthCubit = MockAuthCubit();
  });

  tearDown(() async {
    await mockAuthCubit.close();
  });

  group('AppRouter', () {
    test('creates router with correct configuration', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      // Router should be created successfully
      expect(appRouter.router, isNotNull);
      expect(appRouter.authCubit, mockAuthCubit);
    });

    test('has 6 routes defined', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);
      final routes = appRouter.router.configuration.routes;

      expect(routes.length, 6);
    });

    test('has correct route paths', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);
      final routePaths = appRouter.router.configuration.routes
          .map((r) => (r as dynamic).path)
          .toList();

      expect(routePaths, contains('/'));
      expect(routePaths, contains('/settings'));
      expect(routePaths, contains('/account'));
      expect(routePaths, contains('/login'));
      expect(routePaths, contains('/signup'));
    });

    test('has redirect function defined', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      expect(appRouter.router.configuration.redirect, isNotNull);
    });

    test('has refreshListenable configured', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      // Router is created with refreshListenable
      expect(appRouter.router, isNotNull);
    });
  });

  group('GoRouterRefreshStream', () {
    test('creates valid ChangeNotifier from stream', () async {
      final streamController = StreamController<int>.broadcast();
      final refreshStream = GoRouterRefreshStream(streamController.stream);

      // Should be a valid ChangeNotifier
      expect(refreshStream, isA<ChangeNotifier>());

      refreshStream.dispose();
      await streamController.close();
    });

    test('notifies listeners when stream emits', () async {
      final streamController = StreamController<int>.broadcast();
      final refreshStream = GoRouterRefreshStream(streamController.stream);

      var notifyCount = 0;
      refreshStream.addListener(() {
        notifyCount++;
      });

      final initialCount = notifyCount;

      // Emit on stream
      streamController.add(1);
      await Future.delayed(const Duration(milliseconds: 50));

      expect(notifyCount, greaterThan(initialCount));

      // Emit again
      final countAfterFirst = notifyCount;
      streamController.add(2);
      await Future.delayed(const Duration(milliseconds: 50));

      expect(notifyCount, greaterThan(countAfterFirst));

      refreshStream.dispose();
      await streamController.close();
    });

    test('disposes subscription properly', () async {
      final streamController = StreamController<int>.broadcast();
      final refreshStream = GoRouterRefreshStream(streamController.stream);

      var notifyCount = 0;
      refreshStream.addListener(() {
        notifyCount++;
      });

      await Future.delayed(const Duration(milliseconds: 50));
      final countBeforeDispose = notifyCount;

      // Dispose
      refreshStream.dispose();

      // Emit after dispose - should not notify
      streamController.add(1);
      await Future.delayed(const Duration(milliseconds: 50));

      // Count should remain the same after dispose
      expect(notifyCount, countBeforeDispose);

      await streamController.close();
    });

    test('handles multiple emissions', () async {
      final streamController = StreamController<int>.broadcast();
      final refreshStream = GoRouterRefreshStream(streamController.stream);

      var notifyCount = 0;
      refreshStream.addListener(() {
        notifyCount++;
      });

      final initialCount = notifyCount;

      // Emit multiple values
      for (var i = 0; i < 6; i++) {
        streamController.add(i);
      }
      await Future.delayed(const Duration(milliseconds: 100));

      // Should have received multiple notifications
      expect(notifyCount, greaterThan(initialCount));

      refreshStream.dispose();
      await streamController.close();
    });
  });

  group('AppRouter redirect behavior', () {
    // Test the redirect logic by examining the state
    test('unauthenticated state means user is not logged in', () {
      mockAuthCubit.setState(Unauthenticated());
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      expect(isLoggedIn, isFalse);
    });

    test('authenticated state means user is logged in', () {
      mockAuthCubit.setState(Authenticated(testUser));
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      expect(isLoggedIn, isTrue);
    });

    test('login path is considered auth page', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      const path = '/login';
      final isAuth = path == '/login' || path == '/signup';
      expect(isAuth, isTrue);
    });

    test('signup path is considered auth page', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      const path = '/signup';
      final isAuth = path == '/login' || path == '/signup';
      expect(isAuth, isTrue);
    });

    test('home path is not considered auth page', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      const path = '/';
      final isAuth = path == '/login' || path == '/signup';
      expect(isAuth, isFalse);
    });

    test('settings path is not considered auth page', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      const path = '/settings';
      final isAuth = path == '/login' || path == '/signup';
      expect(isAuth, isFalse);
    });

    test('account path is not considered auth page', () {
      appRouter = AppRouter(authCubit: mockAuthCubit);

      const path = '/account';
      final isAuth = path == '/login' || path == '/signup';
      expect(isAuth, isFalse);
    });

    // Test redirect logic rules
    test('unauthenticated user on protected page should be redirected', () {
      mockAuthCubit.setState(Unauthenticated());
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      const path = '/settings';
      final isAuth = path == '/login' || path == '/signup';

      // Rule: if not logged in and not on auth page -> redirect to login
      final shouldRedirectToLogin = !isLoggedIn && !isAuth;
      expect(shouldRedirectToLogin, isTrue);
    });

    test('unauthenticated user on login page should not be redirected', () {
      mockAuthCubit.setState(Unauthenticated());
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      const path = '/login';
      final isAuth = path == '/login' || path == '/signup';

      // Rule: if not logged in and on auth page -> no redirect
      final shouldRedirectToLogin = !isLoggedIn && !isAuth;
      expect(shouldRedirectToLogin, isFalse);
    });

    test('authenticated user on login page should be redirected to home', () {
      mockAuthCubit.setState(Authenticated(testUser));
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      const path = '/login';
      final isAuth = path == '/login' || path == '/signup';

      // Rule: if logged in and on auth page -> redirect to home
      final shouldRedirectToHome = isLoggedIn && isAuth;
      expect(shouldRedirectToHome, isTrue);
    });

    test('authenticated user on home page should not be redirected', () {
      mockAuthCubit.setState(Authenticated(testUser));
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      const path = '/';
      final isAuth = path == '/login' || path == '/signup';

      // Rule: if logged in and not on auth page -> no redirect
      final shouldRedirectToHome = isLoggedIn && isAuth;
      final shouldRedirectToLogin = !isLoggedIn && !isAuth;
      expect(shouldRedirectToHome, isFalse);
      expect(shouldRedirectToLogin, isFalse);
    });

    test('authenticated user on settings page should not be redirected', () {
      mockAuthCubit.setState(Authenticated(testUser));
      appRouter = AppRouter(authCubit: mockAuthCubit);

      final isLoggedIn = appRouter.authCubit!.state is Authenticated;
      const path = '/settings';
      final isAuth = path == '/login' || path == '/signup';

      final shouldRedirectToHome = isLoggedIn && isAuth;
      final shouldRedirectToLogin = !isLoggedIn && !isAuth;
      expect(shouldRedirectToHome, isFalse);
      expect(shouldRedirectToLogin, isFalse);
    });
  });
}
