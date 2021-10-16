import 'dart:async';

import 'package:flutter/material.dart';

class StateMonitor {
  StateMonitor({required this.onStateChanged});

  final Function(ConnectStatus)? onStateChanged;
  final int _interval = 500;
  Timer? _timer;
  DateTime? _lastHeartbeat;
  ConnectStatus? _connectStatus;
  ConnectStatus get connectStatus => _connectStatus ?? ConnectStatus.unconnect;

  void startTimer() {
    stopTimer();
    _timer = Timer.periodic(Duration(milliseconds: _interval), (timer) {
      Duration? diff = DateTime.now().difference(_lastHeartbeat!);
      ConnectStatus connected = ConnectStatus.unconnect;
      if (diff.inMilliseconds <= 50) {
        connected = ConnectStatus.best;
      } else if (diff.inMilliseconds <= 100) {
        connected = ConnectStatus.better;
      } else if (diff.inMilliseconds <= 200) {
        connected = ConnectStatus.connect;
      }

      debugPrint(
          'connected: $connected, lastHeartbeat: $_lastHeartbeat, diff in milliSeconds: ${diff.inMilliseconds}');

      if (connected != _connectStatus && onStateChanged != null) {
        onStateChanged?.call(connected);
      }
      _connectStatus = connected;
    });
  }

  void stopTimer() {
    _lastHeartbeat = DateTime.now();
    _timer?.cancel();
  }

  void registerHeartbeat() {
    _lastHeartbeat = DateTime.now();
  }
}

enum ConnectStatus { unconnect, connect, better, best }
