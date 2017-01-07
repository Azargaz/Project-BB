using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreature : LivingCreature
{
    Animator anim;
    Player controller;
    [HideInInspector]
    public WeaponManager weaponM;
    bool cheatMode = false;
    bool timeStopped = false;
    public float timeStoppedDuration;
    GameObject timeStoppedCounter;
    float timeStoppedTimeLeft;
    int stoppedMobsCount = 0;

    void Awake()
    {
        timeStoppedCounter = GameObject.FindGameObjectWithTag("TimeStop");
        DontDestroyOnLoad(this);
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<Player>();
        weaponM = transform.GetComponentInChildren<WeaponManager>();
    }

    protected override void Update()
    {
        base.Update();

        #region Time stop

        if(timeStoppedCounter == null)
            timeStoppedCounter = GameObject.FindGameObjectWithTag("TimeStop");

        timeStoppedCounter.GetComponentInChildren<Text>().text = !timeStopped ? "Time is moving." : "Time is frozen for " + (timeStoppedTimeLeft > 0f ? Mathf.Ceil(timeStoppedTimeLeft) : 0f) + " seconds.";       
        Vector2 curSizeDelta = timeStoppedCounter.GetComponentInChildren<Image>().rectTransform.sizeDelta;
        float smooth = 0.1f;
        //float curFill = timeStoppedCounter.GetComponentInChildren<Image>().fillAmount;
        //timeStoppedCounter.GetComponentInChildren<Image>().fillAmount = timeStopped ? Mathf.Lerp(curFill, 1, smooth) : Mathf.Lerp(curFill, 0, smooth);
        timeStoppedCounter.GetComponentInChildren<Image>().rectTransform.sizeDelta = timeStopped ? Vector2.Lerp(curSizeDelta, new Vector2(Screen.width * 2, Screen.width * 2), smooth) : Vector2.Lerp(curSizeDelta, Vector2.zero, smooth);

        if(timeStopped)
        {
            List<GameObject> mobs = GameManager.instance.monsters;

            if(stoppedMobsCount < mobs.Count)
            {
                for (int i = 0; i < mobs.Count; i++)
                {
                    if (mobs[i] != null && mobs[i].GetComponent<EnemyAI>() != null)
                    {
                        mobs[i].GetComponent<EnemyAI>().freeze = timeStopped;
                    }
                }

                stoppedMobsCount = mobs.Count;
            }            
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            timeStopped = !timeStopped;

            if (timeStopped)
            {
                stats.Invincibility(timeStoppedDuration);
                timeStoppedTimeLeft = timeStoppedDuration;
            }
            else
            {
                stats.invincibilityTime = 0;
                timeStoppedTimeLeft = 0;
            }

            List<GameObject> mobs = GameManager.instance.monsters;

            for (int i = 0; i < mobs.Count; i++)
            {
                if (mobs[i] != null && mobs[i].GetComponent<EnemyAI>() != null)
                {
                    mobs[i].GetComponent<EnemyAI>().freeze = timeStopped;
                }
            }

            stoppedMobsCount = mobs.Count;
        }

        if(timeStoppedTimeLeft > 0)
        {            
            timeStoppedTimeLeft -= Time.deltaTime;
        }
        else
        {
            if (timeStopped)
            {
                timeStopped = false;

                List<GameObject> mobs = GameManager.instance.monsters;

                for (int i = 0; i < mobs.Count; i++)
                {
                    if (mobs[i] != null && mobs[i].GetComponent<EnemyAI>() != null)
                    {
                        mobs[i].GetComponent<EnemyAI>().freeze = timeStopped;
                    }
                }
            }
        }

        #endregion

        #region Invincibility cheat

        if (Input.GetKeyDown(KeyCode.O) && Input.GetKeyDown(KeyCode.P))
        {            
            if (!cheatMode)
                stats.Invincibility(1000000);
            else
                stats.invincibilityTime = 0;

            cheatMode = !cheatMode;                       
        }

        #endregion

        if (Input.GetKeyDown(KeyCode.Escape))
            RestartGame();

        stats.damage = weaponM.equippedWeapon.crit ? weaponM.equippedWeapon.criticalDamage : weaponM.equippedWeapon.baseDamage;
        stats.knockbackPower = weaponM.equippedWeapon.knockbackPower;        

        #region Restore health

        if (timeToRH > 0)
            timeToRH -= Time.deltaTime;
        else
        {
            if(RHDecayIntervalTime <= 0)
            {
                RHDecayIntervalTime = RHDecayInterval;

                if (healthToRestore >= RHDecayAmount)
                    healthToRestore -= RHDecayAmount;
                else
                    healthToRestore = 0;
            }
            else
            {
                RHDecayIntervalTime -= Time.deltaTime;
            }
        }

        #endregion
    }

    #region Restore health

    [HideInInspector]
    public int healthToRestore = 0;

    [Header("Restore Health mechanic")]    
    [SerializeField]
    int RHAmount;
    [SerializeField]
    int RHDecayAmount;
    [SerializeField]
    float RHDecayInterval;
    float RHDecayIntervalTime;    
    [SerializeField]
    float timeForRH;
    float timeToRH;

    public void RestoreHealthAfterAttack()
    {
        if (healthToRestore <= 0)
            return;

        int amount = 0;
        if (healthToRestore >= RHAmount)
            amount = RHAmount;
        else
            amount = healthToRestore;
        healthToRestore -= amount;
        stats.curHealth += amount;
    }

    #endregion

    public override bool Damage(int damageTaken, LivingCreature dmgSource, int knockbackPower)
    {
        base.Damage(damageTaken, dmgSource, knockbackPower);

        if (stats.invincible)
            return false;

        healthToRestore += damageTaken;
        timeToRH = timeForRH;
        
        if (timeToRH <= 0)
            timeToRH = timeForRH;

        if(!stats.stunned)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");

            stats.stunned = true;
        }

        // Knockback
        if (knockbackPower != 0 && dmgSource != null)
        {
            float direction = Mathf.Sign(dmgSource.transform.position.x - transform.position.x);
            float velocityX = (direction > 0 ? -1 : 1) * knockbackPower;
            controller.velocity.x += velocityX;
        }

        return true;
    }

    public override void Kill()
    {
        base.Kill();
        Debug.LogError("YOU DIED");
        GameManager.instance.GameOver();
    }

    void RestartGame()
    {
        GameManager.instance.RestartGame();
        Destroy(gameObject);
    }
}
