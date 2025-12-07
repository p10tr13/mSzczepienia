class CitizenDetailsModel {
  final String name;
  final String surname;
  final DateTime dateOfBirth;
  final DateTime creationDate;
  final String peselHash;
  final String facilityName;
  final String submissionStatus;

  CitizenDetailsModel({
    required this.name,
    required this.surname,
    required this.dateOfBirth,
    required this.creationDate,
    required this.peselHash,
    required this.facilityName,
    required this.submissionStatus,
  });

  factory CitizenDetailsModel.fromJson(Map<String, dynamic> json) {
    return CitizenDetailsModel(
      name: json['name'] as String,
      surname: json['surname'] as String,
      dateOfBirth: DateTime.parse(json['dateOfBirth']),
      creationDate: DateTime.parse(json['creationDate']),
      peselHash: json['peselHash'] as String,
      facilityName: json['facilityName'] as String,
      submissionStatus: json['submissionStatus'] as String,
    );
  }
}