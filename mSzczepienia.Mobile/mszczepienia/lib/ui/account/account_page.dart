import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/account/account_custom_container.dart';
import 'package:mszczepienia/ui/layout/layout.dart';
import 'package:url_launcher/url_launcher.dart';

class AccountPage extends StatefulWidget {
  const AccountPage({super.key});

  @override
  State<AccountPage> createState() => _AccountPageState();
}

class _AccountPageState extends State<AccountPage> {
  Future<void> makePhoneCall(String phoneNumber) async {
    final Uri launchUri = Uri(scheme: 'tel', path: phoneNumber);

    // Check if the device can make phone calls
    if (await canLaunchUrl(launchUri)) {
      await launchUrl(launchUri);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;

    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, state) {
        return Layout(
          child: SingleChildScrollView(
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
                    l10n.yourAccount,
                    style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
                  ),
                  const SizedBox(height: 20),
                  Container(
                    padding: const EdgeInsets.all(20),
                    width: double.infinity,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: borderColor, width: 1.1),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.start,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Container(
                          padding: EdgeInsets.all(2.0),
                          child: ElevatedButton(
                            onPressed: () {},
                            style: ElevatedButton.styleFrom(
                              shape: CircleBorder(),
                              padding: EdgeInsets.all(7.0),
                              backgroundColor: AppColors.lightGrey,
                              foregroundColor: AppColors.lightGrey,
                            ),
                            child: Icon(
                              Icons.person,
                              color: AppColors.naturalGrey,
                              size: 20.0,
                            ),
                          ),
                        ),
                        const SizedBox(width: 15),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                l10n.name,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated ? state.user.name : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.surname,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated
                                    ? state.user.surname
                                    : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.email,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated ? state.user.email : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.dateOfBirth,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated
                                    ? DatetimeFormatter().datetimeFormatter(
                                        state.user.dateOfBirth,
                                      )
                                    : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.accountCreationDate,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated
                                    ? DatetimeFormatter().datetimeFormatter(
                                        state.user.creationDate,
                                      )
                                    : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.pesel,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated ? state.user.pesel : '',
                                style: TextStyle(fontSize: 14),
                              ),
                              const SizedBox(height: 15),
                              Text(
                                l10n.medicalFacility,
                                style: TextStyle(
                                  fontSize: 15,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                              const SizedBox(height: 10),
                              Text(
                                state is Authenticated
                                    ? state.user.defaultMedicalFacility
                                    : '',
                                style: TextStyle(fontSize: 14),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),

                  InkWell(
                    onTap: () {
                      makePhoneCall('+48676776767');
                    },
                    borderRadius: BorderRadius.circular(16),
                    child: AccountCustomContainer(
                      icon: Icons.phone,
                      title: l10n.callSupport,
                    ),
                  ),
                  const SizedBox(height: 20),

                  InkWell(
                    onTap: () {
                      context.read<AuthCubit>().logoutRequested();
                    },
                    borderRadius: BorderRadius.circular(16),
                    child: AccountCustomContainer(
                      icon: Icons.logout,
                      title: l10n.logOut,
                      isColored: true,
                    ),
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }
}
