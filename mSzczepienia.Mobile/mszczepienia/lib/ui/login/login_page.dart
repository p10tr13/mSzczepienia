import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';
import 'package:mszczepienia/logic/auth/auth_cubit.dart';
import 'package:mszczepienia/logic/auth/auth_state.dart';
import 'package:mszczepienia/ui/common/custom_text_field.dart';
import 'package:mszczepienia/ui/layout/layout.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {

  final formKey = GlobalKey<FormState>();

  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }
  
  @override
  Widget build(BuildContext context) {

    final AuthStatus authStatus =  context.read<AuthCubit>().state.status;

    return Layout(
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 20.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  'Good afternoon!',
                  style: TextStyle(
                    fontSize: 32,
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 8),
                const Text(
                  'Log in to the application',
                  style: TextStyle(
                    fontSize: 16,
                    color: AppColors.naturalGrey,
                    fontWeight: FontWeight.w500,
                  ),
                ),
          
                const SizedBox(height: 30),
          
                Container(
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(16),
                    border: Border.all(color: Colors.black, width: 1.0)
                  ),
                  child: Form(
                    key: formKey,
                    child: Padding(
                      padding: const EdgeInsets.all(8.0),
                      child: Column(
                        children: [
                          CustomTextField(
                            label: "Email",
                            controller: _emailController,
                            showEyeIcon: false,
                            keyboardType: TextInputType.emailAddress,
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'Enter email address';
                              }
                              if (!value.contains('@')) {
                                return 'Email has to contain @';
                              }
                              return null;
                            },
                          ),
                          
                          const SizedBox(height: 16),
                    
                          CustomTextField(
                            label: "Password",
                            controller: _passwordController,
                            obscureText: true,
                            showEyeIcon: true,
                            validator: (value) {
                              if (value == null || value.isEmpty) {
                                return 'Enter password';
                              }
                              return null;
                            },
                          ),
                        ],
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 15,),

                Center(
                  child: Text(
                    authStatus == AuthStatus.loginFailed ? 'Incorrect credentials' : '',
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
                        final email = _emailController.text;
                        final password = _passwordController.text;

                        context.read<AuthCubit>().loginRequested(email, password);
                      }
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.buttonBlue, 
                      elevation: 0,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(30),
                      ),
                    ),
                    child: authStatus == AuthStatus.loading ? CircularProgressIndicator(color: Colors.white,) : Text(
                      'Log in',
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
                    onPressed: () {},
                    style: OutlinedButton.styleFrom(
                      side: const BorderSide(color: Colors.black, width: 1.5),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(30),
                      ),
                    ),
                    child: const Text(
                      'Sign in',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                        color: Colors.black,
                      ),
                    ),
                  ),
                ),
                const SizedBox(height: 16)
              ],
            ),
          ],
        ),
      ),
    );
  }
}
