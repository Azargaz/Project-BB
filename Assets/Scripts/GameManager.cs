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
    bool questLogOpen;
    bool questboardOpen;

    GameObject staticHUD;
    GameObject pauseMenu;
    GameObject passiveMenu;
    public GameObject questLog;
    public GameObject questboard;
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

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (player == null)
            {
                player = Instantiate(playerPrefab);
                player.transform.position = new Vector3(0, 6, 0);
            }

            if (pauseMenu == null)
            {
                pauseMenu = GameObject.FindGameObjectWithTag("Pause");
                pauseMenu.SetActive(false);
            }

            if (staticHUD == null)
            {
                staticHUD = Instantiate(staticHUDPrefab, transform);
            }

            if (passiveMenu == null)
            {
                passiveMenu = staticHUD.transform.Find("Passives").gameObject;
                passiveMenu.SetActive(false);
            }

            if (questLog == null)
            {
                questLog = staticHUD.transform.Find("QuestLog").gameObject;
                questLog.SetActive(false);
            }

            if (questboard == null)
            {
                questboard = staticHUD.transform.Find("Questboard").gameObject;
                questboard.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {           
            if (pauseMenu == null)
            {
                pauseMenu = GameObject.FindGameObjectWithTag("Pause");
                pauseMenu.SetActive(false);
            }

            if (staticHUD == null)
            {
                staticHUD = Instantiate(staticHUDPrefab, transform);
            }

            if (passiveMenu == null)
            {
                passiveMenu = staticHUD.transform.Find("Passives").gameObject;
                passiveMenu.SetActive(false);
            }

            if (questLog == null)
            {
                questLog = staticHUD.transform.Find("QuestLog").gameObject;
                questLog.SetActive(false);
            }

            if(questboard == null)
            {
                questboard = staticHUD.transform.Find("Questboard").gameObject;
                questboard.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                if (!OpenPauseMenu())
                {
                    if (questLogOpen)
                        OpenQuestLog();
                    else if (passiveMenuOpen)
                        OpenPassiveMenu();
                    else if (questboardOpen)
                        OpenQuestboard();                        
                }
            }

            if(Input.GetButtonDown("QuestLog") || (questLogOpen && Input.GetButtonDown("Interact")))
            {
                OpenQuestLog();
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                OpenPassiveMenu();
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

            currency.text = "" + Mathf.RoundToInt(displayCurrency) + "$";

            lastCurrentCurrency = displayCurrency;

            #endregion
        }
    }

    public bool OpenPauseMenu()
    {
        if (!passiveMenuOpen && !questLogOpen && !questboardOpen)
        {
            PauseUnPause(pauseMenu);
            return true;
        }
        else
            return false;
    }

    public void OpenQuestLog()
    {
        if ((!pause && !passiveMenuOpen) || questLogOpen)
        {
            questLogOpen = !questLogOpen;

            PauseUnPause(questLog);
        }
    }

    public void OpenQuestboard()
    {
        if ((!pause && !questboardOpen) || questboardOpen)
        {
            questboardOpen = !questboardOpen;

            PauseUnPause(questboard);
        }
    }

    void OpenPassiveMenu()
    {
        if ((!pause && !questLogOpen) || passiveMenuOpen)
        {
            passiveMenuOpen = !passiveMenuOpen;

            PauseUnPause(passiveMenu);
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

    void Reset()
    {
        for (int i = 0; i < passives.Count; i++)
        {
            passives[i].passive.Reset();
        }

        QuestLogController.QL.ResetQuests();

        foreach (Transform child in questboard.transform.Find("QuestsSpace"))
        {
            Destroy(child.gameObject);
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
        Reset();
        LoadScene(1);
    }

    public void LoadScene(int i)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            Destroy(gameObject);

        if (pauseMenu != null)
        {
            if (pause && !passiveMenuOpen && !questLogOpen && !questboardOpen)
                PauseUnPause(pauseMenu);
        }

        if (passiveMenu != null)
        {            
            if (pause && passiveMenuOpen)
                PauseUnPause(passiveMenu);

            passiveMenuOpen = false;
        }

        if(questLog != null)
        {
            if (pause && questLogOpen)
                PauseUnPause(questLog);

            questLogOpen = false;
        }

        if (questboard != null)
        {
            if (pause && questboardOpen)
                PauseUnPause(questboard);

            questboardOpen = false;
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
