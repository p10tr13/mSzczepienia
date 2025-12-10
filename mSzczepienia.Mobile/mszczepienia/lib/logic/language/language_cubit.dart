import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/logic/language/language_repository.dart';
import 'package:mszczepienia/logic/language/language_state.dart';

class LanguageCubit extends Cubit<LanguageState> {
  final LanguageRepository languageRepository;

  LanguageCubit(this.languageRepository) : super(LanguageEnglish()) {
    checkAppStart();
  }

  Future<void> checkAppStart() async {
    try {
      final locale = await languageRepository.getCurrentLanguage();

      if (locale == 'pl') {
        emit(LanguagePolish());
      } else {
        emit(LanguageEnglish());
      }
    } catch (_) {
      emit(LanguageEnglish());
    }
  }

  void switchToPolish() {
    emit(LanguagePolish());
    languageRepository.setCurrentLanguage('pl');
  }

  void switchToEnglish() {
    emit(LanguageEnglish());
    languageRepository.setCurrentLanguage('en');
  }
}
