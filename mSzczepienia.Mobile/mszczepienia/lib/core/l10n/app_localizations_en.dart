// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get appTitle => 'mSzczepienia';

  @override
  String get goodMorning => 'Good morning!';

  @override
  String get goodAfternoon => 'Good afternoon';

  @override
  String get goodEvening => 'Good evening';

  @override
  String get logInTitle => 'Log in to the application';

  @override
  String get logInButton => 'Log In';

  @override
  String get signUpTitle => 'Sign up to the application';

  @override
  String get signUpButton => 'Sign Up';

  @override
  String get email => 'Email';

  @override
  String get enterEmail => 'Enter your email address';

  @override
  String get emailCondition => 'Email has to contain @';

  @override
  String get password => 'Password';

  @override
  String get enterPassword => 'Enter your password';

  @override
  String get passwordCondition => 'Password must be at least 6 characters long';

  @override
  String get name => 'Name';

  @override
  String get enterName => 'Enter your name';

  @override
  String get surname => 'Surname';

  @override
  String get enterSurname => 'Enter your surname';

  @override
  String get pesel => 'PESEL';

  @override
  String get enterPesel => 'Enter your PESEL number';

  @override
  String get peselCondition => 'PESEL must be exactly 11 digits long';

  @override
  String get dateOfBirth => 'Date of birth';

  @override
  String get enterDateOfBirth => 'Enter your date of birth (YYYY-MM-DD)';

  @override
  String get dateOfBirthCondition =>
      'Date of birth must be in the format YYYY-MM-DD';

  @override
  String get medicalFacility => 'Medical Facility';

  @override
  String get selectMedicalFacility => 'Select your medical facility';

  @override
  String get validMedicalFacilityCondition =>
      'Please select a valid medical facility from the list';

  @override
  String get repeatPassword => 'Repeat Password';

  @override
  String get enterRepeatPassword => 'Repeat your password';

  @override
  String get repeatPasswordCondition => 'Passwords do not match';

  @override
  String goodMorningUser(String userName) {
    return 'Good morning, $userName';
  }

  @override
  String get vaccinationBooklet => 'Vaccination Booklet';

  @override
  String get vaccinationBookletDescription => 'Add your vaccination booklet';

  @override
  String get vaccinationHistory => 'Vaccination History';

  @override
  String get vaccinationHistoryDescription => 'View your vaccination history';

  @override
  String get settings => 'Settings';

  @override
  String get settingsDescription => 'Change application settings';

  @override
  String get yourAccount => 'Your Account';

  @override
  String get yourAccountDescription => 'View your account details';

  @override
  String get theme => 'Theme';

  @override
  String get changeDarkMode => 'Change to dark mode';

  @override
  String get changeLightMode => 'Change to light mode';

  @override
  String get language => 'Language';

  @override
  String get changePolish => 'Zmień język na polski';

  @override
  String get changeEnglish => 'Change language to English';

  @override
  String get accountCreationDate => 'Account creation date';

  @override
  String get callSupport => 'Call Support';

  @override
  String get logOut => 'Log Out';
}
