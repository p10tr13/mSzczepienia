import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/home/home_custom_container.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
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
              BlocBuilder<AuthCubit, AuthState>(
                builder: (context, state) {
                  return state is Authenticated
                      ? Text(
                          l10n.goodMorningUser(state.user.name),
                          style: TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.w800,
                          ),
                        )
                      : Text(
                          l10n.goodMorning,
                          style: TextStyle(
                            fontSize: 32,
                            fontWeight: FontWeight.w800,
                          ),
                        );
                },
              ),
              const SizedBox(height: 35),
              InkWell(
                onTap: () {
                  context.push('/submission-status');
                },
                borderRadius: BorderRadius.circular(16),
                child: HomeCustomContainer(
                  icon: Icons.camera_alt_rounded,
                  title: l10n.vaccinationBooklet,
                  subtitle: l10n.vaccinationBookletDescription,
                ),
              ),
              const SizedBox(height: 22),
              InkWell(
                onTap: () {
                  context.push('/history');
                },
                child: HomeCustomContainer(
                  icon: Icons.history_rounded,
                  title: l10n.vaccinationHistory,
                  subtitle: l10n.vaccinationHistoryDescription,
                ),
              ),
              const SizedBox(height: 22),
              InkWell(
                onTap: () {
                  context.push('/settings');
                },
                child: HomeCustomContainer(
                  icon: Icons.settings_rounded,
                  title: l10n.settings,
                  subtitle: l10n.settingsDescription,
                ),
              ),
              const SizedBox(height: 22),
              InkWell(
                onTap: () {
                  context.push('/account');
                },
                child: HomeCustomContainer(
                  icon: Icons.account_circle_rounded,
                  title: l10n.yourAccount,
                  subtitle: l10n.yourAccountDescription,
                ),
              ),
              const SizedBox(height: 22),
            ],
          ),
        ),
      ),
    );
  }
}
