import 'package:flutter/material.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';

class Layout extends StatelessWidget {
  const Layout({super.key, required this.child});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    //final isDarkMode = Theme.of(context).brightness == Brightness.dark;

    return Scaffold(
      extendBodyBehindAppBar: true,
      resizeToAvoidBottomInset: true,
      appBar: AppBar(
        centerTitle: false,
        toolbarHeight: 70.0,
        backgroundColor: Colors.transparent,
        scrolledUnderElevation: 0,
        title: Padding(
          padding: const EdgeInsets.only(top: 20.0, bottom: 20.0, left: 8.0),
          child: Row(
            children: [
              Container(
                width: 32,
                height: 32,
                decoration: BoxDecoration(
                  color: AppColors.logoRed,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: const Icon(Icons.shield, color: Colors.white, size: 20),
              ),
              const SizedBox(width: 12),
              const Text(
                'mSzczepienia',
                style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
            ],
          ),
        ),
      ),
      body: SafeArea(child: child),

      //TEMPORATY DISABLED GRADIENT BACKGROUND FOR SMOOTHNESS OF THE APP
      // body: Container(
      //   width: double.infinity,
      //   height: double.infinity,
      //   decoration: BoxDecoration(
      //     gradient: LinearGradient(
      //       begin: Alignment.topLeft,
      //       end: Alignment.bottomRight,
      //       colors: isDarkMode
      //           ? [AppColors.darkGrey, AppColors.defaultBlue]
      //           : [AppColors.lightGrey, AppColors.lightBlue],
      //     ),
      //   ),
      //   child: SafeArea(child: child),
      // ),
    );
  }
}
