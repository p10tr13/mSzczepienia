class VaccinationRecordOverviewModel {
  final int id;
  final String vaccineName;
  final String manufacturer;
  final DateTime vaccinationDate;
  final int dosesNumber;

  VaccinationRecordOverviewModel({
    required this.id,
    required this.vaccineName,
    required this.manufacturer,
    required this.vaccinationDate,
    required this.dosesNumber,
  });

  factory VaccinationRecordOverviewModel.fromJson(Map<String, dynamic> json) {
    return VaccinationRecordOverviewModel(
      id: json['id'] as int,
      vaccineName: json['vaccineName'] as String,
      manufacturer: json['manufacturer'] as String,
      vaccinationDate: DateTime.parse(json['vaccinationDate']),
      dosesNumber: json['dosesNumber'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'vaccineName': vaccineName,
      'manufacturer': manufacturer,
      'vaccinationDate': vaccinationDate.toIso8601String(),
      'dosesNumber': dosesNumber,
    };
  }
}
