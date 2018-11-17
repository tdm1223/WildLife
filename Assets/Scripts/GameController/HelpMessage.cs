using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMessage : MonoBehaviour
{
    [SerializeField] protected HelpStruct[] HelpStructs;

    [System.Serializable]
    public struct HelpStruct
    {
        public string CauseOfDeathName;
        public HelpMessageStruct[] HelpMessageStructs;
    }

    [System.Serializable]
    public struct HelpMessageStruct
    {
        public Sprite HelpSprite;
        public string HelpMessage;
    }

    protected Dictionary<string, HelpMessageStruct[]> HelpDictionary;

    void Awake ()
    {
        HelpDictionary = new Dictionary<string, HelpMessageStruct[]>();

        for (int i = 0; i < HelpStructs.Length; i++)
                HelpDictionary.Add(HelpStructs[i].CauseOfDeathName, HelpStructs[i].HelpMessageStructs);
    }

    public HelpMessageStruct GetHelpMessage(string CauseOfDeathName)
    {
        return HelpDictionary[CauseOfDeathName][Random.Range(0, HelpDictionary[CauseOfDeathName].Length)];
    }
}
