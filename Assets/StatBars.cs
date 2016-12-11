using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBars : MonoBehaviour
{
	public enum Stat { HP, Stamina };
    public Stat chooseStat;

	void Update ()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;
        LivingCreature.Statistics stats = player.GetComponent<PlayerCreature>().stats;

        switch (chooseStat)
        {
            case Stat.HP:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(stats.maxHealth * 2, rt.sizeDelta.y);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, (float) stats.curHealth / stats.maxHealth, 0.15f);
                    break;
                }
            case Stat.Stamina:
                {
                    RectTransform rt = GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(stats.maxStamina * 2, rt.sizeDelta.y);

                    GetComponent<Image>().fillAmount = Mathf.Lerp(GetComponent<Image>().fillAmount, stats.curStamina / stats.maxStamina, 0.15f);                    
                    break;
                }
        }
	}
}
