import 'package:dio/dio.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';
import 'package:mszczepienia/core/network/dio_client.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/datasources/medical_facility_remote_datasource.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/data/repositories/medical_facility_repository.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_repository.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_repository.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_repository.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // --- EXTERNAL ---
  sl.registerLazySingleton<Dio>(() => DioClient().instance);

  // --- DATA SOURCES ---
  sl.registerLazySingleton(() => CitizenRemoteDataSource(sl()));
  sl.registerLazySingleton(() => MedicalFacilityRemoteDatasource(sl()));

  // --- REPOSITORIES ---
  sl.registerLazySingleton(() => CitizenRepository(sl()));
  sl.registerLazySingleton(
    () => AuthRepository(
      FirebaseAuth.instance,
      const FlutterSecureStorage(),
      sl<CitizenRepository>(),
    ),
  );
  sl.registerLazySingleton(
    () => LanguageRepository(storage: const FlutterSecureStorage()),
  );
  sl.registerLazySingleton(
    () => ThemeRepository(storage: const FlutterSecureStorage()),
  );
  sl.registerLazySingleton(() => MedicalFacilityRepository(sl()));

  // --- CUBITS ---
  sl.registerFactory(() => LanguageCubit(sl()));
  sl.registerFactory(() => ThemeCubit(sl()));
  sl.registerFactory(() => AuthCubit(sl()));
  sl.registerFactory(() => CitizenCubit(sl()));
  sl.registerFactory(() => FacilitiesCubit(sl()));
}
