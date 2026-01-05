import 'package:flutter/material.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';

class EntryTile extends StatelessWidget {
  const EntryTile({super.key, required this.entry});

  final VaccinationEntryModel entry;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Container(
      padding: const EdgeInsets.all(12),
      width: double.infinity,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: borderColor, width: 1.1),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  entry.vaccineName,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  entry.date == DateTime(1900, 1, 1)
                      ? '${l10n.vaccinationDate}: '
                      : '${l10n.vaccinationDate}: ${DatetimeFormatter().datetimeFormatter(entry.date)}',
                  style: const TextStyle(fontSize: 14),
                ),
                const SizedBox(height: 4),
                Text(
                  '${l10n.batchNo}: ${entry.batch}',
                  style: const TextStyle(fontSize: 14),
                ),
              ],
            ),
          ),
          IconButton(
            icon: const Icon(Icons.edit_outlined),
            onPressed: () {},
            tooltip: l10n.settings,
          ),
        ],
      ),
    );
  }
}
