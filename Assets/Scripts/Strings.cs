using System.Collections.Generic;
using System.ComponentModel;

public static class Strings {
    private static Dictionary<int, string> LevelNames = new Dictionary<int, string>();

    static Strings() {
        LevelNames.Add(0, ""); // (menu)
        LevelNames.Add(1, "Space Station");
        LevelNames.Add(2, "Research Facility");
        LevelNames.Add(3, "Warehouse");
    }

    public static string GetLevelName(int index) {
        if(index > 0 && index < LevelNames.Count)
            return LevelNames[index];
        return null;
    }
}

public enum Settings { BulletImpactEffects, MasterVolume, SFXVolume, MusicVolume };

public enum Controls { Fire1, Fire2, Vertical, Horizontal, Sprint, Action, SwitchFiringMode, FreeLook };

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

public enum ExposedMixerGroup { MasterVolume, SFXVolume, MusicVolume };