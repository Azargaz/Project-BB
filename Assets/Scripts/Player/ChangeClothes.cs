using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeClothes : MonoBehaviour
{
    public Sprite[] sprites;
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.C))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                sprite.sprite = null;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                sprite.sprite = sprites[0];
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                sprite.sprite = sprites[1];
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                sprite.sprite = sprites[2];
            }
        }
    }
}
