import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:console/ui_component/toast.dart';
import 'package:flutter/material.dart';

import 'data/command_data.dart';
import 'data/session.dart';
import 'plugins/udp/udp.dart';
import 'ui_component/game_setting_view.dart';
import 'ui_component/text_toggle.dart';
import 'utils/state_monitor.dart';

class ConsoleView extends StatefulWidget {
  const ConsoleView({Key? key}) : super(key: key);

  @override
  _ConsoleViewState createState() => _ConsoleViewState();
}

class _ConsoleViewState extends State<ConsoleView> {
  final List<EffectButtonData> _effectButtonDatas = [
    EffectButtonData("UR卡", CommandUtil.buildAnimationCommand("ur")),
    EffectButtonData("CR卡", CommandUtil.buildAnimationCommand("cr")),
    EffectButtonData("掌声", CommandUtil.buildAudioCommand("guzhang")),
    EffectButtonData("欢呼", CommandUtil.buildAudioCommand("huanhu")),
    // EffectButtonData("UR卡", "ur"),
    // EffectButtonData("CR卡", "cr"),
    // EffectButtonData("掌声", "zhangsheng"),
    // EffectButtonData("欢呼", "huanhu"),
    // EffectButtonData("UR卡", "ur"),
    // EffectButtonData("CR卡", "cr"),
    // EffectButtonData("掌声", "zhangsheng"),
    // EffectButtonData("欢呼", "huanhu"),
    // EffectButtonData("UR卡", "ur"),
    // EffectButtonData("CR卡", "cr"),
    // EffectButtonData("掌声", "zhangsheng"),
    // EffectButtonData("欢呼", "huanhu"),
  ];
  final List<ButtonData> _buttonDatas = [
    ButtonData(text: "开始", color: Colors.green, value: GameEvent.start),
    ButtonData(
        text: "暂停", color: Colors.amber.shade600, value: GameEvent.pause),
    ButtonData(text: "结束", color: Colors.red, value: GameEvent.end)
  ];
  final GlobalKey<GameSettingViewState> _leftGameSettingViewKey = GlobalKey();
  final GlobalKey<GameSettingViewState> _rightGameSettingViewKey = GlobalKey();
  final GlobalKey<TextToggleState> _gameEventToggleKey = GlobalKey();
  StateMonitor? _stateMonitor;
  String? _leftName, _rightName, _leftAvatar, _rightAvatar;
  Size? _screenSize;
  UDP? _sender;
  int? _leftSeconds;
  bool? _connected;

  @override
  void initState() {
    super.initState();
    _inititialize();

    _stateMonitor = StateMonitor(onStateChanged: _onConnectStateChanged);
    _stateMonitor?.startTimer();
  }

  @override
  void dispose() {
    _stateMonitor?.stopTimer();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    _screenSize = MediaQuery.of(context).size;

    return Scaffold(
      body: Stack(children: [
        // Image.asset("assets/images/ui/title_bar.png"),
        Column(children: [
          _titleBar(),
          _effectButtonGroup(),
          _consoleZone(),
        ]),
      ]),
    );
  }

