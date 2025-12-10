import 'package:equatable/equatable.dart';

abstract class LanguageState extends Equatable {
  const LanguageState();

  @override
  List<Object?> get props => [];
}

class LanguagePolish extends LanguageState {}

class LanguageEnglish extends LanguageState {}
