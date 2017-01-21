using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayOnHover : EventTrigger
{
    public GameObject displayOnHover;
    bool awaken = false;

    public void Start()
    {
        if(displayOnHover != null)
        {
            if (displayOnHover.activeInHierarchy)
                displayOnHover.SetActive(false);

            awaken = true;
        }
    }

    void OnEnable()
    {
        if (displayOnHover != null && awaken)
        {
            displayOnHover.SetActive(false);
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        if (displayOnHover != null)
            displayOnHover.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData data)
    {
        if (displayOnHover != null)
            displayOnHover.SetActive(false);
    }
}
