import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/data/repositories/theme_repository.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';

class ThemeCubit extends Cubit<ThemeState> {
  final ThemeRepository themeRepository;

  ThemeCubit(this.themeRepository) : super(LightTheme()) {
    checkAppStart();
  }

  Future<void> checkAppStart() async {
    try {
      final locale = await themeRepository.getCurrentTheme();

      if (locale == 'dark') {
        emit(DarkTheme());
      } else {
        emit(LightTheme());
      }
    } catch (_) {
      emit(LightTheme());
    }
  }

  void switchToDark() {
    emit(DarkTheme());
    themeRepository.setCurrentTheme('dark');
  }

  void switchToLight() {
    emit(LightTheme());
    themeRepository.setCurrentTheme('light');
  }
}
