import 'package:console/data/command_data.dart';
import 'package:console/data/player_data.dart';
import 'package:console/ui_component/pop_button.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../plugins/udp/udp.dart';

class GameSettingView extends StatefulWidget {
  final String? player;

  const GameSettingView({Key? key, this.player}) : super(key: key);

  @override
  GameSettingViewState createState() => GameSettingViewState();
}

class GameSettingViewState extends State<GameSettingView> {
  final double _spacing = 10;
  final GlobalKey<PopButtonState> _weakButtonKey = GlobalKey();
  final GlobalKey<PopButtonState> _aidButtonKey = GlobalKey();
  final GlobalKey<PopButtonState> _effectButtonKey = GlobalKey();
  TextEditingController? _nameController;
  TextEditingController? _healthController;
  String? _avatarPath;
  UDP? _sender;
  String? _message;
  int? _dataLength;
  PlayerData? _playerData;

  @override
  void initState() {
    super.initState();
    _playerData = PlayerData.empty;
    _nameController = TextEditingController();
    _nameController?.text = _playerData!.name;
    _healthController = TextEditingController();
    _healthController?.text = _playerData!.health.toString();
    _initSocket();
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
            color: widget.player == "left" ? Colors.blue : Colors.red,
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
        child: Container(
            width: 132,
            height: 30,
            decoration: BoxDecoration(
              border: Border.all(color: Colors.grey, width: 2),
              borderRadius: const BorderRadius.all(Radius.circular(4)),
            ),
            child: TextField(
                controller: _nameController,
                maxLength: 10,
                maxLines: 1,
                showCursor: false,
                decoration: const InputDecoration(
                  border: InputBorder.none,
                  counterText: '',
                ),
                onChanged: (value) {
                  _playerData!.name = value;
                })),
      ),
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
                maxLines: 1,
                showCursor: false,
                decoration: const InputDecoration(border: InputBorder.none),
                inputFormatters: [
                  FilteringTextInputFormatter(RegExp("[0-9]"), //限制只允许输入数字
                      // RegExp("[a-z,A-Z,0-9]"), //限制只允许输入字母和数字
                      allow: true),
                ],
                onChanged: (value) {
                  int? cardCount = int.tryParse(value);
                  if (cardCount != null) {
                    _playerData!.health = cardCount;
                  }
                },
              ),
            )),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(
                child: const Text("设定"),
                onPressed: () {
                  _sendStringMessage(CommandUtil.buildHealthCommand(
                      _playerData!.health, widget.player!));
                })),
      ],
    );
  }

  Widget _playerStatus() {
    return Row(
      children: [
        const Text("状态加成"),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: PopButton("衰弱",
                key: _weakButtonKey,
                light: false,
                color: Colors.red, onPressed: (value) {
              setState(() {
                _playerData!.weak = value;
              });
              _sendStringMessage(CommandUtil.buildWeakCommand(
                  _playerData!.weak, widget.player!));
            })),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: PopButton("被支援",
                key: _aidButtonKey,
                light: false,
                color: Colors.green, onPressed: (value) {
              setState(() {
                _playerData!.aid = value;
              });
              _sendStringMessage(CommandUtil.buildAidCommand(
                  _playerData!.aid, widget.player!));
            })),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: PopButton("效果",
                key: _effectButtonKey,
                light: false,
                color: Colors.blue, onPressed: (value) {
              setState(() {
                _playerData!.effect = value;
              });
              _sendStringMessage(CommandUtil.buildEffectCommand(
                  _playerData!.effect, widget.player!));
            })),
      ],
    );
  }

  Widget _cardStatus() {
    return Row(
      children: [
        const Text("当前牌库"),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: Text("${_playerData!.cardCount}")),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(
                child: const Text("+"),
                onPressed: () {
                  setState(() {
                    _playerData!.cardCount += 1;
                  });
                  _sendStringMessage(CommandUtil.buildCardCommand(
                      _playerData!.cardCount, widget.player!));
                })),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(
                child: const Text("-"),
                onPressed: () {
                  if (_playerData!.cardCount > 0) {
                    setState(() {
                      _playerData!.cardCount -= 1;
                    });
                    _sendStringMessage(CommandUtil.buildCardCommand(
                        _playerData!.cardCount, widget.player!));
                  }
                })),
      ],
    );
  }

  Widget _hurtStatus() {
    return Row(
      children: [
        const Text("当前伤害区"),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: Text("${_playerData!.hurt}")),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(
                child: const Text("+"),
                onPressed: () {
                  if (_playerData!.hurt < 5) {
                    setState(() {
                      _playerData!.hurt += 1;
                    });
                    _sendStringMessage(CommandUtil.buildHurtCommand(
                        _playerData!.hurt, widget.player!));
                  }
                })),
        Padding(
            padding: EdgeInsets.only(left: _spacing),
            child: TextButton(
                child: const Text("-"),
                onPressed: () {
                  if (_playerData!.hurt > 0) {
                    setState(() {
                      _playerData!.hurt -= 1;
                    });
                    _sendStringMessage(CommandUtil.buildHurtCommand(
                        _playerData!.hurt, widget.player!));
                  }
                })),
      ],
    );
  }

  void _initSocket() async {
    _sender = await UDP.bind(Endpoint.any(port: const Port(2000)));
  }

  void _sendStringMessage(String? message) async {
    _message = message;
    debugPrint("execute command: $message");
    _dataLength = await _sender?.send(
        message!.codeUnits, Endpoint.broadcast(port: const Port(1000)));
  }

  void reset() {
    setState(() {
      _playerData = PlayerData.empty;
      _nameController?.text = _playerData!.name;
      _healthController?.text = _playerData!.health.toString();
      _weakButtonKey.currentState?.reset();
      _aidButtonKey.currentState?.reset();
      _effectButtonKey.currentState?.reset();
    });
  }
}
