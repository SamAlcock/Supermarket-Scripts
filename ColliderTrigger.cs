using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    ItemSearch itemSearch;
    PlayerInteract playerInteract;
    bool clicked;
    private void Start()
    {
        GameObject Player = GameObject.Find("Player");
        playerInteract = Player.GetComponent<PlayerInteract>();
    }
    void Update()
    {
        clicked = playerInteract.clicked; // Check if user has clicked
    }
    private void OnTriggerStay(Collider other)
    {
        GameObject Supermarket = GameObject.Find("Supermarket");
        itemSearch = Supermarket.GetComponent<ItemSearch>();
        GameObject target = itemSearch.searchItem;

        string otherName = other.gameObject.name.Replace("(Clone)", ""); // Remove "(Clone)" from object names so they can be matched

        if (target.name == otherName)
        {
            if (clicked)
            {
                Debug.Log("Target found"); // Target found if user is looking at target and clicks
            }
        }
    }
}
