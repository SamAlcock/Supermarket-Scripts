using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    ItemSearch itemSearch;
    
    private void OnTriggerEnter(Collider other)
    {
        GameObject Supermarket = GameObject.Find("Supermarket");
        itemSearch = Supermarket.GetComponent<ItemSearch>();
        GameObject target = itemSearch.searchItem;

        string otherName = other.gameObject.name.Replace("(Clone)", "");

        if (target.name == otherName)
        {
            Debug.Log("Collided!");
        }


    }
}
