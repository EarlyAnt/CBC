class PlayerData {
  String name;
  int health;
  int cardCount;
  int hurt;
  bool weak;
  bool aid;
  bool effect;

  PlayerData(
      {required this.name,
      required this.health,
      required this.cardCount,
      required this.hurt,
      required this.weak,
      required this.aid,
      required this.effect});

  static PlayerData get empty => PlayerData(
      name: "未设置",
      health: 10000,
      cardCount: 40,
      hurt: 0,
      weak: false,
      aid: false,
      effect: false);
}
