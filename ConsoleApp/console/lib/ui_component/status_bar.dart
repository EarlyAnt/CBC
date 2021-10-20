import 'package:flutter/material.dart';

import '../utils/state_monitor.dart';

class StatusBar extends StatefulWidget {
  StatusBar({Key? key}) : super(key: key);

  @override
  StatusBarState createState() => StatusBarState();
}

class StatusBarState extends State<StatusBar> {
  ConnectStatus? _connectStatus;

  @override
  Widget build(BuildContext context) {
    Color color = Colors.white;
    String iconPath = 'assets/images/ui/wifi1.png';
    switch (_connectStatus ?? ConnectStatus.unconnect) {
      case ConnectStatus.unconnect:
        break;
      case ConnectStatus.connect:
        color = Colors.red;
        iconPath = 'assets/images/ui/wifi2.png';
        break;
      case ConnectStatus.better:
        color = Colors.yellow[700]!;
        iconPath = 'assets/images/ui/wifi3.png';
        break;
      case ConnectStatus.best:
        color = Colors.green;
        iconPath = 'assets/images/ui/wifi4.png';
        break;
    }

    return Image.asset(iconPath, width: 14, height: 14, color: color);
  }

  void setStatus(ConnectStatus status) {
    setState(() {
      _connectStatus = status;
    });
  }
}
