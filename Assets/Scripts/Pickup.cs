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
        playerCreature.stats.curHealth += 50;

        if(numbersDisplay != null)
        {
            GameObject clone = Instantiate(numbersDisplay, transform.position, Quaternion.identity);
            Text txt = clone.transform.GetChild(0).GetComponent<Text>();
            
            txt.text = "+" + 50;

            clone.GetComponent<Animator>().SetTrigger("Display");
            clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
            Destroy(clone, 1f);
        }
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
