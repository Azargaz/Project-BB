using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    GameObject player;
    public GameObject prompt;

    void Update()
    {
        if (player != null)
        {
            if (Input.GetButtonDown("PickupWeapon"))
            {
                GoToTheNextLevel();
            }
        }
    }

    void GoToTheNextLevel()
    {
        WeaponPedestalController._playerHasChosenWeapon = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.player.transform.position = new Vector3(0, 6, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            player = other.gameObject;
            prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            player = null;
            prompt.SetActive(false);
        }
    }
}
