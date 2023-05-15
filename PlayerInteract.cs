using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public bool clicked = false;
    bool isAvailable = true;

    void OnFire()
    {
        if (isAvailable)
        {
            clicked = true;
            StartCoroutine(StartCooldown());
        }

    }
    IEnumerator StartCooldown() // Click cooldown to stop too much spamming
    {
        isAvailable = false;
        yield return new WaitForSeconds(0.1f);
        isAvailable = true;
        clicked = false;
    }
}
