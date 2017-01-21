using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBars : MonoBehaviour
{
	public enum Stat { HP, Stamina, RH };
    public Stat chooseStat;
    public RectTransform bg;
    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void Update ()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        LivingCreature.Statistics stats = player.GetComponent<PlayerCreature>().stats;
        PlayerCreature pc = player.GetComponent<PlayerCreature>();

        switch (chooseStat)
        {
            case Stat.HP:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = stats.maxHealth;
                    rt.sizeDelta = new Vector2(stat, rt.sizeDelta.y);
                    bg.sizeDelta = new Vector2(stat + 2, rt.sizeDelta.y + 2);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float) stats.curHealth / stats.maxHealth, 0.15f);
                    break;
                }
            case Stat.Stamina:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = stats.maxStamina;
                    rt.sizeDelta = new Vector2(stat, rt.sizeDelta.y);
                    bg.sizeDelta = new Vector2(stat + 2, rt.sizeDelta.y + 2);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, stats.curStamina / stats.maxStamina, 0.15f);                    
                    break;
                }
            case Stat.RH:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = stats.maxHealth;
                    rt.sizeDelta = new Vector2(stat, rt.sizeDelta.y);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float) (stats.curHealth + pc.healthToRestore) / stats.maxHealth, 0.15f);
                    break;
                }
        }
	}
}
