using System.ComponentModel;

public class Strings {
    public enum Settings { BulletImpactEffects, MasterVolume, SFXVolume, MusicVolume };

    public enum Controls { Fire1, Fire2, Vertical, Horizontal, Sprint, Action, SwitchFiringMode };

    public enum AnimatorSettings { speedPercent };

    public enum BulletImpactEffect { Full, Simple, None };

    public enum Message { //unused
        [Description(" doors unlocked")]DoorsUnlocked,
        [Description(" door unlocked")]DoorUnlocked,
        [Description("This door is locked")]DoorIsLocked,
        [Description("Switched to automatic firing mode")]SwitchedToAutoFire,
        [Description("Switched to single firing mode")]SwitchedToSingleFire,
    }

    // internal struct DisplayMessage : DescriptionAttribute {
    //     public string message;

    //     public DisplayMessage(string _message) {
    //         message = _message;
    //     }

    // }
}