class VaccinationRecordOverviewModel {
  final int id;
  final String vaccineName;
  final String shortDesciriptionEn;
  final String shortDescriptionPl;
  final DateTime vaccinationDate;
  final int dosesNumber;

  VaccinationRecordOverviewModel({
    required this.id,
    required this.vaccineName,
    required this.shortDesciriptionEn,
    required this.shortDescriptionPl,
    required this.vaccinationDate,
    required this.dosesNumber,
  });

  factory VaccinationRecordOverviewModel.fromJson(Map<String, dynamic> json) {
    return VaccinationRecordOverviewModel(
      id: json['id'] as int,
      vaccineName: json['vaccineName'] as String,
      shortDesciriptionEn: json['shortDescriptionEn'] as String,
      shortDescriptionPl: json['shortDescriptionPl'] as String,
      vaccinationDate: DateTime.parse(json['vaccinationDate']),
      dosesNumber: json['dosesNumber'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'vaccineName': vaccineName,
      'shortDescriptionEn': shortDesciriptionEn,
      'shortDescriptionPl': shortDescriptionPl,
      'vaccinationDate': vaccinationDate.toIso8601String(),
      'dosesNumber': dosesNumber,
    };
  }
}
