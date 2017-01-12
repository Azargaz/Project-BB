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

    #region Time stop
    bool timeStopped = false;
    public float timeStoppedDuration;
    GameObject timeStopHUD;
    float timeStoppedTimeLeft;
    int stoppedMobsCount = 0;
    int stoppedProjectilesCount = 0;
    #endregion

    void Awake()
    {
        timeStopHUD = GameObject.FindGameObjectWithTag("TimeStop");
        DontDestroyOnLoad(this);
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<Player>();
        weaponM = transform.GetComponentInChildren<WeaponManager>();
    }

    protected override void Update()
    {
        if (!stats.alive)
            return;

        base.Update();

        #region Time stop

        if(timeStopHUD == null)
            timeStopHUD = GameObject.FindGameObjectWithTag("TimeStop");
        else
        {
            timeStopHUD.GetComponentInChildren<Text>().text = !timeStopped ? "Time is moving." : "Time is frozen for " + (timeStoppedTimeLeft > 0f ? Mathf.Ceil(timeStoppedTimeLeft) : 0f) + " seconds.";
            timeStopHUD.GetComponent<Animator>().SetBool("TimeStopped", timeStopped);
        }

        if (timeStopped)
        {
            StopMobsAndProjectiles(true);
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

            StopMobsAndProjectiles(false);
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

                StopMobsAndProjectiles(false);
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

    #region Time stop

    void StopMobsAndProjectiles(bool count)
    {
        List<GameObject> mobs = GameManager.instance.monsters;
        List<GameObject> projectiles = GameManager.instance.flyingProjectiles;

        if ((stoppedMobsCount < mobs.Count && count) || !count)
        {
            for (int i = 0; i < mobs.Count; i++)
            {
                if (mobs[i] != null && mobs[i].GetComponent<EnemyAI>() != null)
                {
                    mobs[i].GetComponent<EnemyAI>().freeze = timeStopped;
                }
            }

            if(timeStopped)
                stoppedMobsCount = mobs.Count;
        }

        if ((stoppedProjectilesCount < projectiles.Count && count) || !count)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i] != null && projectiles[i].GetComponent<Projectile>() != null)
                {
                    projectiles[i].GetComponent<Projectile>().freeze = timeStopped;
                }
            }

            if (timeStopped)
                stoppedProjectilesCount = projectiles.Count;
        }
    }

    #endregion

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

        #region Restore Health

        healthToRestore += damageTaken;
        timeToRH = timeForRH;
        
        if (timeToRH <= 0)
            timeToRH = timeForRH;

        #endregion

        #region Stun

        if (!stats.stunned)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");

            stats.stunned = true;
        }

        #endregion

        #region Knockback

        if (knockbackPower != 0 && dmgSource != null)
        {
            float direction = Mathf.Sign(dmgSource.transform.position.x - transform.position.x);
            float velocityX = (direction > 0 ? -1 : 1) * knockbackPower;
            controller.velocity.x += velocityX;
        }

        #endregion

        return true;
    }

    public override void Kill()
    {
        base.Kill();
        anim.SetTrigger("Death");
        Debug.LogError("YOU DIED");
        StartCoroutine(GameManager.instance.GameOver());
    }

    void RestartGame()
    {
        GameManager.instance.RestartGame();
        Destroy(gameObject);
    }
}
