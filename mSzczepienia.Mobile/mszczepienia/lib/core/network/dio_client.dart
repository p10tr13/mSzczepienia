import 'package:dio/dio.dart';
import 'package:firebase_auth/firebase_auth.dart';

class DioClient {
  final Dio _dio;

  DioClient()
    : _dio = Dio(
        BaseOptions(
          // For testing
          // baseUrl: 'http://10.0.2.2:8080/api',
          // For deployment
          // WITH VM
          // baseUrl: 'http://74.161.152.171:8080/api',
          // connectTimeout: const Duration(seconds: 10),
          // receiveTimeout: const Duration(seconds: 10),
          // headers: {'Content-Type': 'application/json'},
          baseUrl: 'https://mszczepienia-app.salmonbay-ca24e90b.switzerlandnorth.azurecontainerapps.io/api',
          connectTimeout: const Duration(seconds: 10),
          receiveTimeout: const Duration(seconds: 10),
          headers: {'Content-Type': 'application/json'},
        ),
      ) {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          // 1. Get current user
          final user = FirebaseAuth.instance.currentUser;

          if (user != null) {
            // 2. Get user token
            final token = await user.getIdToken();

            // 3. Add token as an option
            options.headers['Authorization'] = 'Bearer $token';
          }

          return handler.next(options);
        },
        onError: (DioException e, handler) {
          return handler.next(e);
        },
      ),
    );

    _dio.interceptors.add(
      LogInterceptor(requestBody: true, responseBody: true),
    );
  }

  Dio get instance => _dio;
}
