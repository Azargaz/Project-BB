using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeStop : MonoBehaviour
{
    public bool stopTime = false;
    float staminaDrainPerSec;
    bool timeStopped = false;
    GameObject timeStopHUD;
    float timeStoppedTimeLeft;
    int stoppedMobsCount = 0;
    int stoppedProjectilesCount = 0;
    PlayerCreature player;

    void Awake()
    {
        timeStopHUD = GameObject.FindGameObjectWithTag("TimeStop");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCreature>();
    }

	void Update ()
    {
        if (GameManager.instance.pause)
            return;        

        if (timeStopHUD == null)
            timeStopHUD = GameObject.FindGameObjectWithTag("TimeStop");
        else
        {
            //timeStopHUD.GetComponentInChildren<Text>().text = !timeStopped ? "Time is moving." : "Time is frozen for " + (timeStoppedTimeLeft > 0f ? Mathf.Ceil(timeStoppedTimeLeft) : 0f) + " seconds.";
            timeStopHUD.GetComponent<Animator>().SetBool("TimeStopped", timeStopped);
        }

        if (timeStopped)
        {
            StopMobsAndProjectiles(true);
        }

        if (stopTime && player.stats.curStamina > 0)
        {
            staminaDrainPerSec = WeaponController.wc.eqWeaponCurAttack.staminaCost;

            stopTime = false;
            timeStopped = !timeStopped;

            if (!timeStopped)
            {                          
                player.stats.invincibilityTime = 0;
                timeStoppedTimeLeft = 0;
            }

            StopMobsAndProjectiles(false);
        }

        if (player.stats.curStamina > 0 && timeStopped)
        {
            timeStoppedTimeLeft -= Time.deltaTime;
            player.stats.curStamina -= staminaDrainPerSec * Time.deltaTime;
            player.stats.DelayStaminaRegen();
            timeStoppedTimeLeft = player.stats.curStamina / staminaDrainPerSec;
            player.stats.Invincibility(timeStoppedTimeLeft);
        }
        else if(player.stats.curStamina <= 0)
        {
            if (timeStopped)
            {
                stopTime = false;
                timeStopped = false;
                timeStoppedTimeLeft = 0;
                player.stats.invincibilityTime = 0;
                StopMobsAndProjectiles(false);
            }
        }
    }

    void StopMobsAndProjectiles(bool count)
    {
        List<GameObject> mobs = GameManager.instance.monsters;
        List<GameObject> projectiles = GameManager.instance.flyingProjectiles;

        if ((stoppedMobsCount < mobs.Count && count) || !count)
        {
            for (int i = 0; i < mobs.Count; i++)
            {
                if (mobs[i] != null && mobs[i].GetComponent<EnemyAI>() != null)
                {
                    mobs[i].GetComponent<EnemyAI>().freeze = timeStopped;
                }
            }

            if (timeStopped)
                stoppedMobsCount = mobs.Count;
        }

        if ((stoppedProjectilesCount < projectiles.Count && count) || !count)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i] != null && projectiles[i].GetComponent<Projectile>() != null)
                {
                    projectiles[i].GetComponent<Projectile>().freeze = timeStopped;
                }
            }

            if (timeStopped)
                stoppedProjectilesCount = projectiles.Count;
        }
    }

    void AnimationStopTime()
    {
        stopTime = true;
    }
}
