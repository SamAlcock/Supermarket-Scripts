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

    // Update is called once per frame
    void Update()
    {
        
    }
    double CalculateItemNumberDisplay(MeshRenderer shelf_renderer, MeshRenderer item_renderer)
    {
        float shelf_size = shelf_renderer.bounds.size.x - 0.15f; // get shelf width (subtracting to avoid overhang)
        float item_size = item_renderer.bounds.size.x; // get item width

        double number_to_loop = System.Math.Floor(shelf_size / item_size); // divide shelf width by item width to get number of items that fit on shelf, then round down to avoid floating items

        return number_to_loop;
    }
    float GetXOffset(MeshRenderer item_renderer)
    {
        float item_size = item_renderer.bounds.size.x; // get item width
        float x_offset = 0.9f;
        if (item_size > 0.9f)
        {
            x_offset = item_size / 2;
        }

        return x_offset;
    }
    void FillShelf(GameObject item, GameObject shelf)
    {
        GameObject single_shelf = GameObject.Find("Shelf_kicg9f_shelf01");
        MeshRenderer shelf_renderer = single_shelf.GetComponent<MeshRenderer>();
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();

        float original_y = shelf.transform.position.y; // to help with readability
        float[] y_offsets = { original_y + 1.565f, original_y + 1.12f, original_y + 0.66f, original_y + 0.2f }; // offsets for each shelf row
        
        float x_gap = renderer.bounds.size.x;

        float x_offset = GetXOffset(renderer);
        float z_offset = 0; // may be needed for shelves rotated differently

        int shelves = 4;

        double loop = CalculateItemNumberDisplay(shelf_renderer, renderer);

        for (int i = 0; i < shelves; i++)
        {
            Vector3 position = new Vector3(shelf.transform.position.x + x_offset, shelf.transform.position.y + y_offsets[i], shelf.transform.position.z); // get starting coordiantes of shelf row

            for (int j = 0; j < loop; j++)
            {
                Instantiate(item, position, Quaternion.identity); // create instance of current item
                position.x -= x_gap; // decrement by gap

            }
        }
    }
}
