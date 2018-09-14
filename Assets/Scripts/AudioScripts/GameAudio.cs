using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : ObjectAudio {

    protected override AudioSource[] SetUpEngineAudioSource(AudioClipAndOption elem)
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
            source.spatialBlend = 0f;

            source.dopplerLevel = 1;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.minDistance = 0;
            source.maxDistance = elem.MaxDistance;

            sources[i] = source;
        }

        return sources;
    }

    public override void Play(string AudioName)
    {
        if (!AudioDictionary.ContainsKey(AudioName))
        {
            Debug.Log(gameObject.name + "에 " + AudioName + "라는 AudioClip이 없습니다.");
            return;
        }

        int index = Random.Range(0, AudioDictionary[AudioName].Length);

        AudioDictionary[AudioName][index].Play();
    }
}
