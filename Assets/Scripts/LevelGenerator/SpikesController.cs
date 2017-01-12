using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesController : MonoBehaviour
{
    public int firstSpriteStepCount;
    public int secondSpriteStepCount;
    public int thirdSpriteStepCount;
    int playerStepOnCount = 0;
    public Sprite[] sprites;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerCreature>() != null)
        {
            playerStepOnCount++;
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        if (playerStepOnCount >= firstSpriteStepCount)
        {
            sprite.sprite = sprites[0];
        }

        if (playerStepOnCount >= secondSpriteStepCount)
        {
            sprite.sprite = sprites[1];
        }

        if (playerStepOnCount >= thirdSpriteStepCount)
        {
            sprite.sprite = sprites[2];
        }
    }
}
