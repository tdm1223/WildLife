using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Audio;

public class ItemAudio : NetworkBehaviour {
    [SerializeField]
    protected AudioMixerGroup AudioMixerGroup;
    [Space(10)]
    [SerializeField]
    protected AudioClipAndOption[] AudioClipAndOptions;

    [System.Serializable]
    protected struct AudioClipAndOption
    {
        public AudioClip AudioClip;
        [Range(0f, 1f)]
        public float volume;
        [Tooltip("소리가 들리는 최대 거리")]
        public float MaxDistance;
    }

    protected Dictionary<string, AudioSource> AudioDictionary;

    protected void Start()
    {
        AudioDictionary = new Dictionary<string, AudioSource>();

        for (int i = 0; i < AudioClipAndOptions.Length; i++)
        {
            AudioDictionary.Add(AudioClipAndOptions[i].AudioClip.name, SetUpEngineAudioSource(AudioClipAndOptions[i]));
        }
    }

    protected AudioSource SetUpEngineAudioSource(AudioClipAndOption elem)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = elem.AudioClip;
        source.outputAudioMixerGroup = AudioMixerGroup;

        source.volume = elem.volume;
        source.playOnAwake = false;
        source.loop = false;
        source.pitch = 1f;
        source.spatialBlend = 1f;

        source.dopplerLevel = 1;
        source.rolloffMode = AudioRolloffMode.Custom;
        source.minDistance = 0;
        source.maxDistance = elem.MaxDistance;

        return source;
    }

    public void Play(string AudioName)
    {
        CmdPlay(AudioName);
    }

    [Command]
    public void CmdPlay(string AudioName)
    {
        RpcPlay(AudioName);
        AudioDictionary[AudioName].Play();
    }

    [ClientRpc]
    public void RpcPlay(string AudioName)
    {
        AudioDictionary[AudioName].Play();
    }
}
