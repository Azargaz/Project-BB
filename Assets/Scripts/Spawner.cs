using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;
    public int enemiesPerWave;

	void Update ()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if(enemies.Length < enemiesPerWave)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
        }
	}
}
