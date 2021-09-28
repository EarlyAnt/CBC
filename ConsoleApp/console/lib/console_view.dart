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
    EffectButtonData("UR卡", "ur"),
    EffectButtonData("CR卡", "cr"),
    EffectButtonData("掌声", "zhangsheng"),
    EffectButtonData("欢呼", "huanhu"),
    EffectButtonData("UR卡", "ur"),
    EffectButtonData("CR卡", "cr"),
    EffectButtonData("掌声", "zhangsheng"),
    EffectButtonData("欢呼", "huanhu"),
    EffectButtonData("UR卡", "ur"),
    EffectButtonData("CR卡", "cr"),
    EffectButtonData("掌声", "zhangsheng"),
    EffectButtonData("欢呼", "huanhu"),
  ];
  Size? _screenSize;

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
          //Todo: 发送命令
          print("execute command: ${effectButtonData.effectCommand}");
        });
  }

  Widget _consoleZone() {
    return Expanded(
      child: Padding(
        padding:
            const EdgeInsets.only(top: 10, left: 20, right: 20, bottom: 10),
        child: Container(
          // color: Colors.lightGreenAccent,
          child: Row(
            children: [
              Expanded(
                  flex: 1,
                  child: Container(
                      decoration: BoxDecoration(
                        border: Border.all(color: Colors.blue, width: 2),
                        borderRadius:
                            const BorderRadius.all(Radius.circular(4)),
                      ),
                      child: _consolePanel(
                          "left", "assets/images/ui/avatar_2.jpg", "韩菱纱"))),
              SizedBox(width: 10),
              Expanded(
                  flex: 1,
                  child: Container(
                      decoration: BoxDecoration(
                        border: Border.all(color: Colors.red, width: 2),
                        borderRadius:
                            const BorderRadius.all(Radius.circular(4)),
                      ),
                      child: _consolePanel(
                          "right", "assets/images/ui/avatar_3.jpg", "蕾安娜"))),
            ],
          ),
        ),
      ),
    );
  }

  Widget _consolePanel(String role, String avatarPath, String name) {
    return Column(
      children: [
        _playerInfo(avatarPath, name),
        _playerHealth(),
        _playerStatus(),
        _cardStatus(),
        _hurtStatus(),
      ],
    );
  }

  Widget _playerInfo(String avatarPath, String name) {
    return Row(children: [
      Image.asset(avatarPath, width: 32, height: 32),
      Text(name),
    ]);
  }

  Widget _playerHealth() {
    return Row(
      children: [
        const Text("英雄当前体力"),
        // const TextField(),
        TextButton(child: const Text("设定"), onPressed: () {}),
      ],
    );
  }

  Widget _playerStatus() {
    return Row(
      children: [
        const Text("状态加成"),
        TextButton(child: const Text("衰弱"), onPressed: () {}),
        TextButton(child: const Text("支援"), onPressed: () {}),
        TextButton(child: const Text("效果"), onPressed: () {}),
      ],
    );
  }

  Widget _cardStatus() {
    return Row(
      children: [
        const Text("当前牌库"),
        const Text("30"),
        TextButton(child: const Text("+"), onPressed: () {}),
        TextButton(child: const Text("-"), onPressed: () {}),
      ],
    );
  }

  Widget _hurtStatus() {
    return Row(
      children: [
        const Text("当前伤害区"),
        const Text("5"),
        TextButton(child: const Text("+"), onPressed: () {}),
        TextButton(child: const Text("-"), onPressed: () {}),
      ],
    );
  }
}

class EffectButtonData {
  String buttonText;
  String effectCommand;

  EffectButtonData(this.buttonText, this.effectCommand);
}
