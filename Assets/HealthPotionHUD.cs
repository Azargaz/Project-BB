using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPotionHUD : MonoBehaviour
{
    PlayerCreature player;
    public Sprite[] sprites;
    public Image potionSprite;
    public Text uses;

	void Start ()
    {
        player = GameManager.player.GetComponent<PlayerCreature>();
	}

	void Update ()
    {
        float spriteId = (float) player.GetPotionUses() / player.GetPotionMaxUses();

        potionSprite.sprite = sprites[spriteId <= 0 ? 0 : spriteId < 0.4f ? 1 : spriteId < 0.8f ? 2 : 3];
        uses.text = "" + player.GetPotionUses();
	}
}
