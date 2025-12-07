import 'package:intl/intl.dart';

class DatetimeFormatter {
    String datetimeFormatter(DateTime dt) {
    final DateFormat formatter = DateFormat('yyyy-MM-dd');
    final String formatted = formatter.format(dt);

    return formatted;
  }
}