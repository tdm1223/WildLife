using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Antidote : Item
{
    public enum antidoteColor //해독제 색깔 선택
    {
        Green,
        Brown,
        Yellow
    }

    public GameObject ParticleSystem;
    public antidoteColor aColor;
    public Material[] material = new Material[3];
    public Sprite[] itemIconArray = new Sprite[3];
    private int colorNum;

    public override void Start()
    {
        colorNum = (int)(Random.value * 10) % 3;
        if (colorNum == 0)
        {
            aColor = antidoteColor.Green;
        }
        else if (colorNum == 1)
        {
            aColor = antidoteColor.Brown;
        }
        else if (colorNum == 2)
        {
            aColor = antidoteColor.Yellow;
        }
        transform.Find("Potion_bottle").GetComponent<MeshRenderer>().material = material[(int)aColor];
        ItemIcon = itemIconArray[(int)aColor];
        base.Start();
        ItemName = "해독제(" + aColor + ")";
        EquipTag = "EquipArm";
    }
    public override void Use(int pos)
    {
        Vector3 headPoint = new Vector3(Owner.transform.position.x, Owner.transform.position.y + 2.0f, Owner.transform.position.z);
        base.Use(pos);

        if (Owner.transform.GetComponent<SurvivorStatus>().Infection) //감염상태일때
        {
            if (getAntidoteColor().Equals(Owner.GetComponent<SurvivorStatus>().InfectionColor))//색이 같다면
            {
                Owner.transform.GetComponent<SurvivorStatus>().CmdSetInfection(false);//해독
                GameController.GetInstance().ActionMessage("Right", "맞는 해독제를 사용했습니다.", Owner);
                if (Audio != null)
                {
                    Audio.Play(owner, "Antidote");
                }
                Owner.GetComponent<SurvivorStatus>().CmdSpawnAntidoteEffect(gameObject, headPoint,Owner.transform.rotation);
            }
            else
            {
                GameController.GetInstance().ActionMessage("Wrong", "다른 해독제를 사용했습니다.", Owner);
            }
        }
    }

    public string getAntidoteColor()
    {
        return aColor.ToString();
    }
}
