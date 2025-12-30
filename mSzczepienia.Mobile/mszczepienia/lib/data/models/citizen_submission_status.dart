class CitizenSubmissionStatus {
  final String submissionStatus;

  CitizenSubmissionStatus({required this.submissionStatus});

  factory CitizenSubmissionStatus.fromJson(Map<String, dynamic> json) {
    return CitizenSubmissionStatus(
      submissionStatus: json['submissionStatus'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'submissionStatus': submissionStatus,
    };
  }
}