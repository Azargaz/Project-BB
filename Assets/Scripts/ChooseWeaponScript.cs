using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseWeaponScript : MonoBehaviour
{
    public Dropdown weapons;

	void Start ()
    {
        if(weapons != null)
            weapons.value = GameManager.startingWeapon;
    }
}
