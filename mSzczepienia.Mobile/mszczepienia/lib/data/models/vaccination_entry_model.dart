class VaccinationEntryModel {
  final int id;
  final String? vaccineNameOcr;
  final String? dateOcr;
  final String? batchOcr;
  final String vaccineName;
  final DateTime date;
  final String batch;
  final String status;

  VaccinationEntryModel({
    required this.id,
    required this.vaccineNameOcr,
    required this.dateOcr,
    required this.batchOcr,
    required this.vaccineName,
    required this.date,
    required this.batch,
    required this.status,
  });

  factory VaccinationEntryModel.fromJson(Map<String, dynamic> json) {
    return VaccinationEntryModel(
      id: json['id'] as int,
      vaccineNameOcr: json['vaccineNameOcr'] as String?,
      dateOcr: json['dateOcr'] as String?,
      batchOcr: json['batchOcr'] as String?,
      vaccineName: json['vaccineName'] as String,
      date: DateTime.parse(json['date'] as String),
      batch: json['batch'] as String,
      status: json['status'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'vaccineNameOcr': vaccineNameOcr,
      'dateOcr': dateOcr,
      'batchOcr': batchOcr,
      'vaccineName': vaccineName,
      'date': date.toIso8601String(),
      'batch': batch,
      'status': status,
    };
  }
}
