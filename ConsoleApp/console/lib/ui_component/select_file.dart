import 'dart:io';
import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

typedef OnSelected = void Function(File file);

class SelectImage extends StatefulWidget {
  final OnSelected onSelected;
  const SelectImage(this.onSelected, {Key? key}) : super(key: key);

  @override
  State<StatefulWidget> createState() {
    return SelectImageState();
  }
}

class SelectImageState extends State<SelectImage> {
  String? _avatarPath;

  void _openSelectFileWindow() async {
    final fileResult =
        await FilePicker.platform.pickFiles(type: FileType.image);
    if (fileResult != null && fileResult.paths.first != null) {
      final path = fileResult.paths.first;
      widget.onSelected(File(path!));
      setState(() {
        _avatarPath = path;
      });
    }
  }

  void reset() {
    _avatarPath = null;
  }

  Widget get selectButton {
    return IconButton(
        icon: _avatarPath == null
            ? Image.asset("assets/images/ui/profile.png", width: 32, height: 32)
            : Image.file(File(_avatarPath!), width: 32, height: 32),
        onPressed: _openSelectFileWindow);
  }

  @override
  Widget build(BuildContext context) {
    return Column(mainAxisSize: MainAxisSize.min, children: [
      selectButton,
    ]);
  }
}
