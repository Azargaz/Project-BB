using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Dashing")]
    public int dashCost;
    [Range(0, 100)]
    public float dashLength = 4;
    [Range(0, 100)]
    public float trailLength = 4;

    [Header("Movement")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    [HideInInspector]
    public Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerCreature creature;
    LivingCreature.Statistics stats;
    WeaponManager weaponM;
    [HideInInspector]
    public bool secondaryAttack = false;
    bool attacked = false;
    Transform trail;
    Animator anim;
    public int facing = 1;
    public bool freeze;
    Vector2 mousePos;
    int mouseSide;

    void Start()
    {
        trail = transform.FindChild("Trail");
        controller = GetComponent<Controller2D>();
        creature = GetComponent<PlayerCreature>();
        stats = creature.stats;
        anim = GetComponent<Animator>();
        weaponM = transform.GetComponentInChildren<WeaponManager>();        

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        if (!creature.stats.alive)
        {
            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        if (freeze || !creature.stats.alive)
            return;

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
            controller.jumpDown = false;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseSide = (mousePos.x - transform.position.x) >= 0 ? 1 : -1;

        if (InputControl.UsingGamepad())
            mouseSide = Input.GetAxisRaw("XC Right Stick X") == 0 ? 0 : Input.GetAxisRaw("XC Right Stick X") > 0 ? 1 : -1;

        // Dashing            
        if (Input.GetButtonDown("Dash") && stats.curStamina >= dashCost)
        {
            anim.SetTrigger("Dashing");
        }

        #region Attacking 

        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            attacked = false;            

            if (Input.GetButtonDown("Fire1") && stats.curStamina >= weaponM.equippedWeapon.useStaminaCost)
            {               
                WeaponManager.wp.RollCritical(true);
                anim.SetFloat("AttackSpeed", weaponM.equippedWeapon.attackSpeed);
                anim.SetFloat("AttackId", (float)(weaponM.weapons[weaponM.currentWeapon].attackType + (weaponM.equippedWeapon.comboHits > 0 ? hitCount : 0)) / 10f);
                secondaryAttack = false;
                anim.SetTrigger("Attack");

                if (weaponM.equippedWeapon.chargable)
                {
                    AttackCharge();
                }
            }
            else if (Input.GetButtonDown("Fire2") && weaponM.equippedWeapon.secondaryAttack && stats.curStamina >= weaponM.equippedWeapon.secondaryUseStaminaCost)
            {
                WeaponManager.wp.RollCritical(false);
                anim.SetFloat("AttackSpeed", weaponM.equippedWeapon.secondaryAttackSpeed);
                anim.SetFloat("AttackId", (float)(weaponM.equippedWeapon.secondaryType + (weaponM.equippedWeapon.secondaryComboHits > 0 ? secondaryHitCount : 0)) / 10f);
                secondaryAttack = true;
                anim.SetTrigger("Attack");

                if (weaponM.equippedWeapon.secondaryChargable)
                {
                    AttackCharge();
                }
            }            
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            if (weaponM.equippedWeapon.chargable)
            {
                if (Input.GetButtonUp("Fire1") && !attacked)
                {
                    anim.SetTrigger("BreakCharging");
                    hitCount = 0;
                    GameObject aoe = weaponM.equippedWeapon.aoeObject[0];
                    aoe.GetComponentInChildren<Animator>().SetTrigger("BreakCharging");
                }
            }

            if (weaponM.equippedWeapon.secondaryChargable)
            {
                if (Input.GetButtonUp("Fire2") && !attacked)
                {
                    anim.SetTrigger("BreakCharging");
                    secondaryHitCount = 0;
                    GameObject aoe = weaponM.equippedWeapon.secondaryAoeObject[0];
                    aoe.GetComponentInChildren<Animator>().SetTrigger("BreakCharging");
                }
            }
        }

        #endregion

        #region Animation, stunned, animationBusy, flipping sprites

        anim.SetFloat("Input", Mathf.Abs(input.x));
        anim.SetBool("Grounded", controller.collisions.below);

        #region Fliping sprite

        if (input.x != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {
            facing = input.x > 0 ? 1 : -1;
        }

        if (input.x == 0 && mouseSide != 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_player"))
        {            
            facing = mouseSide;
        }

        FlipAllSprites();

        #endregion

        if (creature.stats.stunned || creature.stats.animationBusy)
        {
            input = Vector2.zero;
        }

        #endregion

        #region Jumping/Jumping down platforms

        if (input.y < 0)
            controller.jumpDown = true;

        if (Input.GetButtonDown("Jump") && controller.collisions.below && !anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
        {
            if (controller.collisions.below)
            {
                velocity.y = maxJumpVelocity;
            }                       
        }
        if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
        }

        #endregion

        #region Movement stuff + dash trail

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        #region Dash trail

        for (int i = 0; i < trail.childCount; i++)
        {
            trail.GetChild(i).localPosition = -velocity * i / (100 - trailLength + 1);
        }

        #endregion

        // Dash without gravity below VVVVV

        //if (!anim.GetCurrentAnimatorStateInfo(0).IsName("dash_player"))
        //    velocity.y += gravity * Time.deltaTime;
        //else
        //    velocity.y = 0;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        #endregion
    }

    void FlipAllSprites()
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        sprites.Add(GetComponent<SpriteRenderer>());
        sprites.AddRange(transform.FindChild("Trail").GetComponentsInChildren<SpriteRenderer>());
        sprites.Add(transform.FindChild("Armor").GetComponent<SpriteRenderer>());

        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].flipX = facing == -1;
        }
    }

    #region AnimationEvents

    void AnimationDash()
    {
        AnimationDrainStaminaDash();
        AnimationDashStep();
        creature.AnimationInvincibility(0.1f);
    }

    void AnimationDrainStamina()
    {
        if(secondaryAttack)
            stats.curStamina -= weaponM.equippedWeapon.secondaryUseStaminaCost;
        else
            stats.curStamina -= weaponM.equippedWeapon.useStaminaCost;

        stats.DelayStaminaRegen();
    }

    void AnimationDrainStaminaDash()
    {
        stats.curStamina -= dashCost;
        stats.DelayStaminaRegen();
    }

    int hitCount = 0;
    int secondaryHitCount = 0;

    void AnimationAttackSlash()
    {
        AnimationDrainStamina();
        attacked = true;

        if (mouseSide != 0)
            facing = mouseSide;

        if (!secondaryAttack)
        {           
            WeaponManager.Weapon eqW = weaponM.equippedWeapon;
            if (eqW.comboHits <= 0)
                hitCount = 0;

            GameObject aoe = weaponM.equippedWeapon.aoeObject[hitCount > 0 ? hitCount : 0];
            AttackSlash(eqW.comboHits, ref hitCount, eqW.attackSpeed, eqW.crit, aoe, eqW.weaponSpecialActive, eqW.weaponSpecialDelay);
            return;
        }
        else
        {
            WeaponManager.Weapon eqW = weaponM.equippedWeapon;
            if (!eqW.secondaryAttack)
                return;

            if (eqW.secondaryComboHits <= 0)
                secondaryHitCount = 0;

            GameObject aoe = weaponM.equippedWeapon.secondaryAoeObject[secondaryHitCount > 0 ? secondaryHitCount : 0];
            AttackSlash(eqW.secondaryComboHits, ref secondaryHitCount, eqW.secondaryAttackSpeed, eqW.secondaryCrit, aoe, eqW.weaponSpecialActive, eqW.weaponSpecialDelay);
            return;
        }        
    }

    void AttackCharge()
    {
        if (mouseSide != 0)
            facing = mouseSide;

        if (!secondaryAttack)
        {
            WeaponManager.Weapon eqW = weaponM.equippedWeapon;

            if (eqW.comboHits <= 0)
                hitCount = 0;

            GameObject aoe = weaponM.equippedWeapon.aoeObject[0];
            AttackSlash(eqW.comboHits, ref hitCount, eqW.attackSpeed, eqW.crit, aoe, eqW.weaponSpecialActive, eqW.weaponSpecialDelay);
            return;
        }
        else
        {
            WeaponManager.Weapon eqW = weaponM.equippedWeapon;
            if (!eqW.secondaryAttack)
                return;

            if (eqW.secondaryComboHits <= 0)
                secondaryHitCount = 0;

            GameObject aoe = weaponM.equippedWeapon.secondaryAoeObject[0];
            AttackSlash(eqW.secondaryComboHits, ref secondaryHitCount, eqW.secondaryAttackSpeed, eqW.secondaryCrit, aoe, eqW.weaponSpecialActive, eqW.weaponSpecialDelay);
            return;
        }
    }

    void AttackSlash(int comboHits, ref int _hitCount, float atkSpeed, bool crit, GameObject aoeObject, bool special, float specialDelay)
    {        
        int numberOfHits = comboHits;

        if (numberOfHits <= 0)
            _hitCount = 0;

        if (aoeObject != null)
        {
            GameObject swing = aoeObject;
            Animator swingAnim = swing.transform.GetChild(0).GetComponent<Animator>();
            swingAnim.SetTrigger("Swing");
            swingAnim.SetFloat("AttackSpeed", atkSpeed);
            swing.transform.localScale = new Vector2(facing, 1);

            if (numberOfHits > 0)
            {
                _hitCount++;

                if (_hitCount > numberOfHits)
                    _hitCount = 0;
            }
            else
                _hitCount = 0;

            if (crit)
            {
                Color swingColor = swing.GetComponentInChildren<SpriteRenderer>().color;
                swingColor = new Color(0.9f, 0, 0);
                swing.GetComponentInChildren<SpriteRenderer>().color = swingColor;
            }
            else
            {
                Color swingColor = swing.GetComponentInChildren<SpriteRenderer>().color;
                swingColor = new Color(1, 1, 1);
                swing.GetComponentInChildren<SpriteRenderer>().color = swingColor;
            }
        }

        if (special)
        {
            float delaySpecialAttack = specialDelay;
            StartCoroutine(AnimationWeaponAttackSpecial(delaySpecialAttack));
        }
    }

    IEnumerator AnimationWeaponAttackSpecial(float delay)
    {
        yield return new WaitForSeconds(delay);

        switch (weaponM.currentWeapon)
        {
            //TEMPLATE
            //Greatsword
            case 0:
                {

                    break;
                }
            //Daggers
            case 2:
                {
                    AnimationAttackStep(weaponM.daggersDashDistance);
                    facing = -facing;
                    break;
                }
            //Katana
            case 8:
                {
                    AnimationAttackStep(weaponM.katanaDashDistance);
                    facing = -facing;
                    break;
                }
        }
    }

    void AnimationDashStep()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0)
            velocity.x = dashLength * input;
        else
            velocity.x = dashLength * facing;

        controller.Move(velocity * Time.deltaTime);
    }    

    void AnimationAttackStep(int distance)
    { 
        velocity.x = distance * facing;

        controller.Move(velocity * Time.deltaTime);
    }

    #endregion
}