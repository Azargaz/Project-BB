using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public enum PickupType { healthPotion };
    public PickupType type;
    public GameObject numbersDisplay;

    void HealthPotion()
    {
        playerCreature.Heal(50);
    }

    void Pickedup()
    {
        switch(type)
        {
            case PickupType.healthPotion:
                {
                    HealthPotion();
                    break;
                }
        }
    }

    PlayerCreature playerCreature;

    void Start()
    {
        playerCreature = GameManager.player.GetComponent<PlayerCreature>();
    }

    void Update()
    {
        if(playerCreature == null)
        {
            playerCreature = GameManager.player.GetComponent<PlayerCreature>();
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 8)
        {
            Pickedup();
            Destroy(gameObject);
        }
    }
}
