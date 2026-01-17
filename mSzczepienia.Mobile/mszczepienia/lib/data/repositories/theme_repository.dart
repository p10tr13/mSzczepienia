import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ThemeRepository {
  final FlutterSecureStorage storage;
  static const locale = 'chosen_theme';

  ThemeRepository({required this.storage});

  Future<String> getCurrentTheme() async {
    try {
      final jsonString = await storage.read(key: locale);

      if (jsonString != null) {
        return jsonString;
      } else {
        return 'light'; // default theme
      }
    } catch (e) {
      // If there is an error delete whatever left
      await storage.delete(key: locale);
      return 'light';
    }
  }

  Future<void> setCurrentTheme(String locale) async {
    try {
      await storage.write(key: locale, value: locale);
    } catch (e) {
      throw Exception("Could not save theme preference");
    }
  }
}
