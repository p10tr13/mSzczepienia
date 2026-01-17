import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class LanguageRepository {
  final FlutterSecureStorage storage;
  static const _locale = 'chosen_language';

  LanguageRepository({required this.storage});

  Future<String> getCurrentLanguage() async {
    try {
      final jsonString = await storage.read(key: _locale);

      if (jsonString != null) {
        return jsonString;
      } else {
        return 'en'; // default language
      }
    } catch (e) {
      // If there is an error delete whatever left
      await storage.delete(key: _locale);
      return 'en';
    }
  }

  Future<void> setCurrentLanguage(String locale) async {
    try {
      await storage.write(key: _locale, value: locale);
    } catch (e) {
      throw Exception("Could not save language preference");
    }
  }
}
