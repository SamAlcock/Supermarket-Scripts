using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class ItemHandling : MonoBehaviour
{
    public List<GameObject> items = new(); // items that will fill shelves
    public List<GameObject> shelves = new(); // every shelf in supermarket
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < shelves.Count; i++)
        {
            FillShelf(items[i], shelves[i]);
        }
    }
    public class ShelfData
    {
        public float[] yOffset { get; set;}
        public float shelfWidth { get; set;}
    }
    void DetermineObjectType(GameObject item)
    {
        if (item.name.Contains("Shelf_kicg9f"))
        {

        }
    }
    double CalculateItemNumberDisplay(MeshRenderer shelfRenderer, MeshRenderer itemRenderer)
    {
        float shelfSize = shelfRenderer.bounds.size.x - 0.05f; // get shelf width (subtracting to avoid overhang)
        float itemSize = itemRenderer.bounds.size.x; // get item width

        double numberToLoop = System.Math.Floor(shelfSize / itemSize); // divide shelf width by item width to get number of items that fit on shelf, then round down to avoid floating items

        return numberToLoop;
    }
    float GetOffset(MeshRenderer itemRenderer)
    {
        float itemSize = itemRenderer.bounds.size.x; // get item width
        float offset = 0.9f;
        if (itemSize > 0.9f)
        {
            offset = itemSize / 2;
        }

        return offset;
    }

    float GetRotation(GameObject shelf)
    {
        float rotation = shelf.transform.eulerAngles.y;
        return rotation;
    }
    void FillShelf(GameObject item, GameObject shelf)
    {
        GameObject singleShelf = GameObject.Find("Shelf_kicg9f_shelf01");
        MeshRenderer shelfRenderer = singleShelf.GetComponent<MeshRenderer>();
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();

        float originalY = shelf.transform.position.y; // to help with readability
        float[] yOffset = { originalY + 1.565f, originalY + 1.12f, originalY + 0.66f, originalY + 0.2f }; // offsets for each shelf row 
        float xGap = renderer.bounds.size.x;
        float offset = GetOffset(renderer);
        int shelves = 4;
        double loop = CalculateItemNumberDisplay(shelfRenderer, renderer);
        float rotation = GetRotation(shelf);

        Vector3 gap = new();
        Vector3 itemRotation = new();

        if (rotation == 0 || rotation == 360) // used to put items across shelf on z axis
        {
            gap = new(0, 0, xGap);
            itemRotation = new(0, 90, 0);
            Vector3 offsets = new(0, 0, offset);
            shelf.transform.position += offsets;
        }
        else
        {
            gap = new(xGap, 0, 0);
            itemRotation = new(0, 0, 0);
            Vector3 offsets = new(offset, 0, 0);
            shelf.transform.position += offsets;
        }

        if(rotation == 90 || rotation == 0)
        {
            itemRotation = new(0, shelf.transform.eulerAngles.y + 90, 0);
        }

        for (int i = 0; i < shelves; i++)
        {
            Vector3 position = new Vector3(shelf.transform.position.x, shelf.transform.position.y + yOffset[i], shelf.transform.position.z); // get starting coordiantes of shelf row

            for (int j = 0; j < loop; j++)
            {
                Instantiate(item, position, Quaternion.Euler(itemRotation)); // create instance of current item
                item.transform.Rotate(0, rotation, 0, Space.Self); // may not need this
                position -= gap; // decrement by gap

            }
        }
    }
}
