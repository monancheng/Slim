using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Sprite spriteOff;
    public Sprite spriteOn;
    // Use this for initialization

    private void Start()
    {
        Defs.Volume = PlayerPrefs.GetInt("SoundVolume", 1);
        if (Defs.Volume == 0)
        {
            Defs.MuteSounds(true);
        }
        SetSoundImage();
    }
    
    public void Click()
    {
        Defs.SwitchSounds();
        SetSoundImage();
    }

    private void SetSoundImage()
    {
        if (Defs.Volume == 1) GetComponent<Image>().sprite = spriteOn;
        else GetComponent<Image>().sprite = spriteOff;
    }
}