using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting;

public class ItemHandling : MonoBehaviour
{
    public List<GameObject> items = new(); // items that can fill shelves

    // Items for each biome
    [SerializeField] List<GameObject> breadItems = new();
    [SerializeField] List<GameObject> alcoholItems = new();
    [SerializeField] List<GameObject> fridgeItems = new();

    List<GameObject> shelves; // every shelf in supermarket

    public List<GameObject> itemsInSupermarket = new();
    public List<GameObject> itemTypesInSupermarket = new();

    public void Main()
    {
        ProceduralGeneration proceduralGeneration = GetComponent<ProceduralGeneration>();
        shelves = new(proceduralGeneration.shelves);
        List<float> chunkRotation = new(proceduralGeneration.chunkRotation);
        List<string> biomes = new(proceduralGeneration.biomes);

        itemsInSupermarket.Clear();
        itemTypesInSupermarket.Clear();



        System.Random rnd = new();

        ItemSearch itemSearch = GetComponent<ItemSearch>();

        for (int i = 0; i < shelves.Count; i++)
        {
            items = GetCurrentBiome(i, biomes);

            int[] nums = { rnd.Next(items.Count), rnd.Next(items.Count), rnd.Next(items.Count), rnd.Next(items.Count) }; // length of nums should be the number of shelves 

            List<GameObject> currItems = new(); // items that will go on shelf for specific iteration
            for (int x = 0; x < nums.Length; x++)
            {
                currItems.Add(items[nums[x]]); // add items to current list based on random locations of list picked
            }

            for (int j = 0; j < 4; j++) // needs to be something like y_offset.Length
            {
                FillShelf(currItems[j], shelves[i], j, chunkRotation[i]);
            }
        }

        //itemTypesInSupermarket = itemsInSupermarket.Distinct().ToList(); // Remove duplicates to avoid biased target picking

        itemTypesInSupermarket = new(itemsInSupermarket);
        itemTypesInSupermarket = RemoveDuplicates(itemTypesInSupermarket);

        itemSearch.Main();
    }

    List<GameObject> GetCurrentBiome(int i, List<string> biomes)
    {
        if (biomes[i] == "Bread")
        {
            return breadItems;
        }
        else if (biomes[i] == "Alcohol")
        {
            return alcoholItems;
        }
        else
        {
            return fridgeItems;
        }
    }

    List<GameObject> RemoveDuplicates(List<GameObject> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            for (int j = 0; j < items.Count; j++)
            {
                if (items[i].ToString() == items[j].ToString())
                {
                    items.Remove(items[j]);
                    if(j > 0)
                    {
                        j--;
                    }
                    
                }
            }
        }
        return items;
    }
    public class ShelfData // attributes for shelves
    {
        public string name { get; set; }
        public float[] YOffset { get; set; }
        public float ShelfWidth { get; set; }
    }
    ShelfData DetermineObjectType(GameObject shelf, float chunkRotation)
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
        else if (shelf.name.Contains("Fridge_4ttiif")) // Big fridge - not fully implemented
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

        // Accounting for chunk rotation when getting shelf width - if chunk has been rotated the width would become the original depth


        if (shelfData.name == "Normal shelf" && Mathf.Approximately(chunkRotation, 0) || shelfData.name == "Normal shelf" &&  Mathf.Approximately(chunkRotation, 180) || shelfData.name == "Grated shelf" && Mathf.Approximately(chunkRotation, 90) || shelfData.name == "Grated shelf" && Mathf.Approximately(chunkRotation, 270))
        {
            shelfData.ShelfWidth = shelfRenderer.bounds.size.z;
        }
        else if (shelfData.name == "Grated shelf" && Mathf.Approximately(chunkRotation, 0) || shelfData.name == "Grated shelf" && Mathf.Approximately(chunkRotation, 180))
        {
            shelfData.ShelfWidth = shelfRenderer.bounds.size.x;
        }
        else
        {
            shelfData.ShelfWidth = shelfRenderer.bounds.size.z;
        }
        //Debug.Log(shelfData.ShelfWidth + " chunkRotation = " + chunkRotation + " shelfData.name = " + shelfData.name);
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
    void FillShelf(GameObject item, GameObject shelf, int iter, float chunkRotation)
    {
        ShelfData shelfData = DetermineObjectType(shelf, chunkRotation);
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();

        float[] yOffset = shelfData.YOffset; // offsets for each shelf row 
        float xGap = renderer.bounds.size.x;
        float offset = GetOffset(renderer);
        int shelves = iter; // will need to be shelfData.YOffset.Length once other bugs have been fixed
        double loop = CalculateItemNumberDisplay(shelfData, renderer, offset);
        float rotation = chunkRotation;

        float originalX;
        bool rotated = false;

        Vector3 gap = new();
        Vector3 itemRotation = new();
        Vector3 offsets = new();


        if(shelfData.name == "Normal shelf")
        {
            if (Mathf.Approximately(rotation, 0)) // used to put items across shelf on z axis
            {
                gap = new(0, 0, xGap);
                itemRotation = new(0, 90, 0);
                offsets = new(0, 0, offset);
                rotated = true;
            }
            else if (Mathf.Approximately(rotation, 180))
            {
                gap = new(0, 0, xGap);
                itemRotation = new(0, 270, 0);
                offsets = new(0, 0, offset);
                rotated = true;
            }
            else
            {
                gap = new(xGap, 0, 0);
                itemRotation = new(0, 0, 0);
                offsets = new(offset, 0, 0);
            }

            if (Mathf.Approximately(rotation, 90))
            {
                itemRotation = new(0, shelf.transform.eulerAngles.y + 90, 0);
            }
            originalX = shelf.transform.position.x;
        }
        else
        {
            if (Mathf.Approximately(rotation, 90) || Mathf.Approximately(rotation, 270)) // used to put items across shelf on z axis
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
        Vector3 position;
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
            itemsInSupermarket.Add(Instantiate(item, position - (offsets * j), Quaternion.Euler(itemRotation)));
            position -= gap; // decrement by gap
        }
        
    }
}
