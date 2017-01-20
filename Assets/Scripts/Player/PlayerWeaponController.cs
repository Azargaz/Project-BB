using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{ 
    int currentAttackNumber = 0;
    int combo = 0;
    bool attacked = true;
    bool chargeBroken = true;
    [HideInInspector]
    public static bool fullyCharged = false;
    bool playerAttackAnimation = false;
    Animator playerAnim;
    WeaponController.Weapon eqWeapon;
    WeaponController.Weapon.Attack curAttack;
    LivingCreature.Statistics stats;
    Player player;
    int attackNumber = 0;

    void Awake()
    {        
        playerAnim = GetComponent<Animator>();
        stats = GetComponent<PlayerCreature>().stats;
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (stats.pause)
            return;

        eqWeapon = WeaponController.wc.equippedWeapon;
        playerAttackAnimation = playerAnim.GetCurrentAnimatorStateInfo(0).IsName("attack_player");        

        if(!playerAttackAnimation)
        {
            WeaponController.wc.eqWeaponCurAttack.chargeTimer = 0;

            if (Input.GetButton("Fire1") && eqWeapon.attacks[0].staminaCost < stats.curStamina)
            {
                attackNumber = 1;                
            }

            if(eqWeapon.attacks.Length > 1)
            {
                if (Input.GetButton("Fire2") && eqWeapon.attacks[1].staminaCost < stats.curStamina)
                {
                    attackNumber = 2;
                }
            }
        }

        if (attackNumber != 0)
        {
            if (eqWeapon.attacks.Length < attackNumber)
                attackNumber = 1;

            if (eqWeapon.attacks[attackNumber - 1].staminaCost > stats.curStamina)
                attackNumber = 0;
        }     

        if (!playerAttackAnimation && attackNumber != 0 && WeaponController.wc.eqWeaponCurAttack.staminaCost <= stats.curStamina)
        {
            if (!WeaponController.wc.eqWeaponCurAttack.needFullCharge || WeaponController.wc.eqWeaponCurAttack.chargedStaminaCost <= stats.curStamina)
            {
                if (attackNumber <= eqWeapon.attacks.Length)
                {
                    StartAttack(attackNumber);
                }

                attackNumber = 0;
            }   
        }

        if (!playerAttackAnimation && stats.animationBusy)            
        {
            playerAnim.SetBool("BreakCharge", true);
            stats.animationBusy = false;
        }

        #region Chargable attacks

        if (playerAttackAnimation && !attacked && stats.stunned)
            BreakCharge();        

        if (playerAttackAnimation && eqWeapon.attacks[currentAttackNumber].chargable && !attacked && curAttack.chargeTimer < curAttack.chargeTime)
        {
            curAttack.chargeTimer += Time.deltaTime;

            if (curAttack.chargedStaminaCost > stats.curStamina)
                BreakCharge();

            switch(currentAttackNumber)
            {
                case 0:
                    {
                        if(!Input.GetButton("Fire1"))
                            BreakCharge();
                        break;
                    }
                case 1:
                    {
                        if (!Input.GetButton("Fire2"))
                            BreakCharge();
                        break;
                    }
            }
        }

        #endregion
    }

    void BreakCharge()
    {
        if (chargeBroken)
            return;

        chargeBroken = true;

        StopCoroutine(AnimationCharge());

        if (curAttack.chargeAnim.Length > 0)
        {
            Animator chargeAnim = curAttack.chargeAnim[combo];
            chargeAnim.SetBool("BreakCharge", true);
        }

        playerAnim.SetFloat("AttackSpeed", curAttack.attackSpeed);
        stats.animationBusy = false;

        if (curAttack.needFullCharge && !fullyCharged)
        {            
            playerAnim.SetBool("BreakCharge", true);            
        }   
    }

    void StartAttack(int _attackNumber)
    {
        currentAttackNumber = _attackNumber - 1;
        WeaponController.wc.currentAttack = currentAttackNumber;
        WeaponController.Weapon.Attack atk = eqWeapon.attacks[currentAttackNumber];
        curAttack = atk;        
        fullyCharged = false;
        WeaponController.wc.RollCritical(currentAttackNumber);

        if (curAttack.numberOfHits <= 1)
            combo = 0;

        if (combo == curAttack.numberOfHits)
            combo = 0;

        playerAnim.SetFloat("AttackId", (float)atk.animationTypes[combo] / 10f);
        playerAnim.SetFloat("AttackSpeed", curAttack.attackSpeed);
        playerAnim.SetBool("BreakCharge", false);
        playerAnim.SetTrigger("Attack");
    }

    #region Animation events

    IEnumerator AnimationCharge()
    {
        attacked = false;
        chargeBroken = false;

        if (curAttack.chargable)
        {
            if(curAttack.chargeAnim.Length > 0)
            {
                Animator chargeAnim = curAttack.chargeAnim[combo];
                chargeAnim.SetBool("BreakCharge", false);
                chargeAnim.SetTrigger("Start");
                chargeAnim.transform.parent.localScale = new Vector2(player.facing, 1);
            }           

            playerAnim.SetFloat("AttackSpeed", 0);
            stats.animationBusy = true;
            curAttack.chargeTimer = 0;
        }

        while (curAttack.chargeTimer < curAttack.chargeTime && curAttack.chargeTime != 0)
            yield return null;

        if(curAttack.chargable)
            fullyCharged = true;

        playerAnim.SetFloat("AttackSpeed", curAttack.attackSpeed);
        stats.animationBusy = false;
    }

    void AnimationAttack()
    {
        DrainStamina();
        SetPlayerStats();
        attacked = true;
        WeaponController.wc.eqWeaponCurAttack.chargeTimer = 0;

        Animator attackAnim = curAttack.aoeAnim[combo];
        attackAnim.SetTrigger("Start");
        attackAnim.transform.parent.localScale = new Vector2(player.facing, 1);
        combo++;

        #region Dash with attack

        if (curAttack.dashWithAttack)
        {
            player.Dash(curAttack.dashDistance, curAttack.dashDirection == WeaponController.Weapon.Attack.DashDirection.normal ? 1 : -1, curAttack.dashFacingBackwards);
        }

        #endregion

        #region Crit color

        if (curAttack.crit)
        {
            Color attackColor = attackAnim.GetComponentInChildren<SpriteRenderer>().color;
            attackColor = new Color(0.9f, 0, 0);
            attackAnim.GetComponentInChildren<SpriteRenderer>().color = attackColor;
        }
        else
        {
            Color attackColor = attackAnim.GetComponentInChildren<SpriteRenderer>().color;
            attackColor = new Color(1, 1, 1);
            attackAnim.GetComponentInChildren<SpriteRenderer>().color = attackColor;
        }

        #endregion        
    }

    void DrainStamina()
    {
        if(fullyCharged)
            stats.curStamina -= eqWeapon.attacks[currentAttackNumber].chargedStaminaCost;
        else
            stats.curStamina -= eqWeapon.attacks[currentAttackNumber].staminaCost;

        stats.DelayStaminaRegen();
    }

    void SetPlayerStats()
    {
        stats.damage = curAttack.baseDamage;
        stats.knockbackPower = curAttack.knockbackPower;

        if (curAttack.crit)
            stats.damage = curAttack.criticalDamage;

        float chargePercent = curAttack.chargeTimer / curAttack.chargeTime;        

        if(curAttack.chargable && curAttack.scaleDamageWithChargeTime)
        {
            if (chargePercent > 0)
            {
                float chargeDmgMultiplier = 1f;

                if (chargePercent >= 0.25f && chargePercent < 0.5f)
                    chargeDmgMultiplier = 1.5f;
                else if (chargePercent >= 0.5f && chargePercent < 0.75f)
                    chargeDmgMultiplier = 2f;
                else if (chargePercent >= 0.7f)
                    chargeDmgMultiplier = 3f;

                if (curAttack.crit)
                    stats.damage = (int)(curAttack.criticalDamage * chargeDmgMultiplier);
                else
                    stats.damage = (int)(curAttack.baseDamage * chargeDmgMultiplier);
            }

            if (fullyCharged)
            {
                stats.damage = curAttack.chargedDamage;
                stats.knockbackPower = curAttack.chargedKnockbackPower;
            }

            if (fullyCharged && curAttack.crit)
                stats.damage = (int)(curAttack.chargedDamage * curAttack.criticalMultiplier);
        }  
        
        if(curAttack.chargable && curAttack.needFullCharge)
        {
            if (fullyCharged)
            {
                stats.damage = curAttack.chargedDamage;
                stats.knockbackPower = curAttack.chargedKnockbackPower;
            }

            if (fullyCharged && curAttack.crit)
                stats.damage = (int)(curAttack.chargedDamage * curAttack.criticalMultiplier);
        }      
    }

    #endregion
}