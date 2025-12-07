import 'package:dio/dio.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';
import 'package:mszczepienia/core/network/dio_client.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_repository.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // --- EXTERNAL ---
  sl.registerLazySingleton<Dio>(() => DioClient().instance);

  // --- DATA SOURCES ---
  sl.registerLazySingleton(() => CitizenRemoteDataSource(sl()));

  // --- REPOSITORIES ---
  sl.registerLazySingleton(() => CitizenRepository(sl()));
sl.registerLazySingleton(() => AuthRepository(
    FirebaseAuth.instance,
    const FlutterSecureStorage(),
    sl<CitizenRepository>(),
  ));

  // --- CUBITS ---
  sl.registerFactory(() => CitizenCubit(sl()));
  sl.registerFactory(() => AuthCubit(sl()));
}