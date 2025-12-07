import 'dart:convert';

class AppUser {
  final String name;
  final String surname;
  final String email;
  final DateTime dateOfBirth;
  final DateTime creationDate;
  final String pesel;
  final String defaultMedicalFacility;

  const AppUser({
    required this.name,
    required this.surname,
    required this.email,
    required this.dateOfBirth,
    required this.creationDate,
    required this.pesel,
    required this.defaultMedicalFacility,
  });

  Map<String, dynamic> toMap() {
    return {
      'name': name,
      'surname': surname,
      'email': email,
      'dateOfBirth': dateOfBirth.toIso8601String(),
      'creationDate': creationDate.toIso8601String(),
      'pesel': pesel,
      'defaultMedicalFacility': defaultMedicalFacility,
    };
  }

  factory AppUser.fromMap(Map<String, dynamic> map) {
    return AppUser(
      name: map['name'] ?? '',
      surname: map['surname'] ?? '',
      email: map['email'] ?? '',
      pesel: map['pesel'] ?? '',
      dateOfBirth: DateTime.parse(map['dateOfBirth']),
      creationDate: DateTime.parse(map['creationDate']),
      defaultMedicalFacility: map['defaultMedicalFacility'] ?? '',
    );
  }

  String toJson() => json.encode(toMap());

  factory AppUser.fromJson(String source) => AppUser.fromMap(json.decode(source));
}