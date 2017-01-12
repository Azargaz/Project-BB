using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSummon : MonoBehaviour
{
    int maxSummons = 20;
    public Transform spawnPoint;
    public GameObject summon;
    List<GameObject> summons = new List<GameObject>();

    void Update()
    {
        if(summons.Count > 0 && summons[0] == null)
        {
            summons.RemoveAt(0);
        }

        if(summons.Count > maxSummons)
        {
            summons[0].GetComponent<LivingCreature>().Kill();
            summons.RemoveAt(0);
        }
    }

	void AnimationSummon()
    {
        GameObject clone = Instantiate(summon, spawnPoint.position, Quaternion.identity);
        clone.transform.parent = GameObject.Find("PlayerSummons").transform;
        summons.Add(clone);
    }
}
