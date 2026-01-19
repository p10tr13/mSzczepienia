import 'package:flutter/material.dart';
import 'app_colors.dart';

class AppTheme {
  // LIGHT THEME
  static ThemeData get lightTheme {
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: AppColors.defaultBlue,
        brightness: Brightness.light,
      ),
    );
  }

  // DARK THEME
  static ThemeData get darkTheme {
    return ThemeData(
      useMaterial3: true,
      colorScheme: ColorScheme.fromSeed(
        seedColor: AppColors.defaultBlue,
        brightness: Brightness.dark,
      ),
    );
  }
}
