class VaccinationEntryModel {
  final String vaccineName;
  final String batchNumber;
  final DateTime vaccinationDate;

  VaccinationEntryModel({
    required this.vaccineName,
    required this.batchNumber,
    required this.vaccinationDate,
  });

  factory VaccinationEntryModel.fromJson(Map<String, dynamic> json) {
    return VaccinationEntryModel(
      vaccineName: json['vaccineName'] as String,
      batchNumber: json['batchNumber'] as String,
      vaccinationDate: DateTime.parse(json['vaccinationDate']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'vaccineName': vaccineName,
      'batchNumber': batchNumber,
      'vaccinationDate': vaccinationDate.toIso8601String(),
    };
  }
}
