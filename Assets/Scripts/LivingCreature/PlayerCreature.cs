using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreature : LivingCreature
{
    Animator anim;
    Player controller;

    [Header("Health Potion")]
    [SerializeField]
    HealthPotion hPotion;

    [System.Serializable]
    public class HealthPotion
    {
        public int maxUses;
        [HideInInspector]
        public int uses;
        public int healAmount;
        public int killsPerRestoredUse;
        int killCount = 0;

        public void updateUses()
        {
            if (killCount >= killsPerRestoredUse && uses < maxUses)
            {
                uses++;
                killCount -= killsPerRestoredUse;
            }
        }

        public void addKills(int amount)
        {
            if (uses == maxUses)
                return;

            killCount += amount;
        }

        public void Initialize()
        {
            uses = maxUses;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this);

        anim = GetComponent<Animator>();
        controller = GetComponent<Player>();

        stats.Initialize();        
        hPotion.Initialize();
    }

    protected override void Update()
    {
        if (!stats.alive)
            return;

        base.Update();

        #region Health potion

        if (Input.GetKeyDown(KeyCode.E))
            usePotion();

        hPotion.updateUses();

        #endregion

        #region Restore health

        RestoreHeatlhOn = WeaponController.wc.equippedWeapon.restoreHealthMechanic;

        if(RestoreHeatlhOn)
        {
            if (timeToRH > 0)
                timeToRH -= Time.deltaTime;
            else
            {
                if (RHDecayIntervalTime <= 0)
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
        }
        else
        {
            timeToRH = 0;
            healthToRestore = 0;
        }

        #endregion
    }

    #region Health potion

    public void usePotion()
    {
        if (stats.curHealth >= stats.maxHealth)
            return;

        if (hPotion.uses <= 0)
            return;

        hPotion.uses--;
        Heal(hPotion.healAmount);
    }

    public int GetPotionMaxUses()
    {
        return hPotion.maxUses;
    }

    public int GetPotionUses()
    {
        return hPotion.uses;
    }

    public void AddPotionKillCount(int amount)
    {
        hPotion.addKills(amount);
    }

    #endregion

    #region Restore health

    [HideInInspector]
    public int healthToRestore = 0;

    [Header("Restore Health mechanic")]
    public bool RestoreHeatlhOn = false;
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
        if (!RestoreHeatlhOn)
            return;

        if (healthToRestore <= 0)
            return;

        int amount = 0;
        if (healthToRestore >= RHAmount)
            amount = RHAmount;
        else
            amount = healthToRestore;
        healthToRestore -= amount;
        Heal(amount);
    }

    #endregion

    public override bool Damage(int damageTaken, LivingCreature dmgSource, int knockbackPower)
    {
        base.Damage(damageTaken, dmgSource, knockbackPower);

        if (stats.invincible)
            return false;

        #region Restore Health

        if(RestoreHeatlhOn)
        {
            healthToRestore += damageTaken;
            timeToRH = timeForRH;

            if (timeToRH <= 0)
                timeToRH = timeForRH;
        }

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
        CurrencyController.CC.ResetCurrency();
        anim.SetTrigger("Death");
        Debug.LogError("YOU DIED");
        StartCoroutine(GameManager.instance.GameOver());
    }
}
