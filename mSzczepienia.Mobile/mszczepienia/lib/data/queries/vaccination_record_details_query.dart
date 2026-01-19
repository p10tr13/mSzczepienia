class VaccinationRecordDetailsQuery {
  final int vaccineId;

  VaccinationRecordDetailsQuery({required this.vaccineId});

  Map<String, dynamic> toJson() {
    return {'vaccineId': vaccineId};
  }
}
