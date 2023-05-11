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
        clicked = playerInteract.clicked;
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject Supermarket = GameObject.Find("Supermarket");
        itemSearch = Supermarket.GetComponent<ItemSearch>();
        GameObject target = itemSearch.searchItem;

        string otherName = other.gameObject.name.Replace("(Clone)", "");

        if (target.name == otherName)
        {
            Debug.Log("Collided!");
            if (clicked)
            {
                Debug.Log("Target found");
            }
        }
    }
}
