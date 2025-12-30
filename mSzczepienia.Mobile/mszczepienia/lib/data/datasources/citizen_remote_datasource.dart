import 'package:dio/dio.dart';
import 'package:mszczepienia/data/models/citizen_details_model.dart';
import 'package:mszczepienia/data/models/citizen_submission_status.dart';

class CitizenRemoteDataSource {
  final Dio dio;

  CitizenRemoteDataSource(this.dio);

  Future<CitizenDetailsModel> fetchCitizenDetails() async {
    final response = await dio.get('/citizens/citizen-details');

    return CitizenDetailsModel.fromJson(response.data);
  }

  Future<void> createCitizenDetails(CitizenDetailsModel citizenDetails) async {
    await dio.post(
      '/citizens/new-citizen-details',
      data: citizenDetails.toJson(),
    );
  }

  Future<CitizenSubmissionStatus> fetchCitizenSubmissionStatus() async {
    final response = await dio.get('/citizens/submission-status');

    return CitizenSubmissionStatus.fromJson(response.data);
  }
}
