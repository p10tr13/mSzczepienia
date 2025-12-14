class VaccinationRecordOverviewQuery {
  final int pageNumber;
  final int pageSize;
  final String sortingField;
  final String sortingDirection;

  VaccinationRecordOverviewQuery({
    required this.pageNumber,
    required this.pageSize,
    required this.sortingField,
    required this.sortingDirection,
  });

  Map<String, dynamic> toJson() {
    return {
      'pageNumber': pageNumber,
      'pageSize': pageSize,
      'sortingField': sortingField,
      'sortingDirection': sortingDirection,
    };
  }
}
