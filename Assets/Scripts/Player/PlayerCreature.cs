using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCreature : LivingCreature
{
    Animator anim;
    Player controller;

    void Awake()
    {
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<Player>();
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

    public override bool Damage(int damageTaken, LivingCreature dmgSource)
    {
        base.Damage(damageTaken, dmgSource);

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
        if (stats.knockbackDistance != 0 && dmgSource != null)
        {
            float direction = Mathf.Sign(dmgSource.transform.position.x - transform.position.x);
            float velocityX = (direction > 0 ? -1 : 1) * stats.knockbackDistance;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
