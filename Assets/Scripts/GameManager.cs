using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int Score = 0;
    public static GameManager instance;
    public GameObject playerPrefab;
    public static GameObject player;
    public List<GameObject> monsters = new List<GameObject>();
    public List<GameObject> flyingProjectiles;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(this);

        if (player == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = new Vector3(0, 6, 0);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(1);
                if(player != null)
                    Destroy(player);
                Destroy(gameObject);
            }            
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = "Score: " + Score;
        }
    }

    public IEnumerator GameOver()
    {
        GameObject.FindGameObjectWithTag("YouDied").GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(3f);
        Debug.Log("Gameover. Your score: " + Score);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (player == null)
            {
                player = Instantiate(playerPrefab);
                player.transform.position = new Vector3(0, 6, 0);
            }

            return;
        }
        
        if(SceneManager.GetActiveScene().buildIndex == 2)
        {
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
}
