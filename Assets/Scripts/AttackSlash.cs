using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlash : MonoBehaviour
{
    void SelfDestruct()
    {
        Destroy(transform.parent.gameObject);
    }
}
