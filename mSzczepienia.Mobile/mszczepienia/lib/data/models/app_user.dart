import 'dart:convert';

class AppUser {
  final String name;
  final String surname;
  final String pesel;
  final String defaultMedicalFacility;
  final String email;

  const AppUser({
    required this.name, 
    required this.surname,  
    required this.pesel, 
    required this.email, 
    required this.defaultMedicalFacility,
  });

  Map<String, dynamic> toMap() {
    return {
      'name': name,
      'surname': surname,
      'pesel': pesel,
      'email': email,
      'defaultMedicalFacility': defaultMedicalFacility,
    };
  }

  factory AppUser.fromMap(Map<String, dynamic> map) {
    return AppUser(
      name: map['name'] ?? '',
      surname: map['surname'] ?? '',
      pesel: map['pesel'] ?? '',
      email: map['email'] ?? '',
      defaultMedicalFacility: map['defaultMedicalFacility'] ?? '',
    );
  }

  String toJson() => json.encode(toMap());

  factory AppUser.fromJson(String source) => AppUser.fromMap(json.decode(source));
}