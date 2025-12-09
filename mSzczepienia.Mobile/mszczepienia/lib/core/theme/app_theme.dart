import 'package:flutter/material.dart';
import 'app_colors.dart';

class AppTheme {
  // Motyw Jasny
  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.light,

      // Definicja ColorScheme (serce kolorów w Material 3)
      colorScheme: const ColorScheme.light(
        primary: AppColors.defaultBlue,
        surface: AppColors.lightGrey,
        error: Colors.redAccent,
        onPrimary: Colors.white,
      ),
    );
  }

  // DARK THEME
  // static ThemeData get darkTheme {
  //   return ThemeData(
  //     useMaterial3: true,
  //     brightness: Brightness.dark,

  //     colorScheme: const ColorScheme.dark(
  //       primary: AppColors.emeraldGreen,
  //       surface: AppColors.darkGrey,
  //     ),

  //   );
  // }
}
