using System.Collections.Generic;
using System.ComponentModel;

public static class Strings {
    private static Dictionary<int, string> LevelNames = new Dictionary<int, string>();
    private static Dictionary<Message, string> Messages = new Dictionary<Message, string>();

    static Strings() {
        LevelNames.Add(0, ""); // (menu)
        LevelNames.Add(1, "Space Station");
        LevelNames.Add(2, "Research Facility");
        LevelNames.Add(3, "Warehouse");

        Messages.Add(Message.DoorsUnlocked, " doors unlocked");
        Messages.Add(Message.DoorUnlocked, " door unlocked");
        Messages.Add(Message.DoorLocked, "Door is locked");
        Messages.Add(Message.SwitchedToAutoFire, "Switched to automatic firing mode");
        Messages.Add(Message.SwitchedToSingleFire, "Switched to single firing mode");
    }

    public static string GetLevelName(int index) {
        if(index > 0 && index < LevelNames.Count)
            return LevelNames[index];
        return null;
    }

    public static string GetMessage(Message message) {
        return Messages[message];
    }
}

public enum Settings { BulletImpactEffects, CameraControl, MovemenControl, MasterVolume, SFXVolume, MusicVolume }

public enum Controls { Fire1, Fire2, Vertical, Horizontal, Sprint, Action, SwitchFiringMode, CameraSpecial }

public enum AnimatorSettings { speedPercent }

public enum BulletImpactEffect { Full, Simple, None }

public enum Message { DoorsUnlocked, DoorUnlocked, DoorLocked, SwitchedToAutoFire, SwitchedToSingleFire }

public enum ExposedMixerGroup { MasterVolume, SFXVolume, MusicVolume }