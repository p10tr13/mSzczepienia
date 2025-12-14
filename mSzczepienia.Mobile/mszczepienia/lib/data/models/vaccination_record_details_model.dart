class VaccinationRecordDetailsModel {
  // Data about vaccine
  final int vaccineId;
  final String vaccineName;
  final String manufacturer;
  final String vaccineDescriptionPl;
  final String vaccineDescriptionEn;
  final DateTime creationDate;
  final List<String> diseasesPl;
  final List<String> diseasesEn;

  // Data about specific vaccination record
  final DateTime vaccinationDate;
  final DateTime? nextVaccinationDate;
  final String doseNumber;
  final String batch;
  final String? remarks;

  VaccinationRecordDetailsModel({
    required this.vaccineId,
    required this.vaccineName,
    required this.manufacturer,
    required this.vaccineDescriptionPl,
    required this.vaccineDescriptionEn,
    required this.creationDate,
    required this.diseasesPl,
    required this.diseasesEn,
    required this.vaccinationDate,
    this.nextVaccinationDate,
    required this.doseNumber,
    required this.batch,
    this.remarks,
  });

  factory VaccinationRecordDetailsModel.fromJson(Map<String, dynamic> json) {
    return VaccinationRecordDetailsModel(
      vaccineId: json['vaccineId'] as int,
      vaccineName: json['vaccineName'] as String,
      manufacturer: json['manufacturer'] as String,
      vaccineDescriptionPl: json['vaccineDescriptionPl'] as String,
      vaccineDescriptionEn: json['vaccineDescriptionEn'] as String,
      creationDate: DateTime.parse(json['creationDate']),
      diseasesPl: List<String>.from(json['diseasesPl'] as List<dynamic>),
      diseasesEn: List<String>.from(json['diseasesEn'] as List<dynamic>),
      vaccinationDate: DateTime.parse(json['vaccinationDate']),
      nextVaccinationDate: json['nextVaccinationDate'] != null
          ? DateTime.parse(json['nextVaccinationDate'])
          : null,
      doseNumber: json['doseNumber'] as String,
      batch: json['batch'] as String,
      remarks: json['remarks'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': vaccineId,
      'vaccineName': vaccineName,
      'manufacturer': manufacturer,
      'vaccineDescriptionPl': vaccineDescriptionPl,
      'vaccineDescriptionEn': vaccineDescriptionEn,
      'creationDate': creationDate.toIso8601String(),
      'diseasePl': diseasesPl,
      'diseaseEn': diseasesEn,
      'vaccinationDate': vaccinationDate.toIso8601String(),
      'nextVaccinationDate': nextVaccinationDate?.toIso8601String(),
      'doseNumber': doseNumber,
      'batch': batch,
      'remarks': remarks,
    };
  }
}
