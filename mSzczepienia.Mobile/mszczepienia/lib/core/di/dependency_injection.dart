import 'package:dio/dio.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:get_it/get_it.dart';
import 'package:mszczepienia/core/network/dio_client.dart';
import 'package:mszczepienia/core/notifications/notification_service.dart';
import 'package:mszczepienia/data/datasources/citizen_remote_datasource.dart';
import 'package:mszczepienia/data/datasources/medical_facility_remote_datasource.dart';
import 'package:mszczepienia/data/datasources/ocr_submission_remote_datasource.dart';
import 'package:mszczepienia/data/datasources/vaccination_record_remote_datasource.dart';
import 'package:mszczepienia/data/datasources/vaccine_remote_datasource.dart';
import 'package:mszczepienia/data/repositories/citizen_repository.dart';
import 'package:mszczepienia/data/repositories/medical_facility_repository.dart';
import 'package:mszczepienia/data/repositories/ocr_submission_repository.dart';
import 'package:mszczepienia/data/repositories/vaccination_record_repository.dart';
import 'package:mszczepienia/data/repositories/vaccine_repository.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/data/repositories/auth_repository.dart';
import 'package:mszczepienia/logic/citizen/citizen_cubit.dart';
import 'package:mszczepienia/logic/facility/facilities_cubit.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/data/repositories/language_repository.dart';
import 'package:mszczepienia/logic/ocr_submission/ocr_submission_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/data/repositories/theme_repository.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record_details/vaccination_record_details_cubit.dart';
import 'package:mszczepienia/logic/vaccine/vaccines_cubit.dart';

final sl = GetIt.instance;

Future<void> init() async {
  // --- EXTERNAL ---
  sl.registerLazySingleton<Dio>(() => DioClient().instance);

  // --- DATA SOURCES ---
  sl.registerLazySingleton(() => CitizenRemoteDataSource(sl()));
  sl.registerLazySingleton(() => MedicalFacilityRemoteDatasource(sl()));
  sl.registerLazySingleton(() => VaccinationRecordRemoteDatasource(sl()));
  sl.registerLazySingleton(() => OcrSubmissionRemoteDatasource(sl()));
  sl.registerLazySingleton(() => NotificationService(sl()));
  sl.registerLazySingleton(() => VaccineRemoteDatasource(sl()));

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
  sl.registerLazySingleton(() => VaccinationRecordRepository(sl()));
  sl.registerLazySingleton(() => OcrSubmissionRepository(sl()));
  sl.registerLazySingleton(() => VaccineRepository(sl()));

  // --- CUBITS ---
  sl.registerFactory(() => LanguageCubit(sl()));
  sl.registerFactory(() => ThemeCubit(sl()));
  sl.registerFactory(() => AuthCubit(sl()));
  sl.registerFactory(() => FacilitiesCubit(sl()));
  sl.registerFactory(() => VaccinationRecordsCubit(sl()));
  sl.registerFactory(() => VaccinationRecordDetailsCubit(sl()));
  sl.registerFactory(() => OcrSubmissionCubit(sl()));
  sl.registerFactory(() => CitizenCubit(sl()));
  sl.registerFactory(() => VaccinesCubit(sl()));
}
