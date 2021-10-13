import 'dart:convert';

import 'package:console/ui_component/toast.dart';
import 'package:flutter/material.dart';

import 'data/command_data.dart';
import 'data/session.dart';
import 'plugins/udp/udp.dart';
import 'ui_component/game_setting_view.dart';
import 'ui_component/text_toggle.dart';

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
  String? _leftName, _rightName, _leftAvatar, _rightAvatar;
  Size? _screenSize;
  UDP? _sender;

  @override
  void initState() {
    super.initState();
    _inititialize();
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
        padding: const EdgeInsets.only(top: 5, right: 20),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.end,
          children: [
            _timerLabel(),
            const SizedBox(width: 20),
            _gameEventButton(),
            const SizedBox(width: 5),
            _gameSettingButton(),
          ],
        ));
  }

  Widget _timerLabel() {
    return const Text("14:30", style: TextStyle(color: Colors.black));
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
          // _leftName = "";
          // _rightName = "";
          // _leftAvatar = "";
          // _rightAvatar = "";
          // _leftGameSettingViewKey.currentState?.reset();
          // _rightGameSettingViewKey.currentState?.reset();
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
  }

  void _sendStringMessage(String? message) async {
    int? dataLength = await _sender?.send(
        utf8.encode(message!), Endpoint.broadcast(port: const Port(1000)));

    debugPrint("execute command: $message, length: $dataLength");
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
