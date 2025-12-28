import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:intl/intl.dart' as intl;

import 'app_localizations_en.dart';
import 'app_localizations_pl.dart';

// ignore_for_file: type=lint

/// Callers can lookup localized strings with an instance of AppLocalizations
/// returned by `AppLocalizations.of(context)`.
///
/// Applications need to include `AppLocalizations.delegate()` in their app's
/// `localizationDelegates` list, and the locales they support in the app's
/// `supportedLocales` list. For example:
///
/// ```dart
/// import 'l10n/app_localizations.dart';
///
/// return MaterialApp(
///   localizationsDelegates: AppLocalizations.localizationsDelegates,
///   supportedLocales: AppLocalizations.supportedLocales,
///   home: MyApplicationHome(),
/// );
/// ```
///
/// ## Update pubspec.yaml
///
/// Please make sure to update your pubspec.yaml to include the following
/// packages:
///
/// ```yaml
/// dependencies:
///   # Internationalization support.
///   flutter_localizations:
///     sdk: flutter
///   intl: any # Use the pinned version from flutter_localizations
///
///   # Rest of dependencies
/// ```
///
/// ## iOS Applications
///
/// iOS applications define key application metadata, including supported
/// locales, in an Info.plist file that is built into the application bundle.
/// To configure the locales supported by your app, you’ll need to edit this
/// file.
///
/// First, open your project’s ios/Runner.xcworkspace Xcode workspace file.
/// Then, in the Project Navigator, open the Info.plist file under the Runner
/// project’s Runner folder.
///
/// Next, select the Information Property List item, select Add Item from the
/// Editor menu, then select Localizations from the pop-up menu.
///
/// Select and expand the newly-created Localizations item then, for each
/// locale your application supports, add a new item and select the locale
/// you wish to add from the pop-up menu in the Value field. This list should
/// be consistent with the languages listed in the AppLocalizations.supportedLocales
/// property.
abstract class AppLocalizations {
  AppLocalizations(String locale)
    : localeName = intl.Intl.canonicalizedLocale(locale.toString());

  final String localeName;

  static AppLocalizations? of(BuildContext context) {
    return Localizations.of<AppLocalizations>(context, AppLocalizations);
  }

  static const LocalizationsDelegate<AppLocalizations> delegate =
      _AppLocalizationsDelegate();

  /// A list of this localizations delegate along with the default localizations
  /// delegates.
  ///
  /// Returns a list of localizations delegates containing this delegate along with
  /// GlobalMaterialLocalizations.delegate, GlobalCupertinoLocalizations.delegate,
  /// and GlobalWidgetsLocalizations.delegate.
  ///
  /// Additional delegates can be added by appending to this list in
  /// MaterialApp. This list does not have to be used at all if a custom list
  /// of delegates is preferred or required.
  static const List<LocalizationsDelegate<dynamic>> localizationsDelegates =
      <LocalizationsDelegate<dynamic>>[
        delegate,
        GlobalMaterialLocalizations.delegate,
        GlobalCupertinoLocalizations.delegate,
        GlobalWidgetsLocalizations.delegate,
      ];

  /// A list of this localizations delegate's supported locales.
  static const List<Locale> supportedLocales = <Locale>[
    Locale('en'),
    Locale('pl'),
  ];

  /// No description provided for @appTitle.
  ///
  /// In en, this message translates to:
  /// **'mSzczepienia'**
  String get appTitle;

  /// No description provided for @goodMorning.
  ///
  /// In en, this message translates to:
  /// **'Good morning!'**
  String get goodMorning;

  /// No description provided for @goodAfternoon.
  ///
  /// In en, this message translates to:
  /// **'Good afternoon'**
  String get goodAfternoon;

  /// No description provided for @goodEvening.
  ///
  /// In en, this message translates to:
  /// **'Good evening'**
  String get goodEvening;

  /// No description provided for @logInTitle.
  ///
  /// In en, this message translates to:
  /// **'Log in to the application'**
  String get logInTitle;

  /// No description provided for @logInButton.
  ///
  /// In en, this message translates to:
  /// **'Log In'**
  String get logInButton;

  /// No description provided for @signUpTitle.
  ///
  /// In en, this message translates to:
  /// **'Sign up to the application'**
  String get signUpTitle;

