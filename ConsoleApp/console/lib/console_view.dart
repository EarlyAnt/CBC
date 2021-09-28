import 'package:flutter/material.dart';

class ConsoleView extends StatefulWidget {
  const ConsoleView({Key? key}) : super(key: key);

  @override
  _ConsoleViewState createState() => _ConsoleViewState();
}

class _ConsoleViewState extends State<ConsoleView> {
  final List<EffectButtonData> _effectButtonDatas = [
    EffectButtonData("UR卡", "ur"),
    EffectButtonData("CR卡", "cr"),
    EffectButtonData("掌声", "zhangsheng"),
    EffectButtonData("欢呼", "huanhu"),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(children: [
        Image.asset("assets/images/ui/title_bar.png"),
        Column(children: [
          Padding(
              padding: const EdgeInsets.only(top: 5, right: 5),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  _timerLabel(),
                  _gameStartOrPauseButton(),
                  _gameOverButton(),
                  _gameSettingButton(),
                ],
              )),
          Padding(
              padding: const EdgeInsets.only(top: 20),
              child: Column(
                children: [
                  Row(mainAxisAlignment: MainAxisAlignment.start, children: [
                    Padding(
                        padding: const EdgeInsets.only(left: 20),
                        child: _effectZoneLabel())
                  ]),
                  Row(mainAxisAlignment: MainAxisAlignment.start, children: [
                    Padding(
                        padding: const EdgeInsets.only(left: 20),
                        child: Wrap(
                          // alignment: WrapAlignment.start,
                          // crossAxisAlignment: WrapCrossAlignment.start,
                          spacing: 10,
                          children: _effectButtonDatas
                              .map((e) => _effectButton(e))
                              .toList(),
                        )),
                  ]),
                ],
              )),
          Expanded(
            flex: 1,
            child: Padding(
              padding: const EdgeInsets.only(top: 20),
              child: Row(
                children: [
                  Expanded(
                      flex: 1,
                      child: Container(
                          color: Colors.blue.shade500,
                          child: const Text("左控制区"))),
                  Expanded(
                      flex: 1,
                      child: Container(
                          color: Colors.red.shade500,
                          child: const Text("右控制区"))),
                ],
              ),
            ),
          ),
        ]),
      ]),
    );
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

  Widget _effectZoneLabel() {
    return const Text("特效区");
  }

  Widget _effectButton(EffectButtonData effectButtonData) {
    return TextButton(
        child: Text(effectButtonData.buttonText),
        onPressed: () {
          //Todo: 发送命令
          print("execute command: ${effectButtonData.effectCommand}");
        });
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
