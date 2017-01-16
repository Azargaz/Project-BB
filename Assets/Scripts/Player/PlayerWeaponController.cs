using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{ 
    int currentAttackNumber = 0;
    int combo = 0;
    bool attacked = true;
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

        if(attacked)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                attackNumber = 1;                
            }

            if (Input.GetButtonDown("Fire2"))
            {
                attackNumber = 2;
            }
        }

        if (eqWeapon.attacks.Length >= attackNumber && attackNumber != 0)
        {
            if(eqWeapon.attacks[attackNumber - 1].staminaCost > stats.curStamina)
                attackNumber = 0;
        }            

        if (!playerAttackAnimation && attackNumber != 0 && WeaponController.wc.eqWeaponCurAttack.staminaCost <= stats.curStamina)
        {
            if(attackNumber <= eqWeapon.attacks.Length)
            {
                StartAttack(attackNumber);
            }

            attackNumber = 0;
        }

        if (!playerAttackAnimation && stats.animationBusy)
            stats.animationBusy = false;

        if (stats.stunned)
            attacked = true;

        #region Chargable attacks

        if (playerAttackAnimation && stats.stunned)
            BreakCharge();

        

        if (playerAttackAnimation && eqWeapon.attacks[currentAttackNumber].chargable && !attacked)
        {
            switch(currentAttackNumber)
            {
                case 0:
                    {
                        if(Input.GetButtonUp("Fire1"))
                            BreakCharge();
                        break;
                    }
                case 1:
                    {
                        if (Input.GetButtonUp("Fire2"))
                            BreakCharge();
                        break;
                    }
            }
        }

        #endregion
    }

    void BreakCharge()
    {
        StopCoroutine(AnimationCharge());

        if (curAttack.chargeAnim.Length > 0)
        {
            Animator chargeAnim = curAttack.chargeAnim[combo];
            chargeAnim.SetBool("BreakCharge", true);
        }

        playerAnim.SetFloat("AttackSpeed", 1);
        playerAnim.SetBool("BreakCharge", true);
        stats.animationBusy = false;
    }

    void StartAttack(int _attackNumber)
    {
        currentAttackNumber = _attackNumber - 1;
        WeaponController.wc.currentAttack = currentAttackNumber;
        WeaponController.Weapon.Attack atk = eqWeapon.attacks[currentAttackNumber];
        curAttack = atk;
        attacked = false;
        WeaponController.wc.RollCritical(currentAttackNumber);

        if (curAttack.numberOfHits <= 1)
            combo = 0;

        if (combo == curAttack.numberOfHits)
            combo = 0;

        playerAnim.SetFloat("AttackId", (float)atk.type[combo] / 10f);
        playerAnim.SetFloat("AttackSpeed", curAttack.attackSpeed);
        playerAnim.SetBool("BreakCharge", false);
        playerAnim.SetTrigger("Attack");
    }

    #region Animation events

    IEnumerator AnimationCharge()
    {
        if(curAttack.chargable)
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
        }

        yield return new WaitForSeconds(eqWeapon.attacks[currentAttackNumber].chargeTime);

        playerAnim.SetFloat("AttackSpeed", curAttack.attackSpeed);
        stats.animationBusy = false;
    }

    void AnimationAttack()
    {
        DrainStamina();
        attacked = true;

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
        stats.curStamina -= eqWeapon.attacks[currentAttackNumber].staminaCost;
        stats.DelayStaminaRegen();
    }

    #endregion
}