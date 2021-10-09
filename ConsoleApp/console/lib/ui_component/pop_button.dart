import 'package:flutter/material.dart';

class PopButton extends StatefulWidget {
  final String text;
  final bool light;
  final Color color;
  final Function(bool) onPressed;

  const PopButton(this.text,
      {required this.light,
      required this.color,
      required this.onPressed,
      Key? key})
      : super(key: key);

  @override
  PopButtonState createState() => PopButtonState();
}

class PopButtonState extends State<PopButton> {
  bool? _pressDown;

  @override
  void initState() {
    super.initState();
    _pressDown = widget.light;
  }

  @override
  Widget build(BuildContext context) {
    return TextButton(
        style: ButtonStyle(
          //圆角
          shape: MaterialStateProperty.all(
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(4))),
          //边框
          side: MaterialStateProperty.all(
            BorderSide(
                color: _pressDown!
                    ? widget.color.withAlpha(200)
                    : const Color.fromRGBO(183, 183, 183, 1),
                width: 1.5),
          ),
          //背景
          backgroundColor: MaterialStateProperty.all(
              _pressDown! ? widget.color.withAlpha(150) : Colors.transparent),
        ),
        child: Text(widget.text,
            style: TextStyle(color: _pressDown! ? Colors.white : Colors.black)),
        onPressed: () {
          setState(() {
            _pressDown = !_pressDown!;
            widget.onPressed(_pressDown!);
          });
          debugPrint("button[${widget.text}] press down: $_pressDown");
        });
  }

  void reset() {
    setState(() {
      _pressDown = false;
    });
  }
}
