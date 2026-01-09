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
  String get incorrectCredentials => 'Nieprawidłowy email lub hasło';

  @override
  String get signUpError => 'Błąd rejestracji';

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

  @override
  String get yourVaccinations => 'Twoje szczepienia';

  @override
  String get sort => 'Sortuj';

  @override
  String get alphabetical_az => 'Alfabetycznie (A-Z)';

  @override
  String get alphabetical_za => 'Alfabetycznie (Z-A)';

  @override
  String get date_newest => 'Data (od najnowszych)';

  @override
  String get date_oldest => 'Data (od najstarszych)';

  @override
  String get dose => 'Dawka';

  @override
  String get last => 'Ostatnia:';

  @override
  String get noOfDoses => 'Dawki:';

  @override
  String get noVaccinations => 'Brak znalezionych szczepień.';

  @override
  String get errorLoadingVaccinationRecords =>
      'Błąd podczas ładowania historii szczepień.';

  @override
  String get errorLoadingVaccinationDetails =>
      'Błąd podczas ładowania szczegółów szczepienia.';

  @override
  String get doseNumber => 'Numer dawki';

  @override
  String get vaccinationDate => 'Data szczepienia';

  @override
  String get batchNo => 'Numer serii';

  @override
  String get remarks => 'Uwagi';

  @override
  String get aboutVaccine => 'O tej szczepionce';

  @override
  String get vaccineDescription => 'Opis szczepionki';

  @override
  String get diseasesPrevented => 'Choroby, którym zapobiega';

  @override
  String get vaccineCreationDate => 'Data powstania szczepionki';

  @override
  String get producer => 'Producent';

  @override
  String get nextVaccination => 'Następne szczepienie za';

  @override
  String get days => 'dni';

  @override
  String get uploadNewPhoto => 'Prześlij nowe zdjęcie';

  @override
  String get updateYourVaccinationHistory =>
      'Zaktualizuj swoją historię szczepień za pomocą książeczki szczepień';

  @override
  String get submissionStatus => 'Status zgłoszenia';

  @override
  String get statusNotUploaded => 'Nie przesłano';

  @override
  String get statusPending => 'Oczekuje na weryfikację';

  @override
  String get statusApproved => 'Zatwierdzono';

  @override
  String get statusRejected => 'Odrzucono';

  @override
  String get selectPhotoSource => 'Wybierz źródło zdjęcia';

  @override
  String get takePhoto => 'Zrób zdjęcie';

  @override
  String get chooseFromGallery => 'Wybierz z galerii';

  @override
  String get cancel => 'Anuluj';

  @override
  String get analize => 'Analizuj';

  @override
  String get analyzingSubmission => 'Analizowanie twoich zdjęć...';

  @override
  String get errorOccurred => 'Wystąpił błąd';

  @override
  String get ocrDescription =>
      'Należy pamiętać, że proces OCR może nie być w 100% dokładny. Dokładnie sprawdź wyodrębnione dane przed przesłaniem.';

  @override
  String get addAnotherVaccination => 'Dodaj szczepienie';

  @override
  String get cancelSubmission => 'Anuluj zgłoszenie';

  @override
  String get submitForVerification => 'Prześlij do weryfikacji';

  @override
  String get modifyEntry => 'Modyfikuj wpis';

  @override
  String get saveChanges => 'Zapisz zmiany';

  @override
  String get batchCondition => 'Numer serii nie może być dłuższy niż 20 znaków';

  @override
  String get dateOfVacccinationCondition =>
      'Data szczepienia musi być w formacie RRRR-MM-DD';

  @override
  String get vaccineName => 'Nazwa szczepionki';

  @override
  String get vaccineNameCondition =>
      'Proszę wybrać prawidłową szczepionkę z listy';

  @override
  String get submit => 'Prześlij';

  @override
  String get deleteEntry => 'Usuń ten wpis';

  @override
  String get deleteEntryDescription =>
      'Czy na pewno chcesz usunąć ten wpis szczepienia?';

  @override
  String get delete => 'Usuń';

  @override
  String get cancelLoading => 'Anulowanie twojego zgłoszenia...';

  @override
  String get sendingSubmission =>
      'Wysyłanie twojego zgłoszenia do weryfikacji...';
}
