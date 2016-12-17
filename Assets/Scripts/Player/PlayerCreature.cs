using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCreature : LivingCreature
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        stats.Initialize();
    }

    protected override void Update()
    {
        base.Update();

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

    public override bool Damage(int damageTaken, int poiseDamage, LivingCreature dmgSource)
    {
        base.Damage(damageTaken, poiseDamage, dmgSource);

        healthToRestore += damageTaken;
        timeToRH = timeForRH;

        if (timeToRH <= 0)
            timeToRH = timeForRH;

        if(!stats.stunned && stats.poise <= 0)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");

            stats.stunned = true;
            stats.poise = stats.maxPoise;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
