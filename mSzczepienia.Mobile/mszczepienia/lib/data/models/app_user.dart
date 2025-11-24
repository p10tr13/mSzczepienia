import 'dart:convert';

class AppUser {
  final String email;

  const AppUser({
    required this.email,
  });

  Map<String, dynamic> toMap() {
    return {
      'email': email,
    };
  }

  factory AppUser.fromMap(Map<String, dynamic> map) {
    return AppUser(
      email: map['email'] ?? '',
    );
  }

  String toJson() => json.encode(toMap());

  factory AppUser.fromJson(String source) => AppUser.fromMap(json.decode(source));
}