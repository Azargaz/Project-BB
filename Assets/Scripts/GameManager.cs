using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public GameObject staticHUDPrefab;
    public static GameObject player;
    public List<GameObject> monsters = new List<GameObject>();
    public List<GameObject> flyingProjectiles;
    public bool pause;
    bool passiveMenuOpen;
    GameObject pauseMenu;
    GameObject passiveMenu;
    public static List<Passive> passives = new List<Passive>();

    #region Currency

    float currentCurrency = 0;
    float lastCurrentCurrency = 0;
    float displayCurrency = 0;
    float lerpRate = 0f;

    #endregion    

    void Awake()
    {       
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (player == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            player = Instantiate(playerPrefab);
            player.transform.position = new Vector3(0, 6, 0);
        }

        if(passiveMenu == null && SceneManager.GetActiveScene().buildIndex == 1)
        {
            passiveMenu = Instantiate(staticHUDPrefab, transform).transform.GetChild(0).gameObject;
            passiveMenu.SetActive(false);
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
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.FindGameObjectWithTag("Pause");
                pauseMenu.SetActive(false);
            }

            if(passiveMenu == null)
            {
                passiveMenu = GameObject.FindGameObjectWithTag("Passives");

                if(passiveMenu == null)
                    passiveMenu = Instantiate(staticHUDPrefab, transform).transform.GetChild(0).gameObject;

                passiveMenu.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                if(!passiveMenuOpen)
                    PauseUnPause(pauseMenu);
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if(!pause || passiveMenuOpen)
                {
                    passiveMenuOpen = !passiveMenuOpen;

                    PauseUnPause(passiveMenu);
                }            
            }

            if (Input.GetButtonDown("Submit"))
                RestartGame();

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

    public void PauseUnPause(GameObject pauseScreen)
    {
        pause = !pause;

        Time.timeScale = pause ? 0 : 1;

        if (pauseScreen != null)
            pauseScreen.SetActive(pause);

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

    void ResetPassives()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            passives[i].passive.Reset();
        }
    }

    public IEnumerator GameOver()
    {
        GameObject.FindGameObjectWithTag("YouDied").GetComponent<Animator>().SetTrigger("Play");
        yield return new WaitForSeconds(3f);
        RestartGame();
    }

    public void RestartGame()
    {
        CurrencyController.CC.ResetCurrency();
        Destroy(player);
        ResetPassives();
        LoadScene(1);
    }

    public void LoadScene(int i)
    {
        if (pauseMenu != null)
        {
            if (pause && !passiveMenuOpen)
                PauseUnPause(pauseMenu);
        }

        if (passiveMenu != null)
        {            
            if (pause && passiveMenuOpen)
                PauseUnPause(passiveMenu);

            passiveMenuOpen = false;
        }

        CurrencyController.CC.ResetRerollCost();
        SceneManager.LoadScene(i);
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
