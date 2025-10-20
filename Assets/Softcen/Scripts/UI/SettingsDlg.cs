using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Beebyte.Obfuscator;
using TMPro;

public class SettingsDlg : MonoBehaviour {
    public AudioMixer mainAudioMixer;
    public Slider sliderAudio;
    public Slider sliderMusic;
    public TextMeshProUGUI txtVersion;
    public GameObject goDebugButton;

    void Start()
    {
        SetSliders();
        //txtVersion.text = "Version " + PygmyMonkey.AdvancedBuilder.AppParameters.Get.bundleVersion;
        txtVersion.SetText ("Version " + Application.version);
        #if SOFTCEN_DEBUG || AD_DEBUG
        goDebugButton.SetActive(true);
        #else
        goDebugButton.SetActive(false);
        #endif

    }

    [SkipRename]
    public void OnSliderAudioChanged()
    {
        GameManager.Instance.SetSFXVolume(sliderAudio.value);
    }
    [SkipRename]
    public void OnSliderMusicChanged()
    {
        GameManager.Instance.SetMusicVolume(sliderMusic.value);
    }
    [SkipRename]
    public void PrestigeButton()
    {

    }

    public void InitAudioMixer()
    {
        SetSliders();
    }

    private void SetSliders()
    {
        float val = GameManager.Instance.playerData.AudioVol;
        if (val < -80f || val > 0f)
            val = -20f;
        sliderAudio.value = val;
        val = GameManager.Instance.playerData.MusicVol;
        if (val < -80f || val > 0f)
            val = -20f;
        sliderMusic.value = val;
    }

}
