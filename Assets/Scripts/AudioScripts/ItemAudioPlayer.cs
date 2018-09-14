using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAudioPlayer : MonoBehaviour {

    [SerializeField] AudioAndClipName[] AudioAndClipNames;

    [System.Serializable]
    struct AudioAndClipName
    {
        public string AudioName;
        public string[] ClipNames;
    }

    Dictionary<string, string[]> AudioNameDictionary;

    void Start()
    {
        AudioNameDictionary = new Dictionary<string, string[]>();

        for (int i = 0; i < AudioAndClipNames.Length; i++)
            AudioNameDictionary.Add(AudioAndClipNames[i].AudioName, AudioAndClipNames[i].ClipNames);
    }

    public void Play(GameObject Owner, string AudioName)
    {
        string ClipName = AudioNameDictionary[AudioName][Random.Range(0, AudioNameDictionary[AudioName].Length)];

        Owner.GetComponent<ItemAudio>().Play(ClipName);
    }
}
