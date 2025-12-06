using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace XX.Tool.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("混音器引用")]
        public AudioMixer mainMixer;

        [Header("混音器组引用")]
        public AudioMixerGroup masterGroup;
        public AudioMixerGroup musicGroup;
        public AudioMixerGroup sfxGroup;
        public AudioMixerGroup uiGroup;
        public AudioMixerGroup footstepGroup;
        public AudioMixerGroup ambientGroup;

        [Header("混音器参数名称")]
        public string masterVolumeParam = "MasterVolume";
        public string musicVolumeParam = "MusicVolume";
        public string sfxVolumeParam = "SFXVolume";
        public string uiVolumeParam = "UIVolume";
        public string footstepVolumeParam = "FootstepVolume";
        public string ambientVolumeParam = "AmbientVolume";

        [Header("音效配置")]
        [SerializeField] private Sound[] sounds;

        [Header("音量设置")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float uiVolume = 1f;
        [Range(0f, 1f)] public float footstepVolume = 1f;
        [Range(0f, 1f)] public float ambientVolume = 1f;

        [Header("脚步声性能设置")]
        public int maxConcurrentSounds = 20;
        public int maxConcurrentFootsteps = 8;
        public float footstepCooldown = 0.1f;
        public float footstepMaxDistance = 30f;


        // 内部变量
        private Dictionary<string, Sound> soundDictionary;//声音字典
        private Dictionary<SoundCategory, float> categoryVolumes;//声音类别音量
        private Dictionary<SoundCategory, AudioMixerGroup> categoryMixerGroups;//混音器组
        private Dictionary<string, Queue<AudioSource>> footstepPools;//脚步音频源对象池
        private Dictionary<string, float> lastFootstepTime;//脚步间隔
        private List<AudioSource> activeAudioSources;//活动式音频源

        // 混音器相关
        private const float MIN_DB = -80f;
        private const float MAX_DB = 0f;

        #region 初始化和单例

        private void Awake()
        {
            InitializeSingleton();
            InitializeAudioManager();
        }

        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        //音效管理器初始化
        private void InitializeAudioManager()
        {
            LoadVolumeSettings();
            InitializeDictionaries();
            InitializeMixerGroups();
            CreateAudioSources();
            InitializeFootstepSystem();
            ApplySavedVolumesToMixer();
        }

        private void InitializeDictionaries()
        {
            soundDictionary = new Dictionary<string, Sound>();
            categoryVolumes = new Dictionary<SoundCategory, float>
            {
                { SoundCategory.Master, masterVolume },
                { SoundCategory.Music, musicVolume },
                { SoundCategory.SFX, sfxVolume },
                { SoundCategory.UI, uiVolume },
                { SoundCategory.Footstep, footstepVolume },
                { SoundCategory.Ambient, ambientVolume }
            };

            categoryMixerGroups = new Dictionary<SoundCategory, AudioMixerGroup>
            {
                { SoundCategory.Music, musicGroup },
                { SoundCategory.SFX, sfxGroup },
                { SoundCategory.UI, uiGroup },
                { SoundCategory.Footstep, footstepGroup },
                { SoundCategory.Ambient, ambientGroup }
            };

            footstepPools = new Dictionary<string, Queue<AudioSource>>();
            lastFootstepTime = new Dictionary<string, float>();
            activeAudioSources = new List<AudioSource>();
        }

        private void InitializeMixerGroups()
        {
            // 确保混音器组引用正确
            if (masterGroup == null && mainMixer != null)
            {
                // 尝试自动查找主输出组
                var groups = mainMixer.FindMatchingGroups("Master");
                if (groups.Length > 0) masterGroup = groups[0];
            }
        }


        private void CreateAudioSources()
        {
            foreach (Sound sound in sounds)
            {
                if (sound.isFootstep) continue; // 脚步声使用对象池

                AudioSource source = gameObject.AddComponent<AudioSource>();
                ConfigureAudioSource(source, sound);
                sound.source = source;
                soundDictionary[sound.name] = sound;

                if (sound.playOnAwake)
                    source.Play();
            }
        }

        #endregion

        #region 混音器音量控制

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            SetMixerVolume(masterVolumeParam, masterVolume);
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            volume = Mathf.Clamp01(volume);
            categoryVolumes[category] = volume;

            string paramName = GetVolumeParameterName(category);
            if (!string.IsNullOrEmpty(paramName))
            {
                SetMixerVolume(paramName, volume);
            }

            // 更新本地变量并保存
            switch (category)
            {
                case SoundCategory.Music:
                    musicVolume = volume;
                    PlayerPrefs.SetFloat("MusicVolume", volume);
                    break;
                case SoundCategory.SFX:
                    sfxVolume = volume;
                    PlayerPrefs.SetFloat("SFXVolume", volume);
                    break;
                case SoundCategory.UI:
                    uiVolume = volume;
                    PlayerPrefs.SetFloat("UIVolume", volume);
                    break;
                case SoundCategory.Footstep:
                    footstepVolume = volume;
                    PlayerPrefs.SetFloat("FootstepVolume", volume);
                    break;
                case SoundCategory.Ambient:
                    ambientVolume = volume;
                    PlayerPrefs.SetFloat("AmbientVolume", volume);
                    break;
            }
        }

        private void SetMixerVolume(string parameterName, float volume)
        {
            if (mainMixer == null)
            {
                Debug.LogWarning("AudioMixer not assigned! Using direct volume control.");
                return;
            }

            // 将0-1的线性音量转换为分贝值
            float dB = VolumeToDecibels(volume);
            bool success = mainMixer.SetFloat(parameterName, dB);

            if (!success)
            {
                Debug.LogWarning($"Failed to set mixer parameter: {parameterName}");
            }
        }

        private float GetMixerVolume(string parameterName)
        {
            if (mainMixer == null) return 1f;

            float dB;
            if (mainMixer.GetFloat(parameterName, out dB))
            {
                return DecibelsToVolume(dB);
            }

            Debug.LogWarning($"Failed to get mixer parameter: {parameterName}");
            return 1f;
        }

        private float VolumeToDecibels(float volume)
        {
            if (volume <= 0f) return MIN_DB;
            return Mathf.Log10(volume) * 20f;
        }

        private float DecibelsToVolume(float dB)
        {
            return Mathf.Pow(10f, dB / 20f);
        }

        private string GetVolumeParameterName(SoundCategory category)
        {
            switch (category)
            {
                case SoundCategory.Master: return masterVolumeParam;
                case SoundCategory.Music: return musicVolumeParam;
                case SoundCategory.SFX: return sfxVolumeParam;
                case SoundCategory.UI: return uiVolumeParam;
                case SoundCategory.Footstep: return footstepVolumeParam;
                case SoundCategory.Ambient: return ambientVolumeParam;
                default: return null;
            }
        }

        private void ApplySavedVolumesToMixer()
        {
            SetMixerVolume(masterVolumeParam, masterVolume);
            SetMixerVolume(musicVolumeParam, musicVolume);
            SetMixerVolume(sfxVolumeParam, sfxVolume);
            SetMixerVolume(uiVolumeParam, uiVolume);
            SetMixerVolume(footstepVolumeParam, footstepVolume);
            SetMixerVolume(ambientVolumeParam, ambientVolume);
        }

        #endregion

        #region 音效播放控制（混音器增强版）

        private void ConfigureAudioSource(AudioSource source, Sound sound)
        {

            source.clip = sound.isFootstep ? 
                sound.variations[Random.Range(0, sound.variations.Length)] : sound.clip; 

            source.volume = sound.volume; // 基础音量，混音器会进一步控制

            // 设置混音器输出组
            if (sound.useMixerGroup && sound.outputGroup != null)
            {
                source.outputAudioMixerGroup = sound.outputGroup;
            }
            else if (categoryMixerGroups.ContainsKey(sound.category))
            {
                source.outputAudioMixerGroup = categoryMixerGroups[sound.category];
            }
            else if (masterGroup != null)
            {
                source.outputAudioMixerGroup = masterGroup;
            }

            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.playOnAwake = sound.playOnAwake;
            source.spatialBlend = sound.spatialBlend;
            source.minDistance = sound.minDistance;
            source.maxDistance = sound.maxDistance;
            source.rolloffMode = sound.rolloffMode;
            source.priority = sound.priority;
        }

        public void Play(string soundName)
        {
            if (!CanPlaySound(soundName)) return;

            if (soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                if (!sound.source.isPlaying)
                {
                    sound.source.Play();
                    activeAudioSources.Add(sound.source);
                }
                
            }
            else
            {
                Debug.LogWarning($"Sound: {soundName} not found!");
            }
        }

        public void PlayOneShot(string soundName)
        {
            if (!CanPlaySound(soundName)) return;

            if (soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                // 注意：PlayOneShot会忽略AudioSource的某些设置，但会使用混音器组
                sound.source.PlayOneShot(sound.clip, sound.volume);
                activeAudioSources.Add(sound.source);
            }
        }

        public void PlayAtPoint(string soundName, Vector3 position, float volumeMultiplier = 1f)
        {
            if (!CanPlaySound(soundName)) return;

            if (soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                // 创建临时AudioSource用于3D播放
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = position;
                AudioSource tempSource = tempGO.AddComponent<AudioSource>();

                ConfigureAudioSource(tempSource, sound);
                tempSource.volume = sound.volume * volumeMultiplier;
                tempSource.spatialBlend = 1f; // 强制3D

                tempSource.Play();
                Destroy(tempGO, sound.clip.length + 0.1f);
            }
        }

        public void PlayFootStep(string soundName,Transform transform)
        {
            if (!CanPlayFootstep(soundName)) return;
            AudioSource source = footstepPools[soundName].Dequeue();
            source.transform.position = transform.position;
            source.gameObject.SetActive(true);
            for(int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name != soundName) continue;
                source.clip = sounds[i].variations[Random.Range(0, sounds[i].variations.Length)];
                source.Play();
                StartCoroutine(DisableAfterPlay(source, soundName));
                return;
            }
            
            
        }

        private IEnumerator DisableAfterPlay(AudioSource source,string soundName)
        {
            yield return new WaitWhile(()=>source.isPlaying);
            source.transform.position=transform.parent.position;
            source.gameObject.SetActive(false);
            footstepPools[soundName].Enqueue(source);
        }


        #endregion

        #region 混音器效果控制

        public void SetMixerEffectEnabled(string effectName, bool enabled)
        {
            if (mainMixer == null) return;

            float value = enabled ? 1f : 0f;
            mainMixer.SetFloat(effectName + "Enabled", value);
        }

        public void SetMixerEffectParameter(string effectName, string parameter, float value)
        {
            if (mainMixer == null) return;

            mainMixer.SetFloat(effectName + parameter, value);
        }

        // 示例：设置低通滤波（用于游戏暂停效果）
        public void SetLowPassFilter(float cutoffFrequency)
        {
            if (mainMixer == null) return;

            mainMixer.SetFloat("LowPassCutoff", cutoffFrequency);
        }

        // 示例：设置混响效果
        public void SetReverb(float reverbLevel)
        {
            if (mainMixer == null) return;

            mainMixer.SetFloat("ReverbLevel", reverbLevel);
        }

        #endregion

        #region 快照过渡系统

        public void TransitionToSnapshot(string snapshotName, float transitionTime = 2f)
        {
            if (mainMixer == null) return;

            AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[1];
            float[] weights = new float[1] { 1f };

            var snapshot = mainMixer.FindSnapshot(snapshotName);
            if (snapshot != null)
            {
                snapshots[0] = snapshot;
                mainMixer.TransitionToSnapshots(snapshots, weights, transitionTime);
            }
            else
            {
                Debug.LogWarning($"Snapshot {snapshotName} not found!");
            }
        }

        public void TransitionToSnapshots(string[] snapshotNames, float[] weights, float transitionTime = 2f)
        {
            if (mainMixer == null) return;

            if (snapshotNames.Length != weights.Length)
            {
                Debug.LogError("Snapshot names and weights arrays must have the same length!");
                return;
            }

            AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[snapshotNames.Length];
            for (int i = 0; i < snapshotNames.Length; i++)
            {
                snapshots[i] = mainMixer.FindSnapshot(snapshotNames[i]);
                if (snapshots[i] == null)
                {
                    Debug.LogWarning($"Snapshot {snapshotNames[i]} not found!");
                    return;
                }
            }

            mainMixer.TransitionToSnapshots(snapshots, weights, transitionTime);
        }

        #endregion

        #region  submix总线控制

        public void SetSubmixEffect(string submixName, string effectName, bool enabled)
        {
            if (mainMixer == null) return;

            // 获取submix并设置效果
            AudioMixerGroup[] groups = mainMixer.FindMatchingGroups(submixName);
            if (groups.Length > 0)
            {
                // 这里需要更复杂的逻辑来操作submix效果
                Debug.Log($"Setting effect {effectName} on submix {submixName} to {enabled}");
            }
        }

        #endregion


        private void LoadVolumeSettings()
        {
            // 从PlayerPrefs加载，如果没有则使用混音器当前值
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", GetMixerVolume(masterVolumeParam));
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", GetMixerVolume(musicVolumeParam));
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", GetMixerVolume(sfxVolumeParam));
            uiVolume = PlayerPrefs.GetFloat("UIVolume", GetMixerVolume(uiVolumeParam));
            footstepVolume = PlayerPrefs.GetFloat("FootstepVolume", GetMixerVolume(footstepVolumeParam));
            ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", GetMixerVolume(ambientVolumeParam));
        }




        #region 内部工具方法

       

        private void InitializeFootstepSystem()
        {
            foreach (var sound in sounds.Where(s => s.isFootstep))
            {
                CreateFootstepPool(sound);
                lastFootstepTime[sound.name] = 0f;
            }
        }

        private void CreateFootstepPool(Sound footstepSound)
        {
            GameObject footPool = new GameObject($"{footstepSound.name}Pool");
            footPool.transform.parent = transform;
            var pool = new Queue<AudioSource>();
            for (int i = 0; i < footstepSound.poolSize; i++)
            {
                GameObject footSound = new GameObject($"FootSound{i}");
                footSound.transform.parent = footPool.transform;
                AudioSource source = footSound.AddComponent<AudioSource>();
                source.loop = false;
                ConfigureAudioSource(source, footstepSound);
                pool.Enqueue(source);
                footSound.SetActive(false);
            }
            footstepPools[footstepSound.name] = pool;
        }

        private bool CanPlaySound(string soundName)
        {
            // 检查音效是否存在
            if (!soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound {soundName} not found!");
                return false;
            }

            // 检查并发音效数量限制
            int activeCount = activeAudioSources.Count(source => source.isPlaying);
            if (activeCount >= maxConcurrentSounds)
            {
                Debug.LogWarning($"Maximum concurrent sounds reached: {maxConcurrentSounds}");
                return false;
            }

            return true;
        }

        private bool CanPlayFootstep(string footstepName)
        {
            // 基础检查
            //if (!CanPlaySound(footstepName)) return false;
            if (!footstepPools.ContainsKey(footstepName)) return false;

            // 冷却时间检查
            if (lastFootstepTime.ContainsKey(footstepName) &&
                Time.time - lastFootstepTime[footstepName] < footstepCooldown)
            {
                return false;
            }

            // 并发脚步声数量检查
            int activeFootsteps = activeAudioSources.Count(source =>
                source.isPlaying && footstepPools.Any(pool => pool.Value.Contains(source)));

            if (activeFootsteps >= maxConcurrentFootsteps)
            {
                return false;
            } 

            return true;
        }

        #endregion

        #region 公共属性访问

        public bool IsPlaying(string soundName)
        {
            return soundDictionary.ContainsKey(soundName) && soundDictionary[soundName].source.isPlaying;
        }

        public float GetSoundLength(string soundName)
        {
            return soundDictionary.ContainsKey(soundName) ? soundDictionary[soundName].clip.length : 0f;
        }

        public Sound[] GetAllSounds()
        {
            return sounds;
        }

        #endregion

        private void OnDestroy()
        {
            // 清理资源
            foreach (var pool in footstepPools.Values)
            {
                foreach (var source in pool)
                {
                    if (source != null)
                        Destroy(source);
                }
            }
        }
    }
}
