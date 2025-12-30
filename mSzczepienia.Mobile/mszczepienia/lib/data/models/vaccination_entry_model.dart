class VaccinationEntryModel {
  final String vaccineName;
  final DateTime vaccinationDate;

  VaccinationEntryModel({
    required this.vaccineName,
    required this.vaccinationDate,
  });

  factory VaccinationEntryModel.fromJson(Map<String, dynamic> json) {
    return VaccinationEntryModel(
      vaccineName: json['vaccineName'] as String,
      vaccinationDate: DateTime.parse(json['vaccinationDate']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'vaccineName': vaccineName,
      'vaccinationDate': vaccinationDate.toIso8601String(),
    };
  }
}
