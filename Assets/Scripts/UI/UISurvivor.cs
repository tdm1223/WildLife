using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISurvivor : MonoBehaviour
{

    GameObject[] survivors;

    Text[] survivorNickcname = new Text[4];
    GameObject[] survivorDead = new GameObject[4];
    GameObject[] survivorIcon = new GameObject[4];
    Image vignetteImage;

    IEnumerator runSetVinette;

    public void SurvivorUIStart()
    {
        survivors = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < 4; i++)
        {
            survivorNickcname[i] = transform.Find("Survivor" + (i + 1) + " Nickname").GetComponent<Text>();
            survivorNickcname[i].text = "";

            survivorIcon[i] = transform.Find("Survivor" + (i + 1) + " Icon").gameObject;
            survivorIcon[i].SetActive(false);

            survivorDead[i] = transform.Find("Survivor" + (i + 1) + " Dead").gameObject;
        }
        for (int i = 0; i < survivors.Length; i++)
        {
            survivorIcon[i].SetActive(true);
            survivorNickcname[i].text = survivors[i].GetComponent<SurvivorStatus>().PlayerName;
        }
        vignetteImage = transform.parent.Find("VignetteImage").gameObject.GetComponent<Image>();
    }

    public void SurvivorUIUpdate()
    {
        for (int i = 0; i < survivors.Length; i++)
        {
            survivorDead[i].SetActive(survivors[i].GetComponent<SurvivorStatus>().IsDead());
        }
    }

    public void SetVignette(float val)
    {
        if (runSetVinette == null)
        {
            runSetVinette = SetVignetteLerp(val);
            StartCoroutine(runSetVinette);
        }
        else
        {
            StopCoroutine(runSetVinette);
            runSetVinette = SetVignetteLerp(val);
            StartCoroutine(runSetVinette);
        }
    }

    IEnumerator SetVignetteLerp(float val)
    {
        while (true)
        {
            vignetteImage.color = Color.Lerp(vignetteImage.color, new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, val / 255f), Time.deltaTime * 3);

            if (Mathf.Abs(vignetteImage.color.a - (val / 255f)) < 0.01f)
            {
                runSetVinette = null;
                break;
            }

            yield return null;
        }
    }
}
