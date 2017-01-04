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

        float scale = Mathf.Round(1920f / Screen.width);

        switch (chooseStat)
        {
            case Stat.HP:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = 0.6f * stats.maxHealth + 200f;
                    rt.sizeDelta = Scale(rt.sizeDelta, scale, stat, false);
                    bg.sizeDelta = Scale(bg.sizeDelta, scale, stat, true);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float) stats.curHealth / stats.maxHealth, 0.15f);
                    break;
                }
            case Stat.Stamina:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = 0.6f * stats.maxStamina + 200f;
                    rt.sizeDelta = Scale(rt.sizeDelta, scale, stat, false);
                    bg.sizeDelta = Scale(bg.sizeDelta, scale, stat, true);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, stats.curStamina / stats.maxStamina, 0.15f);                    
                    break;
                }
            case Stat.RH:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    float stat = 0.6f * stats.maxHealth + 200f;
                    rt.sizeDelta = Scale(rt.sizeDelta, scale, stat, false);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float) (stats.curHealth + pc.healthToRestore) / stats.maxHealth, 0.15f);
                    break;
                }
        }
	}

    Vector2 Scale(Vector2 deltasize, float scale, float stat, bool bg)
    {
        if (!bg)
            return new Vector2(stat / scale, deltasize.y);
        else
            return new Vector2(stat / scale + 2, deltasize.y);
    }
}
