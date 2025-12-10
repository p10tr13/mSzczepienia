import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/language/language_cubit.dart';
import 'package:mszczepienia/logic/language/language_state.dart';
import 'package:mszczepienia/logic/theme/theme_cubit.dart';
import 'package:mszczepienia/logic/theme/theme_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/settings/settings_custom_container.dart';

class SettingsPage extends StatefulWidget {
  const SettingsPage({super.key});

  @override
  State<SettingsPage> createState() => _SettingsPageState();
}

class _SettingsPageState extends State<SettingsPage> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Layout(
      child: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.only(
            left: 24.0,
            right: 24.0,
            top: 8.0,
            bottom: 8.0,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                l10n.settings,
                style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
              ),
              const SizedBox(height: 35),
              BlocBuilder<ThemeCubit, ThemeState>(
                builder: (context, state) {
                  return InkWell(
                    onTap: () {
                      if (state is LightTheme) {
                        context.read<ThemeCubit>().switchToDark();
                      } else {
                        context.read<ThemeCubit>().switchToLight();
                      }
                    },
                    borderRadius: BorderRadius.circular(16),
                    child: BlocBuilder<ThemeCubit, ThemeState>(
                      builder: (context, state) {
                        return SettingsCustomContainer(
                          icon: Icons.color_lens_rounded,
                          title: l10n.theme,
                          subtitle: state is LightTheme
                              ? l10n.changeDarkMode
                              : l10n.changeLightMode,
                        );
                      },
                    ),
                  );
                },
              ),
              const SizedBox(height: 22),
              BlocBuilder<LanguageCubit, LanguageState>(
                builder: (context, state) {
                  return InkWell(
                    onTap: () {
                      if (state is LanguagePolish) {
                        context.read<LanguageCubit>().switchToEnglish();
                      } else {
                        context.read<LanguageCubit>().switchToPolish();
                      }
                    },
                    child: SettingsCustomContainer(
                      icon: Icons.pin_drop_rounded,
                      title: l10n.language,
                      subtitle: state is LanguageEnglish
                          ? l10n.changePolish
                          : l10n.changeEnglish,
                    ),
                  );
                },
              ),
              const SizedBox(height: 22),
            ],
          ),
        ),
      ),
    );
  }
}
