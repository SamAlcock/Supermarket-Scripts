using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSearch : MonoBehaviour
{
    ItemHandling handlingData;
    public GameObject searchItem;

    public void Main() 
    {
        GameObject displayText = GameObject.Find("Display Text");
        handlingData = GetComponent<ItemHandling>();

        List<GameObject> items = new(handlingData.currItems);

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
        searchObject.transform.SetParent(mainCamera.transform);
        searchObject.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y - 0.165f, mainCamera.transform.position.z + 1);

        searchObject.SetActive(true);
        displayText.SetActive(true);
        yield return new WaitForSeconds(5f);
        searchObject.SetActive(false);
        displayText.SetActive(false);
    }
}
