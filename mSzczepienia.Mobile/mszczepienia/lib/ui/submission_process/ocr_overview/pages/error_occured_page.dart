import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';

class ErrorOccuredPage extends StatelessWidget {
  const ErrorOccuredPage({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Padding(
      padding: const EdgeInsets.only(
        left: 48.0,
        right: 48.0,
        top: 48.0,
        bottom: 20.0,
      ),
      child: Center(
        child: Column(
          children: [
            Text(
              l10n.errorOccurred,
              style: TextStyle(fontSize: 16, color: AppColors.logoRed),
            ),
            const SizedBox(height: 32),
            SizedBox(
              width: double.infinity,
              height: 56,
              child: OutlinedButton(
                onPressed: () {
                  context.go('/');
                },
                style: OutlinedButton.styleFrom(
                  side: BorderSide(color: borderColor, width: 2.0),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(30),
                  ),
                ),
                child: const Text(
                  "Wróć do ekranu głównego",
                  style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
