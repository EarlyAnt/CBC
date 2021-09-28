import 'dart:ui';

import 'package:flutter/material.dart';

abstract class CustomColors {
  static const primary = Color(0xFF0A3D91);
  static const accent = Color(0xFFCC0000);

  static const rowCardBackground = Colors.white;
  static const scaffoldBackground = Color.fromRGBO(242, 242, 247, 1.0);

  static const sensorTagRed = Color(0xFFCC0000);

  static const int _gululuBluePrimaryValue = 0xFF44B2E8;
  static const MaterialColor gululuBlue = MaterialColor(
    _gululuBluePrimaryValue,
    <int, Color>{
      50: Color(0xFFEBF7FE),
      100: Color(0xFFDCF3FF),
      200: Color(0xFFB1E5FF),
      300: Color(0xFF82D3FB),
      400: Color(0xFF42A5F5),
      500: Color(_gululuBluePrimaryValue),
      600: Color(0xFF30A7E1),
      700: Color(0xFF2097D1),
      800: Color(0xFF0F76A8),
      900: Color(0xFF0B5D94),
    },
  );
}