  /// No description provided for @signUpButton.
  ///
  /// In en, this message translates to:
  /// **'Sign Up'**
  String get signUpButton;

  /// No description provided for @email.
  ///
  /// In en, this message translates to:
  /// **'Email'**
  String get email;

  /// No description provided for @enterEmail.
  ///
  /// In en, this message translates to:
  /// **'Enter your email address'**
  String get enterEmail;

  /// No description provided for @emailCondition.
  ///
  /// In en, this message translates to:
  /// **'Email has to contain @'**
  String get emailCondition;

  /// No description provided for @password.
  ///
  /// In en, this message translates to:
  /// **'Password'**
  String get password;

  /// No description provided for @enterPassword.
  ///
  /// In en, this message translates to:
  /// **'Enter your password'**
  String get enterPassword;

  /// No description provided for @passwordCondition.
  ///
  /// In en, this message translates to:
  /// **'Password must be at least 6 characters long'**
  String get passwordCondition;

  /// No description provided for @name.
  ///
  /// In en, this message translates to:
  /// **'Name'**
  String get name;

  /// No description provided for @enterName.
  ///
  /// In en, this message translates to:
  /// **'Enter your name'**
  String get enterName;

  /// No description provided for @surname.
  ///
  /// In en, this message translates to:
  /// **'Surname'**
  String get surname;

  /// No description provided for @enterSurname.
  ///
  /// In en, this message translates to:
  /// **'Enter your surname'**
  String get enterSurname;

  /// No description provided for @pesel.
  ///
  /// In en, this message translates to:
  /// **'PESEL'**
  String get pesel;

  /// No description provided for @enterPesel.
  ///
  /// In en, this message translates to:
  /// **'Enter your PESEL number'**
  String get enterPesel;

  /// No description provided for @peselCondition.
  ///
  /// In en, this message translates to:
  /// **'PESEL must be exactly 11 digits long'**
  String get peselCondition;

  /// No description provided for @dateOfBirth.
  ///
  /// In en, this message translates to:
  /// **'Date of birth'**
  String get dateOfBirth;

  /// No description provided for @enterDateOfBirth.
  ///
  /// In en, this message translates to:
  /// **'Enter your date of birth (YYYY-MM-DD)'**
  String get enterDateOfBirth;

  /// No description provided for @dateOfBirthCondition.
  ///
  /// In en, this message translates to:
  /// **'Date of birth must be in the format YYYY-MM-DD'**
  String get dateOfBirthCondition;

  /// No description provided for @medicalFacility.
  ///
  /// In en, this message translates to:
  /// **'Medical Facility'**
  String get medicalFacility;

  /// No description provided for @selectMedicalFacility.
  ///
  /// In en, this message translates to:
  /// **'Select your medical facility'**
  String get selectMedicalFacility;

  /// No description provided for @validMedicalFacilityCondition.
  ///
  /// In en, this message translates to:
  /// **'Please select a valid medical facility from the list'**
  String get validMedicalFacilityCondition;

  /// No description provided for @repeatPassword.
  ///
  /// In en, this message translates to:
  /// **'Repeat Password'**
  String get repeatPassword;

  /// No description provided for @enterRepeatPassword.
  ///
  /// In en, this message translates to:
  /// **'Repeat your password'**
  String get enterRepeatPassword;

  /// No description provided for @repeatPasswordCondition.
  ///
  /// In en, this message translates to:
  /// **'Passwords do not match'**
  String get repeatPasswordCondition;

  /// No description provided for @incorrectCredentials.
  ///
  /// In en, this message translates to:
  /// **'Incorrect email or password'**
  String get incorrectCredentials;

  /// No description provided for @signUpError.
  ///
  /// In en, this message translates to:
  /// **'Sign up error'**
  String get signUpError;

  /// No description provided for @goodMorningUser.
  ///
  /// In en, this message translates to:
  /// **'Good morning, {userName}'**
  String goodMorningUser(String userName);

  /// No description provided for @vaccinationBooklet.
  ///
  /// In en, this message translates to:
  /// **'Vaccination Booklet'**
  String get vaccinationBooklet;

  /// No description provided for @vaccinationBookletDescription.
  ///
  /// In en, this message translates to:
  /// **'Add your vaccination booklet'**
  String get vaccinationBookletDescription;

  /// No description provided for @vaccinationHistory.
  ///
  /// In en, this message translates to:
  /// **'Vaccination History'**
  String get vaccinationHistory;

