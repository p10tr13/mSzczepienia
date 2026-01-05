import 'package:flutter/material.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/data/models/vaccination_entry_model.dart';
import 'package:mszczepienia/ui/submission_process/ocr_overview/entry_tile.dart';

class EntriesList extends StatefulWidget {
  const EntriesList({super.key, required this.entries});

  final List<VaccinationEntryModel> entries;

  @override
  State<EntriesList> createState() => _EntriesListState();
}

class _EntriesListState extends State<EntriesList> {
  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    if (widget.entries.isEmpty) {
      return Center(child: Text(l10n.noVaccinations));
    }

    return ListView.separated(
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      itemCount: widget.entries.length,
      separatorBuilder: (context, index) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final entry = widget.entries[index];
        return EntryTile(entry: entry);
      },
    );
  }
}
