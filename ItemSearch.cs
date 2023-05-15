using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSearch : MonoBehaviour
{
    ItemHandling handlingData;
    public GameObject searchItem;

    public void Main() 
    {
        GameObject displayText = GameObject.Find("Display Text"); // only works for first trial

        if (displayText == null) // displayText is null after first trial - it's deactivated after trial and cannot be found using .Find 
        {
            GameObject[] texts = GameObject.FindObjectsOfType<GameObject>(true);

            foreach (var text in texts)
            {
                if (text.name == "Display Text")
                {
                    displayText = text;
                }
            }
        }

        
        handlingData = GetComponent<ItemHandling>();

        List<GameObject> items = new(handlingData.itemTypesInSupermarket);

        searchItem = GetItemSearch(items);

        displayText.SetActive(false);

        StartCoroutine(DisplaySearch(searchItem, displayText));
    }

    GameObject GetItemSearch(List<GameObject> items)
    {
        System.Random rnd = new System.Random();
        int idx = rnd.Next(0, items.Count);
        return items[idx];
    }

    IEnumerator DisplaySearch(GameObject searchItem, GameObject displayText)
    {
        GameObject searchObject = Instantiate(searchItem);
        GameObject mainCamera = GameObject.Find("Main Camera");
        searchObject.transform.SetParent(mainCamera.transform, false);
        searchObject.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - 0.165f, mainCamera.transform.position.z + 1);

        searchObject.SetActive(true);
        displayText.SetActive(true);
        yield return new WaitForSeconds(5f);
        searchObject.SetActive(false);
        displayText.SetActive(false);
        //Destroy(searchObject);
    }
}
