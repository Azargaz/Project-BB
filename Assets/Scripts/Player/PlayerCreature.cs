using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreature : LivingCreature
{
    Animator anim;
    Player controller;
    [HideInInspector]
    WeaponController wc;
    bool cheatMode = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        anim = GetComponent<Animator>();
        stats.Initialize();
        controller = GetComponent<Player>();
        wc = GetComponentInChildren<WeaponController>();
    }

    protected override void Update()
    {
        if (!stats.alive)
            return;

        base.Update();

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

        if (Input.GetButtonDown("Submit"))
            RestartGame();

        WeaponController.Weapon.Attack atk = wc.eqWeaponCurAttack;
        stats.damage = atk.crit ? atk.criticalDamage : atk.baseDamage;
        stats.knockbackPower = atk.knockbackPower;

        if (wc.equippedWeapon.Name.Contains("Scythe"))
            RestoreHeatlhOn = true;
        else
            RestoreHeatlhOn = false;

        #region Restore health

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

    #region Restore health
    
    [HideInInspector]
    public int healthToRestore = 0;

    [Header("Restore Health mechanic")]
    [SerializeField]
    bool RestoreHeatlhOn = false;
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
    [SerializeField]
    GameObject healthRestoreDisplay;

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
        stats.curHealth += amount;

        if(healthRestoreDisplay != null)
        {
            GameObject clone = Instantiate(healthRestoreDisplay, transform.position, Quaternion.identity);
            Text txt = clone.transform.GetChild(0).GetComponent<Text>();

            txt.text = "+" + amount;
            clone.GetComponent<Animator>().SetTrigger("Display");
            clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
            Destroy(clone, 1f);
        }
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

    void RestartGame()
    {
        GameManager.instance.RestartGame();
        Destroy(gameObject);
    }
}