  /// No description provided for @vaccinationHistoryDescription.
  ///
  /// In en, this message translates to:
  /// **'View your vaccination history'**
  String get vaccinationHistoryDescription;

  /// No description provided for @settings.
  ///
  /// In en, this message translates to:
  /// **'Settings'**
  String get settings;

  /// No description provided for @settingsDescription.
  ///
  /// In en, this message translates to:
  /// **'Change application settings'**
  String get settingsDescription;

  /// No description provided for @yourAccount.
  ///
  /// In en, this message translates to:
  /// **'Your Account'**
  String get yourAccount;

  /// No description provided for @yourAccountDescription.
  ///
  /// In en, this message translates to:
  /// **'View your account details'**
  String get yourAccountDescription;

  /// No description provided for @theme.
  ///
  /// In en, this message translates to:
  /// **'Theme'**
  String get theme;

  /// No description provided for @changeDarkMode.
  ///
  /// In en, this message translates to:
  /// **'Change to dark mode'**
  String get changeDarkMode;

  /// No description provided for @changeLightMode.
  ///
  /// In en, this message translates to:
  /// **'Change to light mode'**
  String get changeLightMode;

  /// No description provided for @language.
  ///
  /// In en, this message translates to:
  /// **'Language'**
  String get language;

  /// No description provided for @changePolish.
  ///
  /// In en, this message translates to:
  /// **'Zmień język na polski'**
  String get changePolish;

  /// No description provided for @changeEnglish.
  ///
  /// In en, this message translates to:
  /// **'Change language to English'**
  String get changeEnglish;

  /// No description provided for @accountCreationDate.
  ///
  /// In en, this message translates to:
  /// **'Account creation date'**
  String get accountCreationDate;

  /// No description provided for @callSupport.
  ///
  /// In en, this message translates to:
  /// **'Call Support'**
  String get callSupport;

  /// No description provided for @logOut.
  ///
  /// In en, this message translates to:
  /// **'Log Out'**
  String get logOut;

  /// No description provided for @yourVaccinations.
  ///
  /// In en, this message translates to:
  /// **'Your Vaccinations'**
  String get yourVaccinations;

  /// No description provided for @sort.
  ///
  /// In en, this message translates to:
  /// **'Sort'**
  String get sort;

  /// No description provided for @alphabetical_az.
  ///
  /// In en, this message translates to:
  /// **'Alphabetical (A-Z)'**
  String get alphabetical_az;

  /// No description provided for @alphabetical_za.
  ///
  /// In en, this message translates to:
  /// **'Alphabetical (Z-A)'**
  String get alphabetical_za;

  /// No description provided for @date_newest.
  ///
  /// In en, this message translates to:
  /// **'Date (Newest first)'**
  String get date_newest;

  /// No description provided for @date_oldest.
  ///
  /// In en, this message translates to:
  /// **'Date (Oldest first)'**
  String get date_oldest;

  /// No description provided for @dose.
  ///
  /// In en, this message translates to:
  /// **'Dose'**
  String get dose;

  /// No description provided for @last.
  ///
  /// In en, this message translates to:
  /// **'Last:'**
  String get last;

  /// No description provided for @noOfDoses.
  ///
  /// In en, this message translates to:
  /// **'Doses:'**
  String get noOfDoses;

  /// No description provided for @noVaccinations.
  ///
  /// In en, this message translates to:
  /// **'No vaccinations found.'**
  String get noVaccinations;

  /// No description provided for @errorLoadingVaccinationRecords.
  ///
  /// In en, this message translates to:
  /// **'Error loading vaccination records.'**
  String get errorLoadingVaccinationRecords;

  /// No description provided for @errorLoadingVaccinationDetails.
  ///
  /// In en, this message translates to:
  /// **'Error loading vaccination details.'**
  String get errorLoadingVaccinationDetails;

  /// No description provided for @doseNumber.
  ///
  /// In en, this message translates to:
  /// **'Dose Number'**
  String get doseNumber;

  /// No description provided for @vaccinationDate.
  ///
  /// In en, this message translates to:
  /// **'Vaccination Date'**
  String get vaccinationDate;

  /// No description provided for @batchNo.
  ///
  /// In en, this message translates to:
  /// **'Batch Number'**
  String get batchNo;

