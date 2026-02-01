import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:mszczepienia/core/l10n/app_localizations.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/authorization/custom_text_field.dart';
import 'package:mszczepienia/ui/authorization/medical_facility_field.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class SignUpPage extends StatefulWidget {
  const SignUpPage({super.key});

  @override
  State<SignUpPage> createState() => _SignUpPageState();
}

class _SignUpPageState extends State<SignUpPage> {
  final formKey = GlobalKey<FormState>();

  final _nameController = TextEditingController();
  final _surnameController = TextEditingController();
  final _peselController = TextEditingController();
  final _dateOfBirthController = TextEditingController();
  final _medicalFacilityController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _repeatPasswordController = TextEditingController();

  @override
  void dispose() {
    _nameController.dispose();
    _surnameController.dispose();
    _peselController.dispose();
    _dateOfBirthController.dispose();
    _medicalFacilityController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final borderColor = Theme.of(context).colorScheme.outline;
    final grey = Theme.of(context).colorScheme.onSurfaceVariant;

    return BlocBuilder<AuthCubit, AuthState>(
      builder: (context, authState) {
        return Layout(
          child: SingleChildScrollView(
            physics: const BouncingScrollPhysics(),
            child: Padding(
              padding: const EdgeInsets.only(
                left: 24.0,
                right: 24.0,
                top: 8.0,
                bottom: 20.0,
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.goodMorning,
                        style: TextStyle(
                          fontSize: 32,
                          fontWeight: FontWeight.w800,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        l10n.signUpTitle,
                        style: TextStyle(
                          fontSize: 16,
                          color: grey,
                          fontWeight: FontWeight.w500,
                        ),
                      ),

                      const SizedBox(height: 30),

                      Container(
                        padding: const EdgeInsets.all(20),
                        decoration: BoxDecoration(
                          borderRadius: BorderRadius.circular(16),
                          border: Border.all(color: borderColor, width: 1.1),
                        ),
                        child: Form(
                          key: formKey,
                          child: Padding(
                            padding: const EdgeInsets.all(8.0),
                            child: Column(
                              children: [
                                CustomTextField(
                                  label: l10n.name,
                                  controller: _nameController,
                                  showEyeIcon: false,
                                  keyboardType: TextInputType.name,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterName;
                                    }
                                    return null;
                                  },
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.surname,
                                  controller: _surnameController,
                                  showEyeIcon: false,
                                  keyboardType: TextInputType.name,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterSurname;
                                    }
                                    return null;
                                  },
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.pesel,
                                  controller: _peselController,
                                  showEyeIcon: false,
                                  keyboardType: TextInputType.number,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterPesel;
                                    }
                                    if (value.length != 11) {
                                      return l10n.peselCondition;
                                    }
                                    return null;
                                  },
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.dateOfBirth,
                                  controller: _dateOfBirthController,
                                  showEyeIcon: false,
                                  keyboardType: TextInputType.datetime,
                                  readOnly: true,
                                  onTap: () async {
                                    final DateTime? picked = await showDatePicker(
                                      context: context,
                                      initialDate: DateTime.now().subtract(const Duration(days: 365 * 18)),
                                      firstDate: DateTime(1900),
                                      lastDate: DateTime.now(),
                                    );
                                    if (picked != null) {
                                      _dateOfBirthController.text =
                                          '${picked.year.toString().padLeft(4, '0')}-${picked.month.toString().padLeft(2, '0')}-${picked.day.toString().padLeft(2, '0')}';
                                    }
                                  },
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return null;
                                    }
                                    final dateRegex = RegExp(
                                      r'^\d{4}-\d{2}-\d{2}$',
                                    );
                                    if (!dateRegex.hasMatch(value)) {
                                      return l10n.dateOfBirthCondition;
                                    }
                                    try {
                                      DateTime.parse(value);
                                    } catch (e) {
                                      return l10n.dateOfBirthCondition;
                                    }
                                    return null;
                                  },
                                  hintText: 'YYYY-MM-DD',
                                ),

                                const SizedBox(height: 16),

                                MedicalFacilityField(
                                  controller: _medicalFacilityController,
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.email,
                                  controller: _emailController,
                                  showEyeIcon: false,
                                  keyboardType: TextInputType.emailAddress,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterEmail;
                                    }
                                    if (!value.contains('@')) {
                                      return l10n.emailCondition;
                                    }
                                    return null;
                                  },
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.password,
                                  controller: _passwordController,
                                  obscureText: true,
                                  showEyeIcon: true,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterPassword;
                                    }
                                    if (value.length < 6) {
                                      return l10n.passwordCondition;
                                    }
                                    return null;
                                  },
                                ),

                                const SizedBox(height: 16),

                                CustomTextField(
                                  label: l10n.repeatPassword,
                                  controller: _repeatPasswordController,
                                  obscureText: true,
                                  showEyeIcon: true,
                                  validator: (value) {
                                    if (value == null || value.isEmpty) {
                                      return l10n.enterRepeatPassword;
                                    }
                                    if (value != _passwordController.text) {
                                      return l10n.repeatPasswordCondition;
                                    }
                                    return null;
                                  },
                                ),
                              ],
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 15),

                      Center(
                        child: Text(
                          authState is AuthError ? authState.message : '',
                          style: TextStyle(
                            fontSize: 14,
                            color: AppColors.logoRed,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
                  ),

                  Column(
                    children: [
                      SizedBox(
                        width: double.infinity,
                        height: 56,
                        child: ElevatedButton(
                          onPressed: () {
                            if (formKey.currentState!.validate()) {
                              final name = _nameController.text;
                              final surname = _surnameController.text;
                              final pesel = _peselController.text;
                              final dateOfBirth = _dateOfBirthController.text;
                              final defaultMedicalFacility =
                                  _medicalFacilityController.text;
                              final email = _emailController.text;
                              final password = _passwordController.text;

                              context.read<AuthCubit>().signUpRequested(
                                pesel,
                                name,
                                surname,
                                dateOfBirth,
                                defaultMedicalFacility,
                                email,
                                password,
                                l10n.signUpError,
                              );
                            }
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.defaultBlue,
                            elevation: 0,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(30),
                            ),
                          ),
                          child: authState is AuthLoading
                              ? CircularProgressIndicator(color: Colors.white)
                              : Text(
                                  l10n.signUpButton,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.w600,
                                    color: Colors.white,
                                  ),
                                ),
                        ),
                      ),

                      const SizedBox(height: 16),

                      SizedBox(
                        width: double.infinity,
                        height: 56,
                        child: OutlinedButton(
                          onPressed: () {
                            context.go('/login');
                          },
                          style: OutlinedButton.styleFrom(
                            side: BorderSide(color: borderColor, width: 1.5),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(30),
                            ),
                          ),
                          child: Text(
                            l10n.logInButton,
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                        ),
                      ),
                      const SizedBox(height: 16),
                    ],
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
