using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ObjectAudio : MonoBehaviour {

    [SerializeField] protected AudioMixerGroup AudioMixerGroup;
    [Space(10)]
    [SerializeField] protected AudioClipAndOption[] AudioClipAndOptions;

    [System.Serializable]
    protected struct AudioClipAndOption
    {
        public string AudioName;
        public AudioClip[] AudioClips;
        [Range(0f,1f)]
        public float volume;
        public bool loop;
        [Tooltip("소리가 들리는 최대 거리")]
        public float MaxDistance;
    }

    protected Dictionary<string, AudioSource[]> AudioDictionary;

    protected virtual void Awake()
    {
        AudioDictionary = new Dictionary<string, AudioSource[]>();

        for(int i = 0; i< AudioClipAndOptions.Length; i++)
            AudioDictionary.Add(AudioClipAndOptions[i].AudioName, SetUpEngineAudioSource(AudioClipAndOptions[i]));
    }

    protected virtual AudioSource[] SetUpEngineAudioSource(AudioClipAndOption elem)
    {
        AudioSource[] sources = new AudioSource[elem.AudioClips.Length];

        for (int i = 0; i < elem.AudioClips.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = elem.AudioClips[i];
            source.outputAudioMixerGroup = AudioMixerGroup;

            source.volume = elem.volume;
            source.playOnAwake = false;
            source.loop = elem.loop;
            source.pitch = 1f;
            source.spatialBlend = 1f;

            source.dopplerLevel = 1;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.minDistance = 0;
            source.maxDistance = elem.MaxDistance;

            sources[i] = source;
        }

        return sources;
    }

    public virtual void Play(string AudioName)
    {
        if (!AudioDictionary.ContainsKey(AudioName))
        {
            Debug.Log(gameObject.name + "에 " + AudioName + "라는 AudioClip이 없습니다.");
            return;
        }

        int index = Random.Range(0, AudioDictionary[AudioName].Length);

        SendMsgManager.GetInstance().SendAudioPlayMsg(gameObject, AudioName, index);
    }

    public virtual void Play(string AudioName, GameObject Player)
    {
        if (!AudioDictionary.ContainsKey(AudioName))
        {
            Debug.Log(gameObject.name + "에 " + AudioName + "라는 AudioClip이 없습니다.");
            return;
        }

        int index = Random.Range(0, AudioDictionary[AudioName].Length);

        SendMsgManager.GetInstance().SendAudioPlayMsg(gameObject, AudioName, index, Player);
    }

    public void StartAudioSource(string AudioName, int index)
    {
        AudioDictionary[AudioName][index].Play();
    }

    public virtual void Stop(string AudioName)
    {
        if (!AudioDictionary.ContainsKey(AudioName))
        {
            Debug.Log(gameObject.name + "에 " + AudioName + "라는 AudioClip이 없습니다.");
            return;
        }

        for (int i = 0; i < AudioDictionary[AudioName].Length; i++) 
        {
            if (AudioDictionary[AudioName][i].isPlaying)
                SendMsgManager.GetInstance().SendAudioStopMsg(gameObject, AudioName, i);

        }
    }

    public void EndAudioSource(string AudioName, int index)
    {
        AudioDictionary[AudioName][index].Stop();
    }
}
