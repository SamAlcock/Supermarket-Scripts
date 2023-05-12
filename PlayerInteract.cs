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
            Debug.Log("Fired");
            StartCoroutine(StartCooldown());
        }

    }
    IEnumerator StartCooldown()
    {
        isAvailable = false;
        yield return new WaitForSeconds(0.1f);
        isAvailable = true;
        clicked = false;
    }
}