  Widget _titleBar() {
    return Padding(
      padding: const EdgeInsets.only(top: 5, left: 20, right: 20),
      child: Row(
        mainAxisSize: MainAxisSize.max,
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.start,
            children: [
              const Text('网络连接'),
              const SizedBox(width: 5),
              _statusBar(),
            ],
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              _timerLabel(),
              const SizedBox(width: 20),
              _gameEventButton(),
              const SizedBox(width: 5),
              _gameSettingButton(),
            ],
          ),
        ],
      ),
    );
  }

  Widget _statusBar() {
    return ClipOval(
        child: Image.asset('assets/images/ui/square_bg.png',
            width: 12,
            height: 12,
            color: (_connected ?? false) ? Colors.green : Colors.red));
  }

  Widget _timerLabel() {
    _leftSeconds = _leftSeconds ?? 0;
    int minute = _leftSeconds! ~/ 60;
    int second = minute > 0 ? _leftSeconds! - minute * 60 : _leftSeconds!;

    return Text(
        "${minute.toString().padLeft(2, '0')}:${second.toString().padLeft(2, '0')}",
        style: const TextStyle(color: Colors.black));
  }

  Widget _gameEventButton() {
    return TextToggle(_buttonDatas, 60, 30, (gameEvent) {
      currentGameEvent = gameEvent;
      switch (gameEvent) {
        case GameEvent.start:
          if (_leftName == null ||
              _leftName!.isEmpty ||
              _rightName == null ||
              _rightName!.isEmpty) {
            MessageBox.show('请先设置玩家姓名');
            _gameEventToggleKey.currentState?.refresh();
            return;
          }

          _sendStringMessage(CommandUtil.buildStartGameCommand(
            _leftName ?? '',
            _rightName ?? '',
            _leftAvatar ?? '',
            _rightAvatar ?? '',
          ));
          break;
        case GameEvent.pause:
          _sendStringMessage(CommandUtil.buildPauseGameCommand());
          break;
        case GameEvent.end:
          _sendStringMessage(CommandUtil.buildStopGameCommand());
          _leftName = "";
          _rightName = "";
          _leftAvatar = "";
          _rightAvatar = "";
          _leftGameSettingViewKey.currentState?.reset();
          _rightGameSettingViewKey.currentState?.reset();
          break;
      }
    }, spacing: 5, defaultButtonIndex: 2, key: _gameEventToggleKey);
  }

  Widget _gameSettingButton() {
    return TextButton(
        child: const Text("比赛设置", style: TextStyle(color: Colors.white)),
        onPressed: () {});
  }

  Widget _effectButtonGroup() {
    return Padding(
        padding: const EdgeInsets.only(top: 0),
        child: Column(
          children: [
            Row(mainAxisAlignment: MainAxisAlignment.start, children: [
              Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 20),
                  child: _effectZoneLabel())
            ]),
            Padding(
              padding: const EdgeInsets.only(top: 10, left: 20, right: 20),
              child: SizedBox(
                height: _screenSize!.height * 0.12,
                child: GridView.builder(
                    itemCount: _effectButtonDatas.length,
                    shrinkWrap: true,
                    // physics: BouncingScrollPhysics(),
                    gridDelegate:
                        const SliverGridDelegateWithFixedCrossAxisCount(
                            crossAxisCount: 10,
                            mainAxisSpacing: 5,
                            crossAxisSpacing: 10,
                            childAspectRatio: 2),
                    itemBuilder: (context, index) {
                      return _effectButton(_effectButtonDatas[index]);
                    }),
              ),
            ),
          ],
        ));
  }

  Widget _effectZoneLabel() {
    return const Text("特效区");
  }

  Widget _effectButton(EffectButtonData effectButtonData) {
    return TextButton(
        child: Text(effectButtonData.buttonText),
        onPressed: () {
          _sendStringMessage(effectButtonData.effectCommand);
        });
  }

  Widget _consoleZone() {
    return Expanded(
      child: Padding(
        padding: const EdgeInsets.only(top: 0, left: 20, right: 20, bottom: 10),
        child: Row(
          children: [
            Expanded(
                flex: 1,
                child: GameSettingView(
                  player: "left",
                  key: _leftGameSettingViewKey,
                  onNameChanged: (name) {
                    _leftName = name;
                  },
                  onAvatarChanged: (avatarFileName) {
                    _leftAvatar = avatarFileName;
                  },
                )),
            const SizedBox(width: 10),
            Expanded(
                flex: 1,
                child: GameSettingView(
                  player: "right",
                  key: _rightGameSettingViewKey,
                  onNameChanged: (name) {
                    _rightName = name;
                  },
                  onAvatarChanged: (avatarFileName) {
                    _rightAvatar = avatarFileName;
                  },
                )),
          ],
        ),
      ),
    );
  }

  void _inititialize() async {
    _sender = await UDP.bind(Endpoint.any(port: const Port(2000)));

    RawDatagramSocket.bind(InternetAddress.anyIPv4, 4000)
        .then((RawDatagramSocket udpSocket) {
      udpSocket.forEach((RawSocketEvent event) {
        if (event == RawSocketEvent.read) {
          Datagram? dg = udpSocket.receive();
          if (dg != null) {
            _onReceiveData(dg.data);
          } else {
            debugPrint("receive data error: datagram is null");
          }
        }
      });
    });
  }

  void _sendStringMessage(String? message) async {
    int? dataLength = await _sender?.send(
        utf8.encode(message!), Endpoint.broadcast(port: const Port(1000)));

    debugPrint("execute command: $message, length: $dataLength");
  }

  void _onReceiveData(Uint8List data) {
    try {
      String dataString = String.fromCharCodes(data);
      // debugPrint('receive message: $dataString');

      Map heartbeat = json.decode(dataString);
      if (heartbeat.containsKey("GameEvent")) {
        _stateMonitor?.registerHeartbeat();

        if (heartbeat["GameEvent"] == GameEvent.start.index &&
            heartbeat.containsKey("LeftSeconds")) {
          int leftSeconds = heartbeat["LeftSeconds"];
          if (leftSeconds != _leftSeconds) {
            _leftSeconds = heartbeat["LeftSeconds"];
            debugPrint("sync left seconds: $_leftSeconds");
            setState(() {});
          }
          return;
        }
      }
    } catch (ex) {
      debugPrint("receive data error: $ex");
    }
  }

  void _onConnectStateChanged(bool connected) {
    setState(() {
      _connected = connected;
    });
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
