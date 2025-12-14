import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/data/models/vaccination_record_overview_model.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_cubit.dart';
import 'package:mszczepienia/logic/vaccination_record/vaccination_records_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:mszczepienia/ui/vaccination_history/vaccination_history_custom_container.dart';

class VaccinationHistoryPage extends StatefulWidget {
  const VaccinationHistoryPage({super.key});

  @override
  State<VaccinationHistoryPage> createState() => _VaccinationHistoryState();
}

class _VaccinationHistoryState extends State<VaccinationHistoryPage> {
  // Sorting options
  List<String> sortingOptions = [];
  String? dropdownValue;

  // Default sorting values
  String _sortingField = 'vaccineName';
  String _sortingDirection = 'asc';

  // Scrolling controller and data items
  final ScrollController _scrollController = ScrollController();
  final List<VaccinationRecordOverviewModel> _items = [];

  // Pagination state
  bool _isLoading = false;
  bool _hasMore = true;
  int _currentPage = 1;
  final int _pageSize = 10;

  @override
  void initState() {
    super.initState();
    _scrollController.addListener(() {
      if (_scrollController.position.pixels >=
              _scrollController.position.maxScrollExtent - 200 &&
          !_isLoading &&
          _hasMore) {
        _loadData();
      }
    });
    _loadData();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  /// Loads data from the API using the Cubit
  void _loadData() {
    if (_isLoading || !_hasMore) return;

    setState(() {
      _isLoading = true;
    });

    debugPrint('Loading page: $_currentPage');

    context.read<VaccinationRecordsCubit>().loadHistory(
      _currentPage,
      _pageSize,
      _sortingField,
      _sortingDirection,
    );
  }

  /// Updates sorting parameters based on selected dropdown value
  void _updateSortingParameters(String sortValue, AppLocalizations l10n) {
    if (sortValue == l10n.alphabetical_az) {
      _sortingField = 'vaccineName';
      _sortingDirection = 'asc';
    } else if (sortValue == l10n.alphabetical_za) {
      _sortingField = 'vaccineName';
      _sortingDirection = 'desc';
    } else if (sortValue == l10n.date_newest) {
      _sortingField = 'vaccinationDate';
      _sortingDirection = 'desc';
    } else if (sortValue == l10n.date_oldest) {
      _sortingField = 'vaccinationDate';
      _sortingDirection = 'asc';
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    // Initialize sorting options on first build
    if (sortingOptions.isEmpty) {
      sortingOptions = [
        l10n.alphabetical_az,
        l10n.alphabetical_za,
        l10n.date_newest,
        l10n.date_oldest,
      ];
      dropdownValue = sortingOptions.first;
    }

    return Layout(
      child: Padding(
        padding: const EdgeInsets.only(
          left: 24.0,
          right: 24.0,
          top: 8.0,
          bottom: 8.0,
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              l10n.yourVaccinations,
              style: const TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
            ),
            const SizedBox(height: 20),

            _buildSortingDropdown(context, l10n),

            const SizedBox(height: 20),

            Expanded(child: _buildVaccinationList(l10n)),
          ],
        ),
      ),
    );
  }

  /// Sorting dropdown widget
  Widget _buildSortingDropdown(BuildContext context, AppLocalizations l10n) {
    return DropdownMenu<String>(
      initialSelection: dropdownValue,
      trailingIcon: const Icon(Icons.sort_rounded),
      menuStyle: MenuStyle(
        shape: WidgetStatePropertyAll(
          RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(16.0),
            side: BorderSide(
              color: Theme.of(context).colorScheme.outline,
              width: 1.0,
            ),
          ),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        isDense: true,
        constraints: const BoxConstraints(maxHeight: 50),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16.0),
          borderSide: BorderSide(
            color: Theme.of(context).colorScheme.outline,
            width: 1.0,
          ),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(16.0),
          borderSide: BorderSide(
            color: Theme.of(context).colorScheme.outline,
            width: 1.0,
          ),
        ),
        contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
      ),
      textStyle: const TextStyle(fontSize: 14),
      menuHeight: 150,
      onSelected: (String? value) {
        if (value != null && value != dropdownValue) {
          _updateSortingParameters(value, l10n);
          setState(() {
            dropdownValue = value;
            _items.clear();
            _currentPage = 1;
            _hasMore = true;
            _isLoading = false;
          });
          _loadData();
        }
      },
      dropdownMenuEntries: sortingOptions.map<DropdownMenuEntry<String>>((
        String value,
      ) {
        return DropdownMenuEntry<String>(
          value: value,
          label: value,
          style: MenuItemButton.styleFrom(
            textStyle: const TextStyle(fontSize: 13),
          ),
        );
      }).toList(),
    );
  }

  /// Build the vaccination records list with infinite scrolling
  Widget _buildVaccinationList(AppLocalizations l10n) {
    return BlocConsumer<VaccinationRecordsCubit, VaccinationRecordsState>(
      listener: (context, state) {
        // State changes handling
        if (state is VaccinationRecordsLoaded) {
          setState(() {
            _items.addAll(state.records);
            _currentPage++;
            _isLoading = false;

            if (state.records.length < _pageSize) {
              _hasMore = false;
            }
          });
        } else if (state is VaccinationRecordsError) {
          debugPrint('Error loading: ${state.message}');
          setState(() {
            _isLoading = false;
          });
        }
      },
      builder: (context, state) {
        if (state is VaccinationRecordsLoading && _items.isEmpty) {
          return const Center(child: CircularProgressIndicator());
        }
        if (state is VaccinationRecordsError && _items.isEmpty) {
          return Center(child: Text(l10n.errorLoadingVaccinationRecords));
        }
        if (_items.isEmpty && !_isLoading) {
          return Center(child: Text(l10n.noVaccinations));
        }

        // List view
        return ListView.separated(
          controller: _scrollController,
          itemCount: _items.length + (_hasMore ? 1 : 0),
          separatorBuilder: (context, index) => const SizedBox(height: 12),
          itemBuilder: (context, index) {
            // Loader at the end for infinite scroll
            if (index == _items.length) {
              return const Center(
                child: Padding(
                  padding: EdgeInsets.all(16.0),
                  child: CircularProgressIndicator(),
                ),
              );
            }

            final record = _items[index];
            return InkWell(
              onTap: () {
                context.push('/history/details?vaccineId=${record.id}');
              },
              child: VaccinationHistoryCustomContainer(
                date: DatetimeFormatter().datetimeFormatter(
                  record.vaccinationDate,
                ),
                vaccineName: record.vaccineName,
                manufacturer: record.manufacturer,
                dosesNumber: record.dosesNumber,
              ),
            );
          },
        );
      },
    );
  }
}
