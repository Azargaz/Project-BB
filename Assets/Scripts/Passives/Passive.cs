using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : MonoBehaviour
{
    public string Name;

    /* State in tree:
    Enabled = can be picked
    Active = has been picked
    Disabled = can't be picked */
    public enum State { Enabled, Active, Disabled };
    public State state;
    public int cost;

    public abstract void Activate();
}
