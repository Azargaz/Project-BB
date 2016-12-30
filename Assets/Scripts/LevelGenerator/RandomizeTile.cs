using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTile : MonoBehaviour
{
    public Sprite[] sprites;
    public float[] spriteChance;
    SpriteRenderer spriteRenderer;
    
	void Awake ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            bool chooseTile = Random.value <= spriteChance[i] / 100f;
            
            if(chooseTile)
            {
                spriteRenderer.sprite = sprites[i];
            }
        }
	}	
}
