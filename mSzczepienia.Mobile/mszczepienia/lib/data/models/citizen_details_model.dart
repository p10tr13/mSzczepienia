import 'dart:convert';

class CitizenDetailsModel {
  final String name;
  final String surname;
  final String email;
  final DateTime dateOfBirth;
  final DateTime creationDate;
  final String pesel;
  final String facilityName;
  final String submissionStatus;

  CitizenDetailsModel({
    required this.name,
    required this.surname,
    required this.email,
    required this.dateOfBirth,
    required this.creationDate,
    required this.pesel,
    required this.facilityName,
    required this.submissionStatus,
  });

  factory CitizenDetailsModel.fromJson(Map<String, dynamic> json) {
    return CitizenDetailsModel(
      name: json['name'] as String,
      surname: json['surname'] as String,
      email: json['email'] as String,
      dateOfBirth: DateTime.parse(json['dateOfBirth']),
      creationDate: DateTime.parse(json['creationDate']),
      pesel: json['pesel'] as String,
      facilityName: json['facilityName'] as String,
      submissionStatus: json['submissionStatus'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'name': name,
      'surname': surname,
      'email': email,
      'dateOfBirth': dateOfBirth.toIso8601String(),
      'creationDate': creationDate.toIso8601String(),
      'pesel': pesel,
      'facilityName': facilityName,
      'submissionStatus': submissionStatus,
    };
  }

  String toJsonString() => json.encode(toJson());

  factory CitizenDetailsModel.fromJsonString(String source) =>
      CitizenDetailsModel.fromJson(json.decode(source));
}
