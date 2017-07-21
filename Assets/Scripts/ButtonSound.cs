using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Sprite spriteOff;

    public Sprite spriteOn;
    // Use this for initialization

    private void Awake()
    {
        //PublishingService.Instance.OnEnableMusic += OnEnableMusic;
        //PublishingService.Instance.OnDisableMusic += OnDisableMusic;
    }

    private void OnDestroy()
    {
        //PublishingService.Instance.OnEnableMusic -= OnEnableMusic;
        //PublishingService.Instance.OnDisableMusic -= OnDisableMusic;
    }

    private void OnEnableMusic()
    {
        Defs.MuteSounds(false);
    }

    private void OnDisableMusic()
    {
        Defs.MuteSounds(true);
    }

    private void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
        SetSoundImage();
    }

    // Update is called once per frame
    private void Update()
    {
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