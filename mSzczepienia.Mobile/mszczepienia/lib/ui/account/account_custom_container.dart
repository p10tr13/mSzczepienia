import 'package:flutter/material.dart';
import 'package:mszczepienia/core/theme/app_colors.dart';

class AccountCustomContainer extends StatelessWidget {
  const AccountCustomContainer({
    super.key,
    required this.icon,
    required this.title,
    this.isColored = false,
  });

  final IconData icon;
  final String title;
  final bool isColored;

  @override
  Widget build(BuildContext context) {
    final borderColor = Theme.of(context).colorScheme.outline;

    return Container(
      padding: const EdgeInsets.all(10),
      width: double.infinity,
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: borderColor, width: 1.1),
      ),
      child: Row(
        children: [
          Container(
            padding: EdgeInsets.only(left: 12.0, right: 2.0),
            child: ElevatedButton(
              onPressed: () {},
              style: ElevatedButton.styleFrom(
                shape: CircleBorder(),
                padding: EdgeInsets.all(7.0),
                backgroundColor: AppColors.lightGrey,
                foregroundColor: AppColors.lightGrey,
              ),
              child: Icon(icon, color: AppColors.naturalGrey, size: 20.0),
            ),
          ),
          const SizedBox(width: 15),
          Expanded(
            child: Text(
              title,
              style: isColored
                  ? TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.bold,
                      color: AppColors.logoRed,
                    )
                  : TextStyle(fontSize: 15),
            ),
          ),
        ],
      ),
    );
  }
}
