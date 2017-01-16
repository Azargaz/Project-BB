using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayOnHover : MonoBehaviour
{
    public GameObject displayOnHover;

    public void OnEnter()
    {
        if(displayOnHover != null)
            displayOnHover.SetActive(true);
    }

    public void OnExit()
    {
        if (displayOnHover != null)
            displayOnHover.SetActive(false);
    }
}
