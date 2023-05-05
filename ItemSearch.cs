using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSearch : MonoBehaviour
{
    ItemHandling handlingData;
    public void Main() 
    {
        GameObject displayText = GameObject.Find("Display Text");
        GameObject searchObject = GameObject.Find("Search Object");
        handlingData = GetComponent<ItemHandling>();

        List<GameObject> items = new(handlingData.items);

        GameObject searchItem = GetItemSearch(items);

        searchObject.SetActive(false);
        displayText.SetActive(false);

        StartCoroutine(DisplaySearch(searchItem, searchObject, displayText));
    }

    GameObject GetItemSearch(List<GameObject> items)
    {
        System.Random rnd = new System.Random();
        int idx = rnd.Next(0, items.Count);
        return items[idx];
    }

    IEnumerator DisplaySearch(GameObject searchItem, GameObject searchObject, GameObject displayText)
    {
        Mesh mesh = searchItem.GetComponent<MeshFilter>().sharedMesh;
        searchObject.GetComponent<MeshFilter>().sharedMesh = mesh;

        Material material = searchItem.GetComponent<Material>();
        searchObject.GetComponent<Renderer>().material = material;

        searchObject.SetActive(true);
        displayText.SetActive(true);
        yield return new WaitForSeconds(5f);
        searchObject.SetActive(false);
        displayText.SetActive(false);
    }
}
