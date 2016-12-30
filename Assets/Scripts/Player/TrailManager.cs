using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    public SpriteRenderer armor;
    public SpriteRenderer player;
    public SpriteRenderer[] armorTrails;
    public SpriteRenderer[] playerTrails;

	void Update ()
    {
        if (armor == null || player == null)
            return;

        if(armorTrails.Length == playerTrails.Length)
        {
            for (int i = 0; i < armorTrails.Length; i++)
            {
                armorTrails[i].sprite = armor.sprite;
                playerTrails[i].sprite = player.sprite;
            }
        }
	}
}
