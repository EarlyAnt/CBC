class CommandUtil {
  static String buildHealthCommand(int health) {
    return "{\"Tag\":\"health\",\"Data\":{\"Value\":$health,\"DataOwner\":\"left\"}}";
  }

  static String buildCardCommand(int cardCount) {
    return "{\"Tag\":\"card\",\"Data\":{\"Value\":$cardCount,\"DataOwner\":\"left\"}}";
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