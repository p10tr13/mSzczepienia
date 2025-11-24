import 'package:flutter/material.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';

class Layout extends StatelessWidget {
  const Layout({super.key, required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      resizeToAvoidBottomInset: false,
      appBar: AppBar(
        centerTitle: false, 
        toolbarHeight: 60.0,
        title: Padding(
          padding: const EdgeInsets.only(top: 30.0, bottom: 8.0, left: 8.0),
          child: Row(
            children: [
              Container(
                width: 32,
                height: 32,
                decoration: BoxDecoration(
                  color: AppColors.logoRed,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: const Icon(
                  Icons.shield,
                  color: Colors.white,
                  size: 20,
                ),
              ),
              const SizedBox(width: 12),
              const Text(
                'mSzczepienia',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                  color: Colors.black, 
                ),
              ),
            ],
          ),
        ),
      ),
      body: child,
    );
  }
}