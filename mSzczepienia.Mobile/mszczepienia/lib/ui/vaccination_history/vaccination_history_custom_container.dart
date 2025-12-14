import 'package:flutter/material.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';

class VaccinationHistoryCustomContainer extends StatelessWidget {
  const VaccinationHistoryCustomContainer({
    super.key,
    required this.date,
    required this.vaccineName,
    required this.manufacturer,
    required this.dosesNumber,
  });

  final String date;
  final String vaccineName;
  final String manufacturer;
  final int dosesNumber;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return Container(
      padding: const EdgeInsets.all(10),
      width: double.infinity,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: borderColor, width: 1.1),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('${l10n.last} $date', style: TextStyle(fontSize: 13)),
                const SizedBox(height: 5),
                Text(
                  vaccineName,
                  style: TextStyle(fontSize: 15, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 5),
                Text(manufacturer, style: TextStyle(fontSize: 14)),
              ],
            ),
          ),
          Text(
            '${l10n.noOfDoses} $dosesNumber',
            style: TextStyle(fontSize: 14, fontWeight: FontWeight.bold),
          ),
        ],
      ),
    );
  }
}
