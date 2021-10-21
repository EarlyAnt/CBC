import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';

import 'package:awesome_dialog/awesome_dialog.dart';
import 'package:console/ui_component/toast.dart';
import 'package:flutter/material.dart';

import 'data/command_data.dart';
import 'data/session.dart';
import 'plugins/udp/udp.dart';
import 'ui_component/game_setting_view.dart';
import 'ui_component/status_bar.dart';
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
  final GlobalKey<StatusBarState> _statusBarKey = GlobalKey();
  StateMonitor? _stateMonitor;
  String? _leftName, _rightName, _leftAvatar, _rightAvatar;
  Size? _screenSize;
  UDP? _sender;
  int? _leftSeconds;

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
    var mediaData = MediaQuery.of(context);
    _screenSize = mediaData.size;

    return SingleChildScrollView(
        child: SizedBox.fromSize(
      size: Size.fromHeight(
          mediaData.size.height + mediaData.viewInsets.bottom / 2),
      child: Scaffold(
        body: Column(children: [
          _titleBar(),
          _effectButtonGroup(),
          _consoleZone(),
        ]),
      ),
    ));
  }

  Widget _titleBar() {
    return Stack(children: [
      Container(
          width: double.infinity,
          height: 50,
          color: Colors.black.withAlpha(220)),
      Padding(
        padding: const EdgeInsets.only(top: 0, left: 20, right: 20),
        child: Row(
          mainAxisSize: MainAxisSize.max,
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.start,
              children: [
                const Text('网络连接',
                    style: TextStyle(
                        color: Colors.white, fontWeight: FontWeight.bold)),
                const SizedBox(width: 5),
                StatusBar(key: _statusBarKey),
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
      )
    ]);
  }

  Widget _timerLabel() {
    _leftSeconds = _leftSeconds ?? 0;
    int minute = _leftSeconds! ~/ 60;
    int second = minute > 0 ? _leftSeconds! - minute * 60 : _leftSeconds!;

    return Text(
        "${minute.toString().padLeft(2, '0')}:${second.toString().padLeft(2, '0')}",
        style: const TextStyle(color: Colors.white));
  }

  Widget _gameEventButton() {
    return TextToggle(_buttonDatas, 60, 30, (gameEvent) {
      currentGameEvent = gameEvent;
      _leftGameSettingViewKey.currentState?.unfocus();
      _rightGameSettingViewKey.currentState?.unfocus();

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
          _endGame();
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
      padding: const EdgeInsets.only(top: 10, left: 20, right: 20),
      child: Container(
        // color: Colors.amber,
        alignment: Alignment.centerLeft,
        height: _screenSize!.height * 0.1,
        child: GridView.builder(
            itemCount: _effectButtonDatas.length,
            shrinkWrap: true,
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                crossAxisCount: 10,
                mainAxisSpacing: 5,
                crossAxisSpacing: 10,
                childAspectRatio: 2),
            itemBuilder: (context, index) {
              return _effectButton(_effectButtonDatas[index]);
            }),
      ),
    );
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
                  onHurt: () {
                    _leftGameSettingViewKey.currentState?.newRound();
                    _rightGameSettingViewKey.currentState?.newRound();
                  },
                )),
            Column(
              mainAxisSize: MainAxisSize.min,
              children: const [
                Icon(Icons.arrow_upward, size: 10, color: Colors.orange),
                SizedBox(height: 30),
                Icon(Icons.arrow_downward, size: 10, color: Colors.orange),
              ],
            ),
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
                  onHurt: () {
                    _leftGameSettingViewKey.currentState?.newRound();
                    _rightGameSettingViewKey.currentState?.newRound();
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

  void _endGame() {
    Size screenSize = MediaQuery.of(context).size;

    AwesomeDialog(
      context: context,
      width: screenSize.width * 0.5,
      dialogType: DialogType.QUESTION,
      headerAnimationLoop: false,
      animType: AnimType.BOTTOMSLIDE,
      // title: '确认',
      desc: '需要清空页面数据吗？',
      btnOkText: '是',
      // btnOkColor: Colors.grey[300],
      btnCancelText: '否',
      // btnCancelColor: Colors.greenAccent[400],
      buttonsTextStyle: const TextStyle(color: Colors.black),
      showCloseIcon: false,
      dismissOnTouchOutside: false,
      dismissOnBackKeyPress: false,
      btnCancelOnPress: () {},
      btnOkOnPress: () {
        _leftName = "";
        _rightName = "";
        _leftAvatar = "";
        _rightAvatar = "";
        _leftGameSettingViewKey.currentState?.reset();
        _rightGameSettingViewKey.currentState?.reset();
      },
    ).show();
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

  void _onConnectStateChanged(ConnectStatus connectStatus) {
    setState(() {
      _statusBarKey.currentState?.setStatus(connectStatus);
    });
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
