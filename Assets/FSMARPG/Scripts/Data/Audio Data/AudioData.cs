
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XX.Tool.AudioSystem;

public class AudioData
{
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float uiVolume = 1f;
    public float footstepVolume = 1f;
    public float ambientVolume = 1f;

    private float value;


    public void UpdateParamData(Dictionary<string,float> dataDic)
    {
        masterVolume = GetParamValue(dataDic, AudioManager.Instance.masterVolumeParam) ? value: masterVolume;
        musicVolume = GetParamValue(dataDic, AudioManager.Instance.musicVolumeParam) ? value : musicVolume;
        sfxVolume = GetParamValue(dataDic, AudioManager.Instance.sfxVolumeParam) ? value: sfxVolume;
        uiVolume = GetParamValue(dataDic, AudioManager.Instance.uiVolumeParam) ? value : uiVolume;
        footstepVolume = GetParamValue(dataDic, AudioManager.Instance.footstepVolumeParam) ? value : footstepVolume;
        ambientVolume = GetParamValue(dataDic, AudioManager.Instance.ambientVolumeParam) ? value : ambientVolume;

        SaveParamData();
    }


    private bool GetParamValue(Dictionary<string, float> dataDic,string paramName)
    {
        if(dataDic.TryGetValue(paramName,out value)) return true;
        else return false;
    }


    public void SaveParamData()
    {
        PlayerPrefs.SetFloat(AudioManager.Instance.masterVolumeParam, masterVolume);
        PlayerPrefs.SetFloat(AudioManager.Instance.musicVolumeParam, musicVolume);
        PlayerPrefs.SetFloat(AudioManager.Instance.sfxVolumeParam, sfxVolume);
        PlayerPrefs.SetFloat(AudioManager.Instance.uiVolumeParam, uiVolume);
        PlayerPrefs.SetFloat(AudioManager.Instance.footstepVolumeParam, footstepVolume);
        PlayerPrefs.SetFloat(AudioManager.Instance.ambientVolumeParam, ambientVolume);
    }
}
