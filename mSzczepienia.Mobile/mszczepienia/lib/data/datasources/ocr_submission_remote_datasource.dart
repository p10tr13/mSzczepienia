import 'package:dio/dio.dart';
import 'package:mszczepienia/data/models/ocr_submission_model.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';

class OcrSubmissionRemoteDatasource {
  final Dio dio;

  OcrSubmissionRemoteDatasource(this.dio);

  Future<List<VaccinationEntryModel>> processVaccinationBooklet(
    OcrSubmissionModel query,
  ) async {
    final formData = FormData();

    for (var file in query.images) {
      String fileName = file.path.split('/').last;

      formData.files.add(
        MapEntry(
          'files',
          await MultipartFile.fromFile(file.path, filename: fileName),
        ),
      );
    }

    final response = await dio.post(
      '/ocrsubmission/process',
      data: formData,
      options: Options(
        sendTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
      ),
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

    return items.map((json) => VaccinationEntryModel.fromJson(json)).toList();
  }

  Future<void> cancelOcrSubmission() async {
    await dio.delete('/ocrsubmission/delete');
  }

  Future<void> submitOcrSubmission(List<VaccinationEntryModel> entries) async {
    final List<Map<String, dynamic>> jsonEntries = entries
        .map((e) => e.toJson())
        .toList();

    await dio.post(
      '/ocrsubmission/submit',
      data: jsonEntries,
      options: Options(
        sendTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
      ),
    );
  }
}
