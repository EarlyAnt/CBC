import 'package:flutter/material.dart';

class GameSettingView extends StatefulWidget {
  final String? direction;

  const GameSettingView({Key? key, this.direction}) : super(key: key);

  @override
  _GameSettingViewState createState() => _GameSettingViewState();
}

class _GameSettingViewState extends State<GameSettingView> {
  final double _spacing = 10;
  TextEditingController? _nameController;
  TextEditingController? _healthController;
  String? _avatarPath;
  String? _playerName;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController();
    _healthController = TextEditingController();
  }

  @override
  void dispose() {
    _nameController?.dispose();
    _healthController?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return _consolePanel();
  }

  Widget _consolePanel() {
    return Container(
      decoration: BoxDecoration(
        border: Border.all(
            color: widget.direction == "left" ? Colors.blue : Colors.red,
            width: 2),
        borderRadius: const BorderRadius.all(Radius.circular(4)),
      ),
      padding: const EdgeInsets.all(5),
      child: SingleChildScrollView(
        child: Column(
          children: [
            _playerInfo(),
            _playerHealth(),
            _playerStatus(),
            _cardStatus(),
            _hurtStatus(),
          ],
        ),
      ),
    );
  }

  Widget _playerInfo() {
    return Row(children: [
      Image.asset(_avatarPath ?? "assets/images/ui/avatar_1.jpg",
          width: 32, height: 32),
      Padding(
          padding: EdgeInsets.only(left: _spacing),
          child: Text(_playerName ?? "未设置")),
      Padding(
          padding: EdgeInsets.only(left: _spacing),
          child: TextButton(child: const Text("设定"), onPressed: () {})),
    ]);
  }

  Widget _playerHealth() {
    return Row(
      children: [
        const Text("英雄当前体力"),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: Container(
              width: 80,
              height: 30,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey, width: 2),
                borderRadius: const BorderRadius.all(Radius.circular(4)),
              ),
              child: TextFormField(
                  controller: _healthController,
                  showCursor: false,
                  decoration: const InputDecoration(border: InputBorder.none)),
            )),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("设定"), onPressed: () {})),
      ],
    );
  }

  Widget _playerStatus() {
    return Row(
      children: [
        const Text("状态加成"),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("衰弱"), onPressed: () {})),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("支援"), onPressed: () {})),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("效果"), onPressed: () {})),
      ],
    );
  }

  Widget _cardStatus() {
    return Row(
      children: [
        const Text("当前牌库"),
        Padding(
            padding: EdgeInsets.only(left: _spacing), child: const Text("30")),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("+"), onPressed: () {})),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("-"), onPressed: () {})),
      ],
    );
  }

  Widget _hurtStatus() {
    return Row(
      children: [
        const Text("当前伤害区"),
        Padding(
            padding: EdgeInsets.only(left: _spacing), child: const Text("5")),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("+"), onPressed: () {})),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(child: const Text("-"), onPressed: () {})),
      ],
    );
  }
}
