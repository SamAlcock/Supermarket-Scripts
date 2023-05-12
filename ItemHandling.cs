using JetBrains.Annotations;
using System;
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
        System.Random rnd = new();

        ItemSearch itemSearch = GetComponent<ItemSearch>();
        
        for (int i = 0; i < shelves.Count; i++)
        {
            int[] nums = { rnd.Next(items.Count), rnd.Next(items.Count), rnd.Next(items.Count), rnd.Next(items.Count) }; // length of nums should be the number of shelves 

            List<GameObject> currItems = new();

            for (int x = 0; x < nums.Length; x++)
            {
                currItems.Add(items[nums[x]]); // add items to current list based on random locations of list picked
            }

            for (int j = 0; j < 4; j++)
            {
                FillShelf(currItems[j], shelves[i], j);
            }
        }

        itemSearch.Main();
    }
    public class ShelfData
    {
        public string name { get; set; }
        public float[] YOffset { get; set; }
        public float ShelfWidth { get; set; }
    }
    ShelfData DetermineObjectType(GameObject shelf)
    {
        
        GameObject singleShelf;

        ShelfData shelfData = new ShelfData();

        if (shelf.name.Contains("Shelf_kicg9f")) // Normal shelf
        {
            // Set y offsets
            float[] yOffset = { shelf.transform.position.y + 1.565f, shelf.transform.position.y + 1.12f, shelf.transform.position.y + 0.66f, shelf.transform.position.y + 0.2f };

            shelfData.YOffset = yOffset;
            shelfData.name = "Normal shelf";

            singleShelf = GameObject.Find("Shelf_kicg9f_shelf01"); // Find a singular shelf to get width

        }
        else if (shelf.name.Contains("Shelf_6h7vop")) // Grated shelf
        {
            float[] yOffset = { shelf.transform.position.y + 0.513f, shelf.transform.position.y + 0.938f, shelf.transform.position.y + 1.363f, shelf.transform.position.y + 1.79f };

            shelfData.YOffset = yOffset;
            shelfData.name = "Grated shelf";

            singleShelf = GameObject.Find("Grated Shelf Width");
        }
        else if (shelf.name.Contains("Fridge_4ttiif")) // Big fridge
        {

            float[] yOffset = { shelf.transform.position.y + 0.15f, shelf.transform.position.y + 0.43f, shelf.transform.position.y + 0.75f, shelf.transform.position.y + 1.07f, shelf.transform.position.y + 1.39f };

            shelfData.YOffset = yOffset;
            shelfData.name = "Big fridge";

            singleShelf = GameObject.Find("Big Fridge Width");
        }
        else
        {
            Debug.Log("Warning: Shelf type not recognised");
            singleShelf = GameObject.Find("Shelf_kicg9f_shelf01");
            float[] yOffset = { };
        }
        
        MeshRenderer shelfRenderer = singleShelf.GetComponent<MeshRenderer>();

        shelfData.ShelfWidth = shelfRenderer.bounds.size.x;

        return shelfData;
    }

    double CalculateItemNumberDisplay(ShelfData shelfData, MeshRenderer itemRenderer, float offset)
    {
        float shelfSize = shelfData.ShelfWidth - 0.05f; // get shelf width (subtracting to avoid overhang)
        float itemSize = itemRenderer.bounds.size.x + offset; // get item width

        double numberToLoop = System.Math.Floor(shelfSize / itemSize); // divide shelf width by item width to get number of items that fit on shelf, then round down to avoid floating items

        return numberToLoop;
    }
    float GetOffset(MeshRenderer itemRenderer)
    {
        float itemSize = itemRenderer.bounds.size.x; // get item width
        float offset;
        if (itemSize > 0.8f)
        {
            offset = itemSize / 2;
        }
        else
        {
            offset = 0.3f;
        }

        offset = 0.01f;

        return offset;
    }

    float GetRotation(GameObject shelf)
    {
        float rotation = shelf.transform.eulerAngles.y;
        return rotation;
    }
    void FillShelf(GameObject item, GameObject shelf, int iter)
    {
        ShelfData shelfData = DetermineObjectType(shelf);
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();

        float[] yOffset = shelfData.YOffset; // offsets for each shelf row 
        float xGap = renderer.bounds.size.x;
        float offset = GetOffset(renderer);
        int shelves = iter; // will need to be shelfData.YOffset.Length once other bugs have been fixed
        double loop = CalculateItemNumberDisplay(shelfData, renderer, offset);
        float rotation = GetRotation(shelf);
        float originalX;
        bool rotated = false;

        Vector3 gap = new();
        Vector3 itemRotation = new();
        Vector3 offsets = new();


        if(shelfData.name == "Normal shelf")
        {
            if (rotation == 0 || rotation == 360) // used to put items across shelf on z axis
            {
                gap = new(0, 0, xGap);
                itemRotation = new(0, 90, 0);
                offsets = new(0, 0, offset);
                rotated = true;
            }
            else
            {
                gap = new(xGap, 0, 0);
                itemRotation = new(0, 0, 0);
                offsets = new(offset, 0, 0);
            }

            if (rotation == 90 || rotation == 0)
            {
                itemRotation = new(0, shelf.transform.eulerAngles.y + 90, 0);
            }
            originalX = shelf.transform.position.x;
        }
        else
        {
            if (rotation == 90 || rotation == 270) // used to put items across shelf on z axis
            {
                gap = new(0, 0, xGap);
                itemRotation = new(0, 90, 0);
                offsets = new(0, 0, offset);
                rotated = true;
            }
            else
            {
                gap = new(xGap, 0, 0);
                itemRotation = new(0, 0, 0);
                offsets = new(offset, 0, 0);
            }
            originalX = shelf.transform.position.x;

        }

        Vector3 position = new();
        if (rotated)
        {
            position = new Vector3(originalX, shelf.transform.position.y + yOffset[shelves], shelf.transform.position.z + (shelfData.ShelfWidth / 2) - (xGap / 2)); // get starting coordinates of shelf row for rotated shelves

        }
        else
        {
            position = new Vector3(originalX + (shelfData.ShelfWidth / 2) - (xGap / 2), shelf.transform.position.y + yOffset[shelves], shelf.transform.position.z); // get starting coordinates of shelf row for non-rotated shelves

        }
        for (int j = 0; j < loop; j++)
        {
            Instantiate(item, position - (offsets * j), Quaternion.Euler(itemRotation)); // create instance of current item
            position -= gap; // decrement by gap
        }   
    }
}
