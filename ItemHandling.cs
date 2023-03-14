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
    double CalculateItemNumberDisplay(MeshRenderer shelf_renderer, MeshRenderer item_renderer)
    {
        float shelf_size = shelf_renderer.bounds.size.x - 0.05f; // get shelf width (subtracting to avoid overhang)
        float item_size = item_renderer.bounds.size.x; // get item width

        double number_to_loop = System.Math.Floor(shelf_size / item_size); // divide shelf width by item width to get number of items that fit on shelf, then round down to avoid floating items

        return number_to_loop;
    }
    float GetOffset(MeshRenderer item_renderer)
    {
        float item_size = item_renderer.bounds.size.x; // get item width
        float offset = 0.9f;
        if (item_size > 0.9f)
        {
            offset = item_size / 2;
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
        GameObject single_shelf = GameObject.Find("Shelf_kicg9f_shelf01");
        MeshRenderer shelf_renderer = single_shelf.GetComponent<MeshRenderer>();
        MeshRenderer renderer = item.GetComponent<MeshRenderer>();

        float original_y = shelf.transform.position.y; // to help with readability
        float[] y_offsets = { original_y + 1.565f, original_y + 1.12f, original_y + 0.66f, original_y + 0.2f }; // offsets for each shelf row 
        float x_gap = renderer.bounds.size.x;
        float offset = GetOffset(renderer);
        int shelves = 4;
        double loop = CalculateItemNumberDisplay(shelf_renderer, renderer);
        float rotation = GetRotation(shelf);

        Vector3 gap = new();
        Vector3 item_rotation = new();

        if (rotation == 0 || rotation == 360) // used to put items across shelf on z axis
        {
            gap = new(0, 0, x_gap);
            item_rotation = new(0, 90, 0);
            Vector3 offsets = new(0, 0, offset);
            shelf.transform.position += offsets;
        }
        else
        {
            gap = new(x_gap, 0, 0);
            item_rotation = new(0, 0, 0);
            Vector3 offsets = new(offset, 0, 0);
            shelf.transform.position += offsets;
        }

        if(rotation == 90 || rotation == 0)
        {
            item_rotation = new(0, shelf.transform.eulerAngles.y + 90, 0);
        }

        for (int i = 0; i < shelves; i++)
        {
            Vector3 position = new Vector3(shelf.transform.position.x, shelf.transform.position.y + y_offsets[i], shelf.transform.position.z); // get starting coordiantes of shelf row

            for (int j = 0; j < loop; j++)
            {
                Instantiate(item, position, Quaternion.Euler(item_rotation)); // create instance of current item
                item.transform.Rotate(0, rotation, 0, Space.Self); // may not need this
                position -= gap; // decrement by gap

            }
        }
    }
}
