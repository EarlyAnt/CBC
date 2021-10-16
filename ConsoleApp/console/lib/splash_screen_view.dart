import 'package:console/console_view.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class SplashScreenView extends StatefulWidget {
  const SplashScreenView({Key? key}) : super(key: key);
  @override
  _SplashScreenViewState createState() {
    return _SplashScreenViewState();
  }
}

class _SplashScreenViewState extends State<SplashScreenView>
    with SingleTickerProviderStateMixin {
  final String _version = "1.2.0";
  late AnimationController _controller;
  late Animation<double> _animation;
  bool _fadeOver = false;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
        vsync: this, duration: const Duration(milliseconds: 3000));
    _animation = Tween(begin: 0.0, end: 1.0).animate(_controller);
    _animation.addStatusListener((status) {
      if (status == AnimationStatus.completed) {
        setState(() {
          _fadeOver = true;
        });

        Future.delayed(const Duration(seconds: 1)).then((value) {
          Navigator.of(context).pushAndRemoveUntil(
              MaterialPageRoute(builder: (subContext) {
            return const ConsoleView();
          }), (route) => route.settings.name == null);
        });
      }
    });
    _controller.forward();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    SystemChrome.setEnabledSystemUIMode(SystemUiMode.immersiveSticky);

    return Scaffold(
      backgroundColor: Colors.black,
      body: Stack(
        children: [
          Center(
            child: FadeTransition(
              opacity: _animation,
              child: Image.asset(
                "assets/images/ui/splash.png",
                scale: 1.0,
                fit: BoxFit.contain,
              ),
            ),
          ),
          Align(
            alignment: Alignment.bottomRight,
            child: Padding(
              padding: const EdgeInsets.only(bottom: 7, right: 30),
              child: Visibility(
                visible: _fadeOver,
                child: Text(
                  _version,
                  style: TextStyle(fontSize: 10, color: Colors.grey.shade500),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
