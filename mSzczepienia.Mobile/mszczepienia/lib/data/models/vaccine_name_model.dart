class VaccineNameModel {
  final int vaccineId;
  final String vaccineName;

  VaccineNameModel({required this.vaccineId, required this.vaccineName});
  factory VaccineNameModel.fromJson(Map<String, dynamic> json) {
    return VaccineNameModel(
      vaccineId: json['vaccineId'] as int,
      vaccineName: json['vaccineName'] as String,
    );
  }
}
