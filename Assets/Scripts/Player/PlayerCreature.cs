using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCreature : LivingCreature
{
    Animator anim;
    public int dashCost;
    public int attack1Cost;

    void Awake()
    {
        anim = GetComponent<Animator>();

        stats.Initialize();
    }

    protected override void Update()
    {
        base.Update();

        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
            stats.invincible = false;

        // Dashing            
        if (Input.GetButtonDown("Jump") && stats.curStamina >= dashCost)
        {
            anim.SetTrigger("Dashing");
        }

        // Attacking  
        if (Input.GetButtonDown("Fire1") && stats.curStamina >= attack1Cost)
        {
            anim.SetTrigger("Attack");
        }
    }

    public override void Damage(int damageTaken, bool stun, int poiseDamage)
    {
        base.Damage(damageTaken, stun, poiseDamage);

        if(!stats.stunned && stun && stats.poise <= 0)
        {
            if (anim != null)
                anim.SetTrigger("Stunned");

            stats.stunned = true;
            stats.poise = stats.maxPoise;
        }
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

    #region Animation events

    void AnimationInvincibility()
    {
        stats.invincible = !stats.invincible;
    }

    public enum StaminaCosts { dash, attack1 };

    void AnimationDrainStamina(StaminaCosts cost)
    {
        switch(cost)
        {
            case StaminaCosts.dash:
                {
                    stats.curStamina -= dashCost;
                    break;
                }
            case StaminaCosts.attack1:
                {
                    stats.curStamina -= attack1Cost;
                    break;
                }
        }

        stats.DelayStaminaRegen();
    }

    #endregion
}
