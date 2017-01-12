using System.Collections;
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
        public bool alive;
        public bool invincible;
        public bool immovable;

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

            invincibilityTime += duration;
        }
    }

    public Statistics stats = new Statistics();
    public GameObject damageDisplay;

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
            if (damageDisplay != null)
            {
                GameObject clone = Instantiate(damageDisplay, transform.position, Quaternion.identity);
                
                if(dmgSource is PlayerCreature)
                    clone.transform.GetChild(0).GetComponent<Text>().text = damageTaken.ToString() + (WeaponManager.wp.equippedWeapon.crit ? "!" : "");
                else
                    clone.transform.GetChild(0).GetComponent<Text>().text = damageTaken.ToString();

                clone.GetComponent<Animator>().SetTrigger("Display");
                Destroy(clone, 1f);
            }
            stats.curHealth -= damageTaken;
            stats.Invincibility();
            return true;
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
