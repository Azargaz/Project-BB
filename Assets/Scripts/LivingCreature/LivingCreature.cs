﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class LivingCreature : MonoBehaviour
{
    [System.Serializable]
    public class Statistics
    {        
        public int maxHealth;
        public float maxStamina;

        public int regenHealthAmount;
        public float regenHealthRate;     

        public float regenStaminaAmount;
        public float regenStaminaRate;        
        public float regenStaminaDelay;

        public int damage;
        public int knockbackPower = 10;

        public bool stunned;
        public bool animationBusy; // for animations
        public bool pause; // for pausing game
        public bool alive;
        public bool invincible;
        public bool immovable;

        public float defaultInvincibilityDuration = 0.5f;        

        [Header("Dont change - info only")]
        public int curHealth;
        public float curStamina;
        public float regenHealthTime = 0;
        public float regenStaminaTime = 0;
        public float invincibilityTime = 0;

        int startingHealth;
        float startingStamina;

        public void Initialize()
        {
            startingHealth = maxHealth;
            startingStamina = maxStamina;
            curHealth = maxHealth;
            curStamina = maxStamina;
            stunned = false;
            alive = true;
        }

        public void Reset()
        {
            alive = true;
            stunned = false;
            animationBusy = false;
            invincible = false;
            immovable = false;
            maxHealth = startingHealth;
            maxStamina = startingStamina;
            curHealth = maxHealth;
            curStamina = maxStamina;
            invincibilityTime = 0;
        }

        public void RegenHealth()
        {
            if (curHealth == maxHealth)
                return;

            if (regenHealthTime <= 0)
            {
                curHealth += regenHealthAmount;
                regenHealthTime = regenHealthRate;
            }
            else
            {
                regenHealthTime -= Time.deltaTime;
            }
        }

        public void RegenStamina()
        {
            if (curStamina == maxStamina)
                return;

            if (regenStaminaTime <= 0)
            {
                curStamina += regenStaminaAmount;
                regenStaminaTime = regenStaminaRate;
            }
            else
            {
                regenStaminaTime -= Time.deltaTime;
            }
        }

        public void DelayStaminaRegen()
        {
            regenStaminaTime = regenStaminaDelay;
        }

        public void Invincibility(float duration = 0)
        {
            if(duration == 0)
                duration = defaultInvincibilityDuration;

            invincibilityTime = duration;
        }
    }

    public Statistics stats = new Statistics();
    public GameObject damageDisplay;
    public GameObject healDisplay;

    void Start()
    {
        stats.Initialize();
    }

    protected virtual void Update()
    {
        if (!stats.alive)
            return;

        if (stats.pause)
            return;

        if (transform.position.y < -50)
        {
            Kill();
        }

        if (stats.curHealth <= 0)
            Kill();        

        stats.RegenHealth();
        stats.RegenStamina();

        if (stats.curHealth > stats.maxHealth)
            stats.curHealth = stats.maxHealth;
        if (stats.curStamina > stats.maxStamina)
            stats.curStamina = stats.maxStamina;
        if (stats.curStamina < 0)
            stats.curStamina = 0;

        Color color = GetComponent<SpriteRenderer>().color;
        SpriteRenderer armorColor = null;

        if (transform.Find("Armor") != null)
            armorColor = transform.Find("Armor").GetComponent<SpriteRenderer>();

        if (stats.invincibilityTime > 0)
        {            
            stats.invincible = true;
            stats.invincibilityTime -= Time.deltaTime;
            color = new Color(color.r, color.g, color.b, 0.7f);
        }
        else if(stats.invincibilityTime <= 0)
        {
            stats.invincible = false;
            color = new Color(color.r, color.g, color.b, 1f);
        }

        GetComponent<SpriteRenderer>().color = color;

        if (armorColor != null)
            armorColor.color = color;
    }

    public virtual bool Damage(int damageTaken, LivingCreature dmgSource, int knockbackPower)
    {
        if (stats.invincible)
        {
            return false;
        }
        else
        {
            if (damageTaken > stats.curHealth)
                damageTaken = stats.curHealth;

            if (damageDisplay != null)
            {
                GameObject clone = Instantiate(damageDisplay, transform.position, Quaternion.identity);
                Text txt = clone.transform.GetChild(0).GetComponent<Text>();

                if (dmgSource is PlayerCreature)
                    txt.text = (WeaponController.wc.eqWeaponCurAttack.crit ? "<color=yellow>CRIT " : "") + damageTaken.ToString() + (WeaponController.wc.eqWeaponCurAttack.crit ? "!</color>" : "");
                else
                    txt.text = damageTaken.ToString();

                clone.GetComponent<Animator>().SetTrigger("Display");
                clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
                Destroy(clone, 1f);
            }
            stats.curHealth -= damageTaken;
            stats.Invincibility();
            return true;
        }               
    }

    public virtual void Heal(int healAmount)
    {
        if (stats.curHealth >= stats.maxHealth)
            return;

        if (healAmount > stats.maxHealth - stats.curHealth)
            healAmount = stats.maxHealth - stats.curHealth;

        stats.curHealth += healAmount;

        if (healDisplay != null)
        {
            GameObject clone = Instantiate(healDisplay, transform.position, Quaternion.identity);
            Text txt = clone.transform.GetChild(0).GetComponent<Text>();
            
            txt.text = "+" + healAmount;

            clone.GetComponent<Animator>().SetTrigger("Display");
            clone.transform.SetParent(GameObject.Find("DamageNumbers").transform);
            Destroy(clone, 1f);
        }
    }

    public virtual void Kill()
    {
        stats.alive = false;
    }

    #region Animations events

    protected void AnimationStunnedEnd()
    {
        stats.stunned = false;
    }

    protected void AnimationBusyStart()
    {
        stats.animationBusy = true;
    }

    protected void AnimationBusyEnd()
    {
        stats.animationBusy = false;
    }

    protected void AnimationImmovableStart()
    {
        stats.immovable = true;
    }

    protected void AnimationImmovableEnd()
    {
        stats.immovable = false;
    }

    public void AnimationInvincibility(float dur)
    {
        stats.Invincibility(dur);
    }

    void AnimationDeath()
    {
        Destroy(gameObject);
    }

    #endregion
}
