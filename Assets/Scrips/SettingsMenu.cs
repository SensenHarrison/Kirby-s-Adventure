using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void OnEnable()
    {
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        bgmSlider.SetValueWithoutNotify(bgm);
        sfxSlider.SetValueWithoutNotify(sfx);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(bgm);
            AudioManager.Instance.SetSFXVolume(sfx);
        }
    }

    public void OnBGMChanged(float value)
    {
        Debug.Log("BGM Changed: " + value);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
    }

    public void OnSFXChanged(float value)
    {
        Debug.Log("SFX Changed: " + value);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }
    
    public void ResetAudioSettings()
    {
        float defaultValue = 0.5f;

        bgmSlider.SetValueWithoutNotify(defaultValue);
        sfxSlider.SetValueWithoutNotify(defaultValue);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(defaultValue);
            AudioManager.Instance.SetSFXVolume(defaultValue);
        }

        PlayerPrefs.SetFloat("BGMVolume", defaultValue);
        PlayerPrefs.SetFloat("SFXVolume", defaultValue);
        PlayerPrefs.Save();
    }
}