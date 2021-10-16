import 'dart:async';

import 'package:flutter/material.dart';

class StateMonitor {
  StateMonitor({required this.onStateChanged});

  final Function(bool)? onStateChanged;
  final int _interval = 500;
  Timer? _timer;
  DateTime? _lastHeartbeat;
  bool? _connected;
  bool get connnected => _connected ?? false;

  void startTimer() {
    stopTimer();
    _timer = Timer.periodic(Duration(milliseconds: _interval), (timer) {
      Duration? diff = DateTime.now().difference(_lastHeartbeat!);
      bool connected = diff.inMilliseconds <= 200;
      debugPrint(
          'connected: $connected, lastHeartbeat: $_lastHeartbeat, diff in milliSeconds: ${diff.inMilliseconds}');

      if (connected != _connected && onStateChanged != null) {
        onStateChanged?.call(connected);
      }
      _connected = connected;
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
