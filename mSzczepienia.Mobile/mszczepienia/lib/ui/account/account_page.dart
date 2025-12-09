import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/core/utils/datetime_formatter.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class AccountPage extends StatefulWidget {
  const AccountPage({super.key});

  @override
  State<AccountPage> createState() => _AccountPageState();
}

class _AccountPageState extends State<AccountPage> {
  @override
  Widget build(BuildContext context) {
    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, state) {
        return Layout(
          child: SingleChildScrollView(
            child: Padding(
              padding: const EdgeInsets.only(
                left: 24.0,
                right: 24.0,
                top: 8.0,
                bottom: 20.0,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    'Your Account',
                    style: TextStyle(fontSize: 32, fontWeight: FontWeight.w800),
                  ),
                  const SizedBox(height: 20),
                  Container(
                    padding: const EdgeInsets.all(20),
                    width: double.infinity,
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(16),
                      border: Border.all(color: Colors.black, width: 1.0),
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
                              padding: EdgeInsets.all(12.0),
                              backgroundColor: AppColors.lightGrey,
                              foregroundColor: AppColors.lightGrey,
                            ),
                            child: Icon(
                              Icons.person,
                              color: AppColors.naturalGrey,
                              size: 25.0,
                            ),
                          ),
                        ),
                        const SizedBox(width: 10),
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            const Text(
                              "Name",
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
                            const Text(
                              "Surname",
                              style: TextStyle(
                                fontSize: 15,
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            const SizedBox(height: 10),
                            Text(
                              state is Authenticated ? state.user.surname : '',
                              style: TextStyle(fontSize: 14),
                            ),
                            const SizedBox(height: 15),
                            const Text(
                              "Email",
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
                            const Text(
                              "Date of birth",
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
                            const Text(
                              "Account creation date",
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
                            const Text(
                              "PESEL",
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
                            const Text(
                              "Default Medical Facility",
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
                      ],
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
