using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealRadar : MonoBehaviour
{
    public List<EnemyCreature> enemiesInRange = new List<EnemyCreature>();

    void OnTriggerEnter2D (Collider2D other)
    {
        if(other.GetComponent<EnemyCreature>() != null)
        {
            enemiesInRange.Add(other.GetComponent<EnemyCreature>());
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.GetComponent<EnemyCreature>() != null)
        {
            enemiesInRange.Remove(other.GetComponent<EnemyCreature>());
        }
    }
}