  /// No description provided for @remarks.
  ///
  /// In en, this message translates to:
  /// **'Remarks'**
  String get remarks;

  /// No description provided for @aboutVaccine.
  ///
  /// In en, this message translates to:
  /// **'About this vaccine'**
  String get aboutVaccine;

  /// No description provided for @vaccineDescription.
  ///
  /// In en, this message translates to:
  /// **'Vaccine Description'**
  String get vaccineDescription;

  /// No description provided for @diseasesPrevented.
  ///
  /// In en, this message translates to:
  /// **'Diseases Prevented'**
  String get diseasesPrevented;

  /// No description provided for @vaccineCreationDate.
  ///
  /// In en, this message translates to:
  /// **'Vaccine Creation Date'**
  String get vaccineCreationDate;

  /// No description provided for @producer.
  ///
  /// In en, this message translates to:
  /// **'Manufacturer'**
  String get producer;

  /// No description provided for @nextVaccination.
  ///
  /// In en, this message translates to:
  /// **'Next Vaccination In'**
  String get nextVaccination;

  /// No description provided for @days.
  ///
  /// In en, this message translates to:
  /// **'days'**
  String get days;

  /// No description provided for @uploadNewPhoto.
  ///
  /// In en, this message translates to:
  /// **'Upload New Photo'**
  String get uploadNewPhoto;

  /// No description provided for @updateYourVaccinationHistory.
  ///
  /// In en, this message translates to:
  /// **'Update your Vaccination History with your Vaccination Booklet'**
  String get updateYourVaccinationHistory;

  /// No description provided for @submissionStatus.
  ///
  /// In en, this message translates to:
  /// **'Submission Status'**
  String get submissionStatus;

  /// No description provided for @statusNotUploaded.
  ///
  /// In en, this message translates to:
  /// **'Not Uploaded'**
  String get statusNotUploaded;

  /// No description provided for @statusPending.
  ///
  /// In en, this message translates to:
  /// **'Pending Verification'**
  String get statusPending;

  /// No description provided for @statusApproved.
  ///
  /// In en, this message translates to:
  /// **'Approved'**
  String get statusApproved;

  /// No description provided for @statusRejected.
  ///
  /// In en, this message translates to:
  /// **'Rejected'**
  String get statusRejected;

  /// No description provided for @selectPhotoSource.
  ///
  /// In en, this message translates to:
  /// **'Select Photo Source'**
  String get selectPhotoSource;

  /// No description provided for @takePhoto.
  ///
  /// In en, this message translates to:
  /// **'Take Photo'**
  String get takePhoto;

  /// No description provided for @chooseFromGallery.
  ///
  /// In en, this message translates to:
  /// **'Choose from Gallery'**
  String get chooseFromGallery;

  /// No description provided for @cancel.
  ///
  /// In en, this message translates to:
  /// **'Cancel'**
  String get cancel;

  /// No description provided for @analize.
  ///
  /// In en, this message translates to:
  /// **'Analyze'**
  String get analize;

  /// No description provided for @analyzingSubmission.
  ///
  /// In en, this message translates to:
  /// **'Analyzing your photos...'**
  String get analyzingSubmission;

  /// No description provided for @errorOccurred.
  ///
  /// In en, this message translates to:
  /// **'An error occurred'**
  String get errorOccurred;
}

class _AppLocalizationsDelegate
    extends LocalizationsDelegate<AppLocalizations> {
  const _AppLocalizationsDelegate();

  @override
  Future<AppLocalizations> load(Locale locale) {
    return SynchronousFuture<AppLocalizations>(lookupAppLocalizations(locale));
  }

  @override
  bool isSupported(Locale locale) =>
      <String>['en', 'pl'].contains(locale.languageCode);

  @override
  bool shouldReload(_AppLocalizationsDelegate old) => false;
}

AppLocalizations lookupAppLocalizations(Locale locale) {
  // Lookup logic when only language code is specified.
  switch (locale.languageCode) {
    case 'en':
      return AppLocalizationsEn();
    case 'pl':
      return AppLocalizationsPl();
  }

  throw FlutterError(
    'AppLocalizations.delegate failed to load unsupported locale "$locale". This is likely '
    'an issue with the localizations generation tool. Please file an issue '
    'on GitHub with a reproducible sample app and the gen-l10n configuration '
    'that was used.',
  );
}
