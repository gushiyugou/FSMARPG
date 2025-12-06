using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


namespace XX.Tool.AudioSystem
{
    [Serializable]
    public class Sound
    {
        [Header("基础设置")]
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;

        public bool loop = false;
        public bool playOnAwake = false;

        [HideInInspector]
        public AudioSource source;

        [Header("混音器设置")]
        public AudioMixerGroup outputGroup;
        public bool useMixerGroup = true;

        [Header("空间设置")]
        [Range(0f, 1f)]
        public float spatialBlend = 0f;
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [Header("高级设置")]
        public SoundCategory category = SoundCategory.SFX;
        public int priority = 128;

        [Header("脚步声专用设置")]
        public bool isFootstep = false;
        public int poolSize = 5;
        public float minPitchVariation = 0.9f;
        public float maxPitchVariation = 1.1f;
        public AudioClip[] variations;
    }


    public enum SoundCategory
    {
        Master,
        Music,
        SFX,
        UI,
        Footstep,
        Ambient
    }
}
