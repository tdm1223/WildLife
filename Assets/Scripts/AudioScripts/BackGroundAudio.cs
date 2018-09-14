using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundAudio : ObjectAudio {

    IEnumerator AudioLooper;

    int BGMindex;
    int BGSFXindex;

    protected void Start()
    {
        AudioLooper = AudioLoop();
        StartCoroutine(AudioLooper);
    }

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
            source.loop = false;
            source.pitch = 1f;
            source.spatialBlend = 0f;

            source.dopplerLevel = 1;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 0;
            source.maxDistance = elem.MaxDistance;

            sources[i] = source;
        }

        return sources;
    }

    IEnumerator AudioLoop()
    {
        BGMindex = Random.Range(0, AudioDictionary["BGM"].Length);
        AudioDictionary["BGM"][BGMindex].Play();

        BGSFXindex = Random.Range(0, AudioDictionary["BGSFX"].Length);
        AudioDictionary["BGSFX"][BGSFXindex].Play();

        while (true)
        {
            if (!AudioDictionary["BGM"][BGMindex].isPlaying)
            {
                BGMindex = Random.Range(0, AudioDictionary["BGM"].Length);
                AudioDictionary["BGM"][BGMindex].Play();
            }

            if (!AudioDictionary["BGSFX"][BGMindex].isPlaying)
            {
                BGSFXindex = Random.Range(0, AudioDictionary["BGSFX"].Length);
                AudioDictionary["BGSFX"][BGSFXindex].Play();
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public void StopBGM()
    {
        StopCoroutine(AudioLooper);

        if (AudioDictionary["BGM"][BGMindex].isPlaying)
            AudioDictionary["BGM"][BGMindex].Stop();
    }
}
