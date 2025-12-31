import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:dio/dio.dart';

@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) async {}

class NotificationService {
  final FirebaseMessaging _firebaseMessaging = FirebaseMessaging.instance;
  final Dio dio;

  NotificationService(this.dio);

  Future<void> initListeners() async {
    // Set up background message handler
    FirebaseMessaging.onBackgroundMessage(firebaseMessagingBackgroundHandler);

    // Notification request
    await _firebaseMessaging.requestPermission(
      alert: true,
      badge: true,
      sound: true,
    );
    setupTokenRefreshListener('en');
  }

  Future<void> uploadFcmToken(String languageCode) async {
    final token = await _firebaseMessaging.getToken();
    if (token != null) {
      await sendTokenToBackend(token, languageCode);
    }
  }

  Future<void> sendTokenToBackend(String token, String languageCode) async {
    await dio.post(
      '/citizens/device-token',
      data: {'deviceToken': token, 'language': languageCode},
    );
  }

  // Listen for token refresh
  void setupTokenRefreshListener(String languageCode) {
    _firebaseMessaging.onTokenRefresh.listen((newToken) {
      sendTokenToBackend(newToken, languageCode);
    });
  }
}
