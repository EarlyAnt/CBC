import 'package:flutter/material.dart';
import 'splash_screen_view.dart';
import 'styles/custom_colors.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Console',
      theme: buildTheme(),
      debugShowCheckedModeBanner: false,
      // home: MyHomePage(title: 'Flutter Demo Home Page'),
      home: const SplashScreenView(),
    );
  }

  ThemeData buildTheme() {
    var themeData = ThemeData.from(
      colorScheme: ColorScheme.fromSwatch(
          primarySwatch: CustomColors.gululuBlue,
          backgroundColor: Colors.white,
          errorColor: const Color.fromRGBO(255, 92, 92, 1)),
      textTheme: const TextTheme(
        bodyText1: TextStyle(
            fontSize: 16,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.bold),
        bodyText2: TextStyle(
            fontSize: 14,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.normal),
        subtitle1: TextStyle(
            fontSize: 18,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.normal),
        subtitle2: TextStyle(
            fontSize: 20,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.normal),
        headline6: TextStyle(
          fontSize: 18,
          color: Color.fromRGBO(68, 68, 68, 1),
        ),
        headline5: TextStyle(
            fontSize: 22,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.w800),
        headline4: TextStyle(
            fontSize: 24,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.w800),
        headline3: TextStyle(
            fontSize: 26,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.w800),
        headline2: TextStyle(
            fontSize: 28,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.w800),
        headline1: TextStyle(
            fontSize: 30,
            color: Color.fromRGBO(68, 68, 68, 1),
            fontWeight: FontWeight.w800),
      ),
    );

    var appBarTheme = themeData.appBarTheme.copyWith(
      backgroundColor: Colors.white,
      textTheme: TextTheme(
          bodyText2: themeData.textTheme.bodyText2!
              .copyWith(color: const Color.fromRGBO(188, 188, 188, 1)),
          headline6: themeData.textTheme.headline6!
              .copyWith(fontWeight: FontWeight.normal)),
      centerTitle: true,
      elevation: 1,
    );

    var textButtonTheme = TextButtonThemeData(
        style: ButtonStyle(
      shape: MaterialStateProperty.all<RoundedRectangleBorder>(
          RoundedRectangleBorder(borderRadius: BorderRadius.circular(4))),
      textStyle: MaterialStateProperty.all<TextStyle>(
          themeData.textTheme.bodyText2!.copyWith(fontSize: 14)),
      padding: MaterialStateProperty.all<EdgeInsetsGeometry>(
          const EdgeInsets.symmetric(vertical: 5, horizontal: 10)),
      foregroundColor: MaterialStateColor.resolveWith((states) => Colors.white),
      backgroundColor:
          MaterialStateColor.resolveWith((states) => CustomColors.gululuBlue),
      minimumSize: MaterialStateProperty.all(const Size(60, 5)),
    ));

    return themeData.copyWith(
      appBarTheme: appBarTheme,
      textButtonTheme: textButtonTheme,
    );
  }
}
