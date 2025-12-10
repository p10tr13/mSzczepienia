// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Polish (`pl`).
class AppLocalizationsPl extends AppLocalizations {
  AppLocalizationsPl([String locale = 'pl']) : super(locale);

  @override
  String get appTitle => 'mSzczepienia';

  @override
  String get goodMorning => 'Dzień dobry!';

  @override
  String get goodAfternoon => 'Dzień dobry!';

  @override
  String get goodEvening => 'Dobry wieczór';

  @override
  String get logInTitle => 'Zaloguj się do aplikacji';

  @override
  String get logInButton => 'Zaloguj się';

  @override
  String get signUpTitle => 'Zarejestruj się w aplikacji';

  @override
  String get signUpButton => 'Zarejestruj się';

  @override
  String get email => 'Email';

  @override
  String get enterEmail => 'Wprowadź swój adres email';

  @override
  String get emailCondition => 'Email musi zawierać @';

  @override
  String get password => 'Hasło';

  @override
  String get enterPassword => 'Wprowadź swoje hasło';

  @override
  String get passwordCondition => 'Hasło musi mieć co najmniej 6 znaków';

  @override
  String get name => 'Imię';

  @override
  String get enterName => 'Wprowadź swoje imię';

  @override
  String get surname => 'Nazwisko';

  @override
  String get enterSurname => 'Wprowadź swoje nazwisko';

  @override
  String get pesel => 'PESEL';

  @override
  String get enterPesel => 'Wprowadź swój numer PESEL';

  @override
  String get peselCondition => 'PESEL musi mieć dokładnie 11 cyfr';

  @override
  String get dateOfBirth => 'Data urodzenia';

  @override
  String get enterDateOfBirth => 'Wprowadź swoją datę urodzenia (RRRR-MM-DD)';

  @override
  String get dateOfBirthCondition =>
      'Data urodzenia musi być w formacie RRRR-MM-DD';

  @override
  String get medicalFacility => 'Placówka medyczna';

  @override
  String get selectMedicalFacility => 'Wybierz swoją placówkę medyczną';

  @override
  String get validMedicalFacilityCondition =>
      'Wybierz prawidłową placówkę medyczną z listy';

  @override
  String get repeatPassword => 'Powtórz hasło';

  @override
  String get enterRepeatPassword => 'Powtórz swoje hasło';

  @override
  String get repeatPasswordCondition => 'Hasła nie są identyczne';

  @override
  String goodMorningUser(String userName) {
    return 'Dzień dobry, $userName';
  }

  @override
  String get vaccinationBooklet => 'Książeczka szczepień';

  @override
  String get vaccinationBookletDescription =>
      'Dodaj swoją książeczkę szczepień';

  @override
  String get vaccinationHistory => 'Historia szczepień';

  @override
  String get vaccinationHistoryDescription =>
      'Sprawdź swoją historię szczepień';

  @override
  String get settings => 'Ustawienia';

  @override
  String get settingsDescription => 'Zmień ustawienia aplikacji';

  @override
  String get yourAccount => 'Twoje konto';

  @override
  String get yourAccountDescription => 'Sprawdź szczegóły swojego konta';

  @override
  String get theme => 'Motyw';

  @override
  String get changeDarkMode => 'Zmień na tryb ciemny';

  @override
  String get changeLightMode => 'Zmień na tryb jasny';

  @override
  String get language => 'Język';

  @override
  String get changePolish => 'Zmień język na polski';

  @override
  String get changeEnglish => 'Change language to English';

  @override
  String get accountCreationDate => 'Data utworzenia konta';

  @override
  String get callSupport => 'Zadzwoń do wsparcia';

  @override
  String get logOut => 'Wyloguj się';
}
