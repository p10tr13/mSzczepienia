import 'dart:io';

class OcrSubmissionModel {
  final List<File> images;

  OcrSubmissionModel({required this.images});

  factory OcrSubmissionModel.fromJson(Map<String, dynamic> json) {
    return OcrSubmissionModel(
      images: (json['images'] as List<dynamic>)
          .map((path) => File(path as String))
          .toList(),
    );
  }

  Map<String, dynamic> toJson() {
    return {'images': images.map((file) => file.path).toList()};
  }
}
