import 'package:flutter/material.dart';

import '../data/common_data.dart';

class GameOverDialog extends StatefulWidget {
  GameOverDialog({Key? key}) : super(key: key);

  @override
  _GameOverDialogState createState() => _GameOverDialogState();

  static Future<DialogResult> show(context) async {
    return await showDialog(
        context: context,
        barrierDismissible: false,
        builder: (ctx) => Dialog(
              insetPadding: EdgeInsets.zero,
              backgroundColor: Colors.white,
              elevation: 5,
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.all(Radius.circular(10))),
              child: GameOverDialog(),
            ));
  }
}

class _GameOverDialogState extends State<GameOverDialog> {
  bool _clearData = false;

  @override
  Widget build(BuildContext context) {
    final Size screenSize = MediaQuery.of(context).size;
    final TextStyle? subtitleStyle = Theme.of(context)
        .textTheme
        .subtitle2
        ?.copyWith(fontSize: 16, fontWeight: FontWeight.bold);
    final TextStyle? bodyTextStyle = Theme.of(context)
        .textTheme
        .bodyText2
        ?.copyWith(fontSize: 16, color: Colors.grey);

    return UnconstrainedBox(
        constrainedAxis: Axis.vertical,
        child: SizedBox(
            width: screenSize.width * 0.42,
            height: screenSize.height * 0.5,
            child: Container(
              child: Column(
                mainAxisSize: MainAxisSize.max,
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Padding(
                      padding: EdgeInsets.only(top: 10),
                      child: _txtTitle(subtitleStyle)),
                  Padding(
                      padding: EdgeInsets.only(top: 5, left: 10, right: 10),
                      child: _imgUnderLine()),
                  Expanded(
                    child: Center(
                        child: Column(
                            mainAxisAlignment: MainAxisAlignment.center,
                            children: [
                          _txtTip(context, bodyTextStyle),
                          SizedBox(height: 10),
                          _chkClearData(bodyTextStyle),
                        ])),
                  ),
                  Padding(
                    padding: EdgeInsets.symmetric(horizontal: 10),
                    child: _buttonGroup(bodyTextStyle),
                  ),
                ],
              ),
            )));
  }

  Widget _txtTitle(TextStyle? textStyle) {
    return Text('结束游戏', textAlign: TextAlign.center, style: textStyle);
  }

  Widget _txtTip(BuildContext context, TextStyle? textStyle) {
    return Container(
        width: double.infinity,
        // color: Colors.lightGreenAccent,
        alignment: Alignment.center,
        child: Text('请选择获胜方，或者直接结束游戏',
            textAlign: TextAlign.center, style: textStyle));
  }

  Widget _imgUnderLine() {
    return Divider(color: Colors.grey.shade300, thickness: 2);
  }

  Widget _chkClearData(TextStyle? textStyle) {
    return Row(
      mainAxisSize: MainAxisSize.max,
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Checkbox(
          value: _clearData,
          visualDensity: VisualDensity(
              horizontal: VisualDensity.minimumDensity,
              vertical: VisualDensity.minimumDensity),
          checkColor: Colors.white,
          activeColor: Colors.blue,
          fillColor: MaterialStateProperty.all(Colors.blue),
          onChanged: (value) {
            setState(() {
              _clearData = !_clearData;
            });
          },
        ),
        Text('是否清空数据', style: textStyle?.copyWith(fontSize: 14)),
      ],
    );
  }

  Widget _buttonGroup(TextStyle? textStyle) {
    final double buttonPadding = 5;
    return Row(
      mainAxisSize: MainAxisSize.max,
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Padding(
            padding: EdgeInsets.only(
                top: 0, left: buttonPadding, right: buttonPadding),
            child: _btnBlueWin(textStyle, context)),
        Padding(
            padding: EdgeInsets.only(
                top: 0, left: buttonPadding, right: buttonPadding),
            child: _btnRedWin(textStyle, context)),
        Padding(
            padding: EdgeInsets.only(
                top: 0, left: buttonPadding, right: buttonPadding),
            child: _btnNoWin(textStyle, context)),
      ],
    );
  }

  Widget _btnBlueWin(TextStyle? textStyle, BuildContext context) {
    return TextButton(
      child: Text('蓝方'),
      onPressed: () {
        Navigator.of(context)
            .pop(DialogResult(winner: Winner.blue, clearData: _clearData));
      },
    );
  }

  Widget _btnRedWin(TextStyle? textStyle, BuildContext context) {
    return TextButton(
      style: Theme.of(context).textButtonTheme.style?.copyWith(
          backgroundColor: MaterialStateProperty.all(Colors.red[400])),
      child: Text('红方'),
      onPressed: () {
        Navigator.of(context)
            .pop(DialogResult(winner: Winner.red, clearData: _clearData));
      },
    );
  }

  Widget _btnNoWin(TextStyle? textStyle, BuildContext context) {
    return TextButton(
      style: Theme.of(context)
          .textButtonTheme
          .style
          ?.copyWith(backgroundColor: MaterialStateProperty.all(Colors.grey)),
      child: Text('直接结束'),
      onPressed: () {
        Navigator.of(context)
            .pop(DialogResult(winner: Winner.none, clearData: _clearData));
      },
    );
  }
}

class DialogResult {
  Winner? winner;
  bool? clearData;

  DialogResult({required this.winner, required this.clearData});
}
