using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int Score = 0;
    public static GameManager instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(0);
                Destroy(gameObject);
            }            
        }
        else
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = "Score: " + Score;
        }
    }

    public void GameOver()
    {
        Debug.Log("Gameover. Your score: " + Score);
        SceneManager.LoadScene(1);
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        if (Score > 0)
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text += Score;
            Score = 0;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = @"Game over!
Your score: 0";
        }
    }
}
