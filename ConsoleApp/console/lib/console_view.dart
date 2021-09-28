import 'package:flutter/material.dart';

import 'data/command_data.dart';
import 'plugins/udp/udp.dart';
import 'ui_component/game_setting_view.dart';

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
  Size? _screenSize;
  UDP? _sender;
  String? _message;
  int? _dataLength;

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
        Image.asset("assets/images/ui/title_bar.png"),
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
        padding: const EdgeInsets.only(top: 5, right: 5),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.end,
          children: [
            _timerLabel(),
            _gameStartOrPauseButton(),
            _gameOverButton(),
            _gameSettingButton(),
          ],
        ));
  }

  Widget _timerLabel() {
    return const Text("14:30", style: TextStyle(color: Colors.white));
  }

  Widget _gameStartOrPauseButton() {
    return IconButton(
        icon: const Icon(Icons.pause, color: Colors.white), onPressed: () {});
  }

  Widget _gameOverButton() {
    return IconButton(
        icon: const Icon(Icons.wrong_location, color: Colors.white),
        onPressed: () {});
  }

  Widget _gameSettingButton() {
    return TextButton(
        child: const Text("比赛设置", style: TextStyle(color: Colors.white)),
        onPressed: () {});
  }

  Widget _effectButtonGroup() {
    return Padding(
        padding: const EdgeInsets.only(top: 10),
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
        padding:
            const EdgeInsets.only(top: 10, left: 20, right: 20, bottom: 10),
        child: Row(
          children: const [
            Expanded(flex: 1, child: GameSettingView(player: "left")),
            SizedBox(width: 10),
            Expanded(flex: 1, child: GameSettingView(player: "right")),
          ],
        ),
      ),
    );
  }

  void _inititialize() async {
    _sender = await UDP.bind(Endpoint.any(port: const Port(2000)));
  }

  void _sendStringMessage(String? message) async {
    _message = message;
    debugPrint("execute command: $message");
    _dataLength = await _sender?.send(
        message!.codeUnits, Endpoint.broadcast(port: const Port(1000)));
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
