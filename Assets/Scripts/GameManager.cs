using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int startingWeapon = 0;
    public static GameManager instance;
    public GameObject playerPrefab;
    public static GameObject player;
    public List<GameObject> monsters = new List<GameObject>();
    public List<GameObject> flyingProjectiles;
    public bool pause;
    GameObject pauseMenu;

    #region Currency

    float currentCurrency = 0;
    float lastCurrentCurrency = 0;
    float displayCurrency = 0;
    float lerpRate = 0f;
    float currentLerpTime;
    float LerpTime = 2f;

    #endregion    

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
            if(Input.GetButtonDown("Submit"))
            {
                LoadScene(1);
            }            
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if(pauseMenu == null)
            {
                pauseMenu = GameObject.FindGameObjectWithTag("Pause");
                pauseMenu.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                PauseUnPause();
            }

            #region Currency

            Text currency = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();

            currentCurrency = CurrencyController.CC.CheckCurrency();

            if (Mathf.Abs(displayCurrency - currentCurrency) > 100)
                lerpRate = 0.05f;
            else if (Mathf.Abs(displayCurrency - currentCurrency) < 10)
                lerpRate = 0.2f;
            
            displayCurrency = Mathf.Lerp(displayCurrency, currentCurrency, lerpRate);            

            if (lastCurrentCurrency == displayCurrency)
                displayCurrency = currentCurrency;

            currency.text = "$$$: " + Mathf.RoundToInt(displayCurrency);

            lastCurrentCurrency = displayCurrency;

            #endregion
        }
    }

    public void PauseUnPause()
    {
        pause = !pause;

        Time.timeScale = pause ? 0 : 1;

        if (pauseMenu != null)
            pauseMenu.SetActive(pause);

        if(player != null)
        {
            player.GetComponent<LivingCreature>().stats.pause = pause;
            player.GetComponent<Animator>().speed = pause ? 0 : 1;
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i] != null)
            {
                monsters[i].GetComponent<LivingCreature>().stats.pause = pause;
                
                if(monsters[i].GetComponent<Animator>() != null)
                    monsters[i].GetComponent<Animator>().speed = pause ? 0 : 1;
            }                     
        }

        for (int i = 0; i < flyingProjectiles.Count; i++)
        {
            if(flyingProjectiles[i] != null)
            {
                flyingProjectiles[i].GetComponent<Projectile>().pause = pause;
            }
        }
    }

    void LoadWeaponSelect()
    {
        SceneManager.LoadScene(3);
        if (player != null)
            Destroy(player);
        Destroy(gameObject);
    }

    public IEnumerator GameOver()
    {
        GameObject.FindGameObjectWithTag("YouDied").GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(3f);
        Destroy(player);
        LoadScene(1);
    }

    public void RestartGame()
    {
        CurrencyController.CC.ResetCurrency();
        LoadScene(1);
    }

    public void LoadScene(int i)
    {
        SceneManager.LoadScene(i);

        if (pause)
            PauseUnPause();

        if (i != 1)
            Destroy(gameObject);        
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (player == null)
            {
                player = Instantiate(playerPrefab);
                WeaponController.wc.currentWeapon = startingWeapon;
                player.transform.position = new Vector3(0, 6, 0);
            }

            return;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeStartingWeapon(Dropdown i)
    {
        startingWeapon = i.value;
    }
}
