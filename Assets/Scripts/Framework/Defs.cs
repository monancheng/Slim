using DarkTonic.MasterAudio;
using UnityEngine;

public struct Defs
{
    public static readonly string AndroidAppID = "com.crazylabs.monsteryumm";
    public static readonly string iOSApp_ID = "id1192223024";

    public static Share ShareVoxel;
    public static Rate Rate;

    public static int Volume;
    
    public static void MuteSounds(bool flag)
    {
        if (flag) {
            MasterAudio.MuteEverything();
            Volume = 0;
        }
        else
        {
            MasterAudio.UnmuteEverything();
            Volume = 1;
        }
        PlayerPrefs.SetInt("SoundVolume", 1);
    }

    public static void SwitchSounds()
    {
        if (Volume == 0)
        {
            Volume = 1;
            MasterAudio.MuteEverything();
            D.Log("Sound OFF");
        }
        else
        {
            Volume = 0;
            MasterAudio.UnmuteEverything();
            D.Log("Sound ON");
        }
        
        PlayerPrefs.SetInt("SoundVolume", 0);
    }
}