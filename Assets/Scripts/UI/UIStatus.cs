using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatus : MonoBehaviour
{
    Image hpIcon;
    Text hpText;
    Image hungerIcon;
    Text hungerText;

    static int maxHP;
    static int maxHunger;

    void Start()
    {
        hpIcon = transform.Find("HP Icon").GetComponent<Image>();
        hpText = transform.Find("HP Text").GetComponent<Text>();
        hungerIcon = transform.Find("Hunger Icon").GetComponent<Image>();
        hungerText = transform.Find("Hunger Text").GetComponent<Text>();
    }

    public void InitStatus(int r_maxHp, int r_maxHunger) //paremeter[0]은 maxHP, parameter[1]은 maxHunger
    {
        maxHP = r_maxHp;
        maxHunger = r_maxHunger;

        hpText.text = maxHP + " / " + maxHP;

        hungerText.text = maxHunger + " / " + maxHunger;
    }

    public void UpdateHP(int HP)
    {
        hpIcon.fillAmount = (float)HP / maxHP;
        hpText.text = HP + " / " + maxHP;
    }

    public void UpdateHunger(int Hunger)
    {
        hungerIcon.fillAmount = (float)Hunger / maxHunger;
        hungerText.text = Hunger + " / " + maxHunger;
    }


}
