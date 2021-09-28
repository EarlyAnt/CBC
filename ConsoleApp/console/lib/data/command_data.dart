class CommandUtil {
  static String buildHealthCommand(int health, String player) {
    return "{\"Tag\":\"health\",\"Data\":{\"Value\":$health,\"DataOwner\":\"$player\"}}";
  }

  static String buildCardCommand(int cardCount, String player) {
    return "{\"Tag\":\"card\",\"Data\":{\"Value\":$cardCount,\"DataOwner\":\"$player\"}}";
  }

  static String buildHurtCommand(int hurt, String player) {
    return "{\"Tag\":\"hurt\",\"Data\":{\"Value\":$hurt,\"DataOwner\":\"$player\"}}";
  }

  static String buildAudioCommand(String audio) {
    return "{\"Tag\":\"audio\",\"Data\":{\"EffectFile\":\"$audio\"}}";
  }

  static String buildAnimationCommand(String animation) {
    return "{\"Tag\":\"animation\",\"Data\":{\"EffectFile\":\"$animation\"}}";
  }
}

/*
  * command list:
  * {\"Tag\":\"health\",\"Data\":{\"Value\":5500,\"DataOwner\":\"left\"}}
  * {\"Tag\":\"card\",\"Data\":{\"Value\":29,\"DataOwner\":\"left\"}}
  * {\"Tag\":\"audio\",\"Data\":{\"EffectFile\":\"guzhang\"}}
  * {\"Tag\":\"animation\",\"Data\":{\"EffectFile\":\"cr\"}}
  * {\"Tag\":\"avatar\",\"Data\":{\"Bytes\":\"255,216,255,224,0,16,74,70,73,70,...\"}}
 */