import 'package:flutter/widgets.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';

class VaccinationDoseContainer extends StatelessWidget {
  const VaccinationDoseContainer({
    super.key,
    required this.doseNumber,
    required this.vaccinationDate,
    required this.batchNumber,
    this.remarks,
    this.nextVaccinationDate,
  });

  final String doseNumber;
  final DateTime vaccinationDate;
  final String batchNumber;
  final String? remarks;
  final DateTime? nextVaccinationDate;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '${l10n.doseNumber}: $doseNumber.',
          style: const TextStyle(fontSize: 15, fontWeight: FontWeight.w600),
        ),
        const SizedBox(height: 5),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(
              "${l10n.vaccinationDate}:",
              style: const TextStyle(fontSize: 14),
            ),
            Expanded(
              child: Text(
                DatetimeFormatter().datetimeFormatter(vaccinationDate),
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                ),
                textAlign: TextAlign.end,
              ),
            ),
          ],
        ),
        const SizedBox(height: 5),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text("${l10n.batchNo}:", style: const TextStyle(fontSize: 14)),
            Expanded(
              child: Text(
                batchNumber,
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                ),
                textAlign: TextAlign.end,
              ),
            ),
          ],
        ),
        const SizedBox(height: 5),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text("${l10n.remarks}:", style: const TextStyle(fontSize: 14)),
            SizedBox(
              width: 120,
              child: Text(
                remarks ?? '',
                style: const TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.w600,
                ),
                textAlign: TextAlign.end,
              ),
            ),
          ],
        ),
        const SizedBox(height: 5),
        ?nextVaccinationDate == null
            ? null
            : Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    "${l10n.nextVaccination}:",
                    style: const TextStyle(fontSize: 14),
                  ),
                  SizedBox(
                    width: 120,
                    child: Text(
                      '${nextVaccinationDate?.difference(vaccinationDate).inDays} ${l10n.days}',
                      style: const TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                      ),
                      textAlign: TextAlign.end,
                    ),
                  ),
                ],
              ),
      ],
    );
  }
}
