using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    ItemSearch itemSearch;
    ItemHandling itemHandling;
    PlayerInteract playerInteract;
    bool clicked;
    bool targetFound = false;
    private void Start()
    {
        GameObject Player = GameObject.Find("Player");
        playerInteract = Player.GetComponent<PlayerInteract>();
    }
    void Update()
    {
        clicked = playerInteract.clicked; // Check if user has clicked
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        GameObject Supermarket = GameObject.Find("Supermarket");
        itemSearch = Supermarket.GetComponent<ItemSearch>();
        GameObject target = itemSearch.searchItem;

        string targetName = target.name.Replace("(Clone)(Clone)", "(Clone)"); // Remove "(Clone)" from target names so they can be matched - target name becomes (Clone)(Clone) as it's instantiating a clone

        if (targetName == other.gameObject.name)
        {
            if (clicked && !targetFound)
            {
                Debug.Log("Target found"); // Target found if user is looking at target and clicks
                
                itemHandling = Supermarket.GetComponent<ItemHandling>();
                List<GameObject> itemsInSupermarket = itemHandling.itemsInSupermarket;

                for (int i = 0; i < itemsInSupermarket.Count; i++)
                {
                    Destroy(itemsInSupermarket[i]);
                }
                itemHandling.Main();
            }
        }
        
    }*/
}
