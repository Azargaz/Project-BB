﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public float knockbackDistance;

        public bool stunned;
        public bool animationBusy; // for animations
        public bool alive;
        public bool invincible;

        public float defaultInvincibilityDuration;        

        [Header("Dont change - info only")]
        public int curHealth;
        public float curStamina;
        public float regenHealthTime = 0;
        public float regenStaminaTime = 0;
        public float invincibilityTime = 0;

        public void Initialize()
        {
            curHealth = maxHealth;
            curStamina = maxStamina;
            stunned = false;
            alive = true;
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

    protected virtual void Update()
    {
        if (!stats.alive)
            return;

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

        if (transform.FindChild("Armor") != null)
            armorColor = transform.FindChild("Armor").GetComponent<SpriteRenderer>();

        if (stats.invincibilityTime > 0)
        {            
            stats.invincible = true;
            stats.invincibilityTime -= Time.deltaTime;
            color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
        }
        else if(stats.invincibilityTime <= 0)
        {
            stats.invincible = false;
            color = new Color(1f, 1f, 1f, 1f);
        }

        GetComponent<SpriteRenderer>().color = color;

        if (armorColor != null)
            armorColor.color = color;
    }

    public virtual bool Damage(int damageTaken, LivingCreature dmgSource)
    {
        if (stats.invincible)
        {
            return false;
        }
        else
        {
            stats.curHealth -= damageTaken;
            stats.Invincibility();
            return true;
        }               
    }

    public virtual void Kill()
    {
        stats.alive = false;
    }

    #region Animation events

    protected void AnimationStunnedEnd()
    {
        stats.stunned = false;
    }

    protected void AnimationBusyEnd()
    {
        stats.animationBusy = false;
    } 

    #endregion
}
