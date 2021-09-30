import 'package:flutter/material.dart';

import '../data/command_data.dart';
import '../ui_component/pop_button.dart';

class TextToggle extends StatefulWidget {
  final List<ButtonData> buttons;
  final double buttonWidth;
  final double buttonHeight;
  final double unselectedWidthDiff;
  final double unselectedHeightDiff;
  final double spacing;
  final MainAxisSize mainAxisSize;
  final MainAxisAlignment mainAxisAlignment;
  final int defaultButtonIndex;
  final Function(GameEvent) onValueChanged;

  const TextToggle(
      this.buttons, this.buttonWidth, this.buttonHeight, this.onValueChanged,
      {Key? key,
      this.unselectedWidthDiff = 0,
      this.unselectedHeightDiff = 0,
      this.defaultButtonIndex = 0,
      this.spacing = 0,
      this.mainAxisSize = MainAxisSize.max,
      this.mainAxisAlignment = MainAxisAlignment.start})
      : super(key: key);

  @override
  TextToggleState createState() => TextToggleState();
}

class TextToggleState extends State<TextToggle> {
  GameEvent? _selectedValue;

  @override
  void initState() {
    super.initState();
    refresh();
  }

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisSize: widget.mainAxisSize,
      mainAxisAlignment: widget.mainAxisAlignment,
      children: widget.buttons
          .map((e) => Row(children: [
                TExtButton(e.value, e.text, "text_button",
                    e.value == _selectedValue, e.color, _onButtonPressed,
                    width: widget.buttonWidth, height: widget.buttonHeight),
                Visibility(
                    visible:
                        widget.buttons.indexOf(e) < widget.buttons.length - 1,
                    child: SizedBox(width: widget.spacing)),
              ]))
          .toList(),
    );
  }

  void _onButtonPressed(GameEvent value) {
    setState(() {
      _selectedValue = value;
      _onValueChanged();
      // debugPrint("text button [$_selectedValue] pressed");
    });
  }

  void _onValueChanged() {
    if (_selectedValue != null) {
      widget.onValueChanged(_selectedValue!);
    }
  }

  void refresh() {
    _selectedValue = widget.buttons.isNotEmpty &&
            widget.defaultButtonIndex >= 0 &&
            widget.defaultButtonIndex < widget.buttons.length
        ? widget.buttons[widget.defaultButtonIndex].value
        : GameEvent.end;
    _onValueChanged();
  }
}

class TExtButton extends StatelessWidget {
  final GameEvent value;
  final String text;
  final String iconName;
  final bool selected;
  final Color color;
  final double width;
  final double height;
  final double unselectedWidthDiff;
  final double unselectedHeightDiff;
  final Function(GameEvent) onPressed;

  const TExtButton(this.value, this.text, this.iconName, this.selected,
      this.color, this.onPressed,
      {Key? key,
      this.width = 150,
      this.height = 45,
      this.unselectedWidthDiff = 0,
      this.unselectedHeightDiff = 0})
      : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Container(
      // color: Colors.green,
      width: width,
      height: height,
      child: TextButton(
          style: ButtonStyle(
            //圆角
            shape: MaterialStateProperty.all(
                RoundedRectangleBorder(borderRadius: BorderRadius.circular(4))),
            //边框
            side: MaterialStateProperty.all(
              BorderSide(
                  color: selected
                      ? color.withAlpha(200)
                      : const Color.fromRGBO(183, 183, 183, 1),
                  width: 1.5),
            ),
            //背景
            backgroundColor: MaterialStateProperty.all(
                selected ? color.withAlpha(150) : Colors.transparent),
          ),
          child: Text(text,
              style: TextStyle(color: selected ? Colors.white : Colors.black)),
          onPressed: () {
            onPressed(value);
            debugPrint("button[$text] press down: $selected");
          }),
    );
  }
}

class ButtonData {
  String text;
  GameEvent value;
  Color color;

  ButtonData({required this.text, required this.value, required this.color});
}
