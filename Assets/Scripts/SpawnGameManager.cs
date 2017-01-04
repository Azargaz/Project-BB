using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGameManager : MonoBehaviour
{
    public GameObject gameManager;
    GameObject gm;

    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController");

        if(gm == null)
        {
            Instantiate(gameManager);
        }
    }
}
