using DoozyUI;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Sprite spriteOff;

    public Sprite spriteOn;
    // Use this for initialization

    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SetSoundImage();
    }

    public void SwitchSound(bool state)
    {
        if (UIManager.isSoundOn && !state) UIManager.ToggleSound();
        
            
    }
    
    public void Click()
    {
        Defs.SwitchSounds();
        SetSoundImage();
    }

    private void SetSoundImage()
    {
        if (AudioListener.volume > 0f) GetComponent<Image>().sprite = spriteOn;
        else GetComponent<Image>().sprite = spriteOff;
    }
}