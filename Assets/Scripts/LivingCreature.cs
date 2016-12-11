using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingCreature : MonoBehaviour
{
    [System.Serializable]
    public class Statistics
    {
        public int curHealth;
        public int maxHealth;

        public int regenHealthAmount;
        public float regenHealthRate;
        public float regenHealthTime = 0;

        public float curStamina;
        public float maxStamina;

        public float regenStaminaAmount;
        public float regenStaminaRate;
        public float regenStaminaTime = 0;
        public float regenStaminaDelay;

        public bool stunned;
        public bool animationBusy; // for animations
        public bool alive;
        public bool invincible;

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

        if (stats.stunned)
            return;
    }

    public virtual void Damage(int damageTaken, bool stun)
    {
        if (stats.invincible)
            return;

        stats.curHealth -= damageTaken;
    }

    public IEnumerator DamageOverTime(int ticks, int damagePerTick, float damageInterval, bool _stun)
    {
        Damage(damagePerTick, _stun);

        yield return new WaitForSeconds(damageInterval);

        if (ticks > 1)
            StartCoroutine(DamageOverTime(ticks - 1, damagePerTick, damageInterval, _stun));
    }

    public virtual void Kill()
    {
        stats.alive = false;
    }

    protected void AnimationStunnedEnd()
    {
        stats.stunned = false;
    }

    protected void AnimationBusyEnd()
    {
        stats.animationBusy = false;
    }
}
