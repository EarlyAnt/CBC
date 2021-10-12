// ignore_for_file: avoid_print

import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:qiniu_flutter_sdk/qiniu_flutter_sdk.dart';

import '/data/command_data.dart';
import '/data/player_data.dart';
import '/ui_component/pop_button.dart';
import '/utils/dio_util.dart';
import '/utils/uint.dart';

import '/plugins/udp/udp.dart';
import 'select_file.dart';

class GameSettingView extends StatefulWidget {
  final String? player;

  const GameSettingView({Key? key, this.player}) : super(key: key);

  @override
  GameSettingViewState createState() => GameSettingViewState();
}

class GameSettingViewState extends State<GameSettingView> {
  GameSettingViewState() : storage = Storage();

  final double _spacing = 10;
  final GlobalKey<PopButtonState> _weakButtonKey = GlobalKey();
  final GlobalKey<PopButtonState> _aidButtonKey = GlobalKey();
  final GlobalKey<PopButtonState> _effectButtonKey = GlobalKey();
  final Storage storage;
  final int partSize = 4;
  late final UDP? _sender;
  PutController? _putController;
  TextEditingController? _nameController;
  TextEditingController? _healthController;
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
      SelectImage(onSelectedFile),
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
    int? dataLength = await _sender?.send(
        message!.codeUnits, Endpoint.broadcast(port: const Port(1000)));

    debugPrint("execute command: $message, length: $dataLength");
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

  void onSelectedFile(File file) {
    print('选中文件: ${file.path}');
    print('文件尺寸：${humanizeFileSize(file.lengthSync().toDouble())}');

    setState(() {
      uploadFile(file);
    });
  }

  Future uploadFile(File file) async {
    try {
      Map body = {};
      body["filepath"] = file.path.substring(file.path.lastIndexOf('/') + 1);

      DioUtils.postHttp(
        'https://collector.kayou.gululu.com/api/qn/token',
        parameters: body,
        onSuccess: (data) {
          debugPrint("<><main.getToken>success: $data");
          Map jsonObject = json.decode(data.toString());
          // debugPrint("****  token: ${jsonObject['data']['token']}");
          // debugPrint("****  key: ${jsonObject['data']['key']}");
          // debugPrint("****  url: ${jsonObject['data']['url']}");
          startUpload(
            jsonObject['data']['token'],
            jsonObject['data']['key'],
            jsonObject['data']['url'],
            file,
          );
        },
        onError: (errorText) {
          debugPrint("<><main.getToken>error: $errorText");
        },
      );
    } catch (e) {
      debugPrint("<><main.getToken>exception: $e");
    }
  }

  void startUpload(String token, String key, String url, File? file) {
    print('创建 PutController');
    _putController = PutController();

    if (token.isEmpty) {
      print('token 不能为空');
      return;
    }

    if (file == null) {
      print('请选择文件');
      return;
    }

    print('开始上传文件');
    debugPrint('url: $url, token: $token');
    storage.putFile(
      file,
      token,
      options:
          PutOptions(key: key, partSize: partSize, controller: _putController),
    )
      ..then((PutResponse response) {
        print('上传已完成: 原始响应数据: ${jsonEncode(response.rawData)}');
        print('------------------------');
        _sendStringMessage(CommandUtil.buildAvatarCommand(
            file.path.substring(file.path.lastIndexOf('/') + 1),
            widget.player!));
      })
      ..catchError((dynamic error) {
        if (error is StorageError) {
          switch (error.type) {
            case StorageErrorType.CONNECT_TIMEOUT:
              print('发生错误: 连接超时');
              break;
            case StorageErrorType.SEND_TIMEOUT:
              print('发生错误: 发送数据超时');
              break;
            case StorageErrorType.RECEIVE_TIMEOUT:
              print('发生错误: 响应数据超时');
              break;
            case StorageErrorType.RESPONSE:
              print('发生错误: ${error.message}');
              break;
            case StorageErrorType.CANCEL:
              print('发生错误: 请求取消');
              break;
            case StorageErrorType.UNKNOWN:
              print('发生错误: 未知错误');
              break;
            case StorageErrorType.NO_AVAILABLE_HOST:
              print('发生错误: 无可用 Host');
              break;
            case StorageErrorType.IN_PROGRESS:
              print('发生错误: 已在队列中');
              break;
          }
        } else {
          print('发生错误: ${error.toString()}');
        }

        print('------------------------');
      });
  }
}
