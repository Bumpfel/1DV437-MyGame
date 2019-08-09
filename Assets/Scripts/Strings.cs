using System.ComponentModel;

public class Strings {
    public enum Settings { BulletImpactEffects, MasterVolume, SFXVolume, MusicVolume };

    public enum Controls { Fire1_Player, Fire2_Player, Vertical_Player, Horizontal_Player, Sprint_Player, Action_Player };

    public enum AnimatorSettings { speedPercent };

    public enum BulletImpactEffect { Full, Simple, None };

    public enum Message {
        [Description(" doors unlocked")]DoorsUnlocked,
        [Description(" door unlocked")]DoorUnlocked,
        [Description("This door is locked")]DoorIsLocked,

    }

    // private class DisplayMessage : EnumAttr {
    //     public string message;

    //     public DisplayMessage(string _message) {
    //         message = _message;
    //     }

    // }
}