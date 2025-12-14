import 'package:dio/dio.dart';
import 'package:mszczepienia/data/models/vaccination_record_details_model.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/data/queries/vaccination_record_details_query.dart';
import 'package:mszczepienia/data/queries/vaccination_record_overview_query.dart';

class VaccinationRecordRemoteDatasource {
  final Dio dio;

  VaccinationRecordRemoteDatasource(this.dio);

  Future<List<VaccinationRecordOverviewModel>> fetchVaccinationHistory(
    VaccinationRecordOverviewQuery query,
  ) async {
    final response = await dio.get(
      '/vaccination-records/history',
      queryParameters: query.toJson(),
    );

    final data = response.data;

    List<dynamic> items;
    if (data is List) {
      items = data;
    } else if (data is Map<String, dynamic>) {
      items = data['items'] as List? ?? [];
    } else {
      items = [];
    }

    return items
        .map((json) => VaccinationRecordOverviewModel.fromJson(json))
        .toList();
  }

  Future<List<VaccinationRecordDetailsModel>>
  fetchVaccinationRecordsByVaccineId(
    VaccinationRecordDetailsQuery query,
  ) async {
    final response = await dio.get(
      '/vaccination-records/details',
      queryParameters: query.toJson(),
    );

    final data = response.data as List;

    return data
        .map((json) => VaccinationRecordDetailsModel.fromJson(json))
        .toList();
  }
}
