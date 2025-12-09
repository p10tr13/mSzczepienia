class MedicalFacilityModel {
  final int facilityId;
  final String facilityName;

  MedicalFacilityModel({required this.facilityId, required this.facilityName});

  factory MedicalFacilityModel.fromJson(Map<String, dynamic> json) {
    return MedicalFacilityModel(
      facilityId: json['facilityId'] as int,
      facilityName: json['facilityName'] as String,
    );
  }
}
