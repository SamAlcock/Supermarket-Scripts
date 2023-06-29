using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using JetBrains.Annotations;

public class ProceduralGeneration : MonoBehaviour
{

    [SerializeField] bool visualiseGrid = true;

    GameObject positiveMarker;
    GameObject negativeMarker;

    public List<GameObject> shelves;
    public List<float> chunkRotation;
    public List<string> biomes;

    // Start is called before the first frame update
    void Start()
    {
        // Current size of area is 6x6m - may be too big for room in Link

        ItemHandling itemHandling = GetComponent<ItemHandling>();

        positiveMarker = GameObject.Find("Marker ++");
        negativeMarker = GameObject.Find("Marker --");

        float threshold = 0.3f;

        Vector3[,] grid = GenerateGrid(positiveMarker, negativeMarker);

        float[,] noiseMap = GenerateNoiseMap(grid.GetLength(0), grid.GetLength(1), 5);

        if(visualiseGrid)
        {
            VisualiseGrid(grid, noiseMap, threshold);
        }

        List<GameObject> shelvesInArea = PopulateArea(grid, noiseMap, positiveMarker, negativeMarker, threshold); // Can be used in supermarket code for items

        shelves = ShelvesToList();
        chunkRotation = GetChunkYRotation();
        biomes = DetermineSupermarketBiome(grid, noiseMap, shelves);

        Debug.Log("Amount of shelves in area: " + shelves.Count);

        itemHandling.Main();
    }
    Vector3[,] GenerateGrid(GameObject positiveMarker, GameObject negativeMarker)
    {

        // Calculate size of area using corner markers
        float widthX = positiveMarker.transform.position.x - negativeMarker.transform.position.x;
        float widthZ = positiveMarker.transform.position.z - negativeMarker.transform.position.z;

        // Gap between grid points - decrease gap to increase point density
        float xGap = 0.3f;
        float zGap = 0.3f;

        // Calculate how many points can fit between both corner markers using the defined gaps
        int elementsX = Convert.ToInt32(widthX / xGap);
        int elementsZ = Convert.ToInt32(widthZ / zGap);

        Vector3[,] grid = new Vector3[elementsX - 1, elementsZ - 1]; // create grid of dimensions elementsX - 1, elementsZ - 1 

        // Define initial coordinates, with an initial offset of xGap and zGap to avoid placing points in wall
        Vector3 initialCoords = new(negativeMarker.transform.position.x + xGap, 0, negativeMarker.transform.position.z + zGap);

        for (int x = 0; x < elementsX - 1; x++)
        {
            for (int z = 0; z < elementsZ - 1; z++)
            {
                grid[x, z] = new Vector3(initialCoords.x + (xGap * x), 0.01f, initialCoords.z + (zGap * z));
            }
        }
        return grid;
    }
    
    float[,] GenerateNoiseMap(int mapWidth, int mapDepth, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapDepth]; // create an empty noise map with mapWidth and mapDepth coordinates

        System.Random rnd = new System.Random();
        float seed = rnd.Next(0, 100000) + 0.01f; // random seed to sample new Perlin noise each time

        // float seed = 1258 + 0.01f; // random seed to sample new Perlin noise each time

        Debug.Log("Seed is: " + seed);

        for (int xIndex = 0; xIndex < mapWidth; xIndex++)
        {
            for (int zIndex = 0; zIndex < mapDepth; zIndex++)
            {
                // Calculate samples based on coordinates and scale
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;

                float noise = Mathf.PerlinNoise(sampleX + seed, sampleZ + seed); // generate noise using PerlinNoise

                noiseMap[xIndex, zIndex] = noise;
            }
        }
        
        return noiseMap;
    }
    List<Vector3> GetMiddlePoints(Vector3[,] grid)
    {
        List<Vector3> middlePoints = new();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                if (x == grid.GetLength(0) / 4 && z == grid.GetLength(0) / 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int[] operators = GetOperators(i);
                        middlePoints.Add(grid[x + operators[0], z + operators[1]]); // Add coordinates for each quadrant
                    }
                }
            }

        }

        return middlePoints;

    }

    List<float> GetMiddleNoise(Vector3[,] grid, float[,] noiseMap)
    {
        List<float> middleNoise = new();

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                if (x == grid.GetLength(0) / 4 && z == grid.GetLength(0) / 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int[] operators = GetOperators(i);
                        middleNoise.Add(noiseMap[x + operators[0], z + operators[1]]);
                    }
                }
            }

        }

        return middleNoise;
    }

    void VisualiseGrid(Vector3[,] grid, float[,] noiseMap, float threshold)
    {

        GameObject coordPoint = GameObject.Find("Coordinate point");
        List<GameObject> currInstance = new();
        List<Color> colors = new();
        Debug.Log(grid.GetLength(0) + ", " + grid.GetLength(1));
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int z = 0; z < grid.GetLength(1); z++)
            {
                if (x == grid.GetLength(0) / 4 && z == grid.GetLength(0) / 4)
                {

                    for (int i = 0; i < 4; i++)
                    {
                        int[] operators = GetOperators(i);

                        colors.Add(Color.Lerp(Color.red, Color.blue, noiseMap[x + operators[0], z + operators[1]])); // assign colour based on Perlin noise value 
                        currInstance.Add(Instantiate(coordPoint, grid[x + operators[0], z + operators[1]], Quaternion.identity)); // show coordinate points on floor (just to visualise the grid)
                        currInstance.Last().GetComponent<MeshRenderer>().material.SetColor("_Color", colors.Last());
                    }
                }

            }

        }
    }

    int[] GetOperators(int key) // used to find middle points in each quadrant
    {
        int[] operators = new int[2];
        int operator1 = 0;
        int operator2 = 0;

        if (key == 0)
        {
            operator1 = 0;
            operator2 = 0;
        }
        else if (key == 1)
        {
            operator1 = 8;
            operator2 = 0;
        }
        else if (key == 2)
        {
            operator1 = 0;
            operator2 = 8;
        }
        else if (key == 3)
        {
            operator1 = 8;
            operator2 = 8;
        }

        operators[0] = operator1;
        operators[1] = operator2;

        return operators;
    }

    List<GameObject> PopulateArea(Vector3[,] grid, float[,] noiseMap, GameObject positiveMarker, GameObject negativeMarker, float threshold)
    {
        GameObject shelf = GameObject.Find("Shelf_kicg9f");

        List<GameObject> shelves = new();

        /* 
         * To add a new chunk:
         * - Create a new chunk in prefabs - needs to be able to fit in 1.25x1.25 area
         * - Add it to the environment - may be able to just reference it straight from prefabs instead
         * - Find gameobject
         * - Add else if statement to block below - will need to adjust noise values in all statements below
         * - Deactivate the chunk at the bottom of the function
         * 
         * When making chunks, make sure the only direct children of the chunk are shelves - it messes up biome generations otherwise!
         */

        GameObject chunk1 = GameObject.Find("Chunk 1");
        GameObject chunk2 = GameObject.Find("Chunk 2");
        GameObject chunk3 = GameObject.Find("Chunk 3");
        GameObject chunk4 = GameObject.Find("Chunk 4");
        GameObject chunk5 = GameObject.Find("Chunk 5");

        List<GameObject> currChunks = new();

        List<Vector3> middlePoints = GetMiddlePoints(grid);
        List<float> middleNoise = GetMiddleNoise(grid, noiseMap);

        for (int i = 0; i < middlePoints.Count; i++) // generate chunk from middle point to avoid overlap
        {

            // int rotation = GetRandomRotation();

            int rotation = 0;

            Vector3 position = middlePoints[i];

            if (middleNoise[i] < 0.15)
            {
                currChunks.Add(Instantiate(chunk1, position, Quaternion.Euler(0, rotation, 0)));
            }
            else if (middleNoise[i] >= 0.15 && middleNoise[i] < 0.3)
            {
                currChunks.Add(Instantiate(chunk2, position, Quaternion.Euler(0, rotation, 0)));
            }
            else if (middleNoise[i] >= 0.3 && middleNoise[i] < 0.55)
            {
                currChunks.Add(Instantiate(chunk3, position, Quaternion.Euler(0, rotation, 0)));
            }
            else if (middleNoise[i] >= 0.55 && middleNoise[i] < 0.65)
            {
                currChunks.Add(Instantiate(chunk4, position, Quaternion.Euler(0, rotation, 0)));
            }
            else if (middleNoise[i] >= 0.65 && middleNoise[i] < 1)
            {
                currChunks.Add(Instantiate(chunk5, position, Quaternion.Euler(0, rotation, 0)));
            }
        }

        chunk1.SetActive(false);
        chunk2.SetActive(false);
        chunk3.SetActive(false);
        chunk4.SetActive(false);
        chunk5.SetActive(false);



        return shelves;
    }

    int GetRandomRotation()
    {
        System.Random rnd = new System.Random();

        int rotationKey = rnd.Next(0, 4);

        if (rotationKey == 0)
        {
            return 0;
        }
        else if (rotationKey == 1)
        {
            return 90;
        }
        else if (rotationKey == 2)
        {
            return 180;
        }
        else if (rotationKey == 3)
        {
            return 270;
        }
        else
        {
            return 0;
        }
    }

    List<GameObject> ShelvesToList()
    {
        List<GameObject> shelves = new();

        foreach (GameObject shelf in GameObject.FindGameObjectsWithTag("Shelf"))
        {
            shelves.Add(shelf);
        }

        return shelves;
    }

    List<float> GetChunkYRotation()
    {
        List<float> rotations = new();

        foreach (GameObject shelf in GameObject.FindGameObjectsWithTag("Shelf"))
        {
            rotations.Add(shelf.GetComponentInParent<Transform>().transform.eulerAngles.y);
        }

        return rotations;
    }

    List<string> DetermineSupermarketBiome(Vector3[,] grid, float[,] noiseMap, List<GameObject> shelves) // BUG - same chunk types cause adding extra data to biomes list e.g. 3 chunk 4s in a 12 shelf supermarket will make biomes.Count = 15
    {
        List<string> biomes = new();

        /* 
         * Adding new supermarket biomes
         * - Add new else if statement in the same format as one below - adjust biomeNoise accordingly (noise value is always between 0 and 1) e.g. bread currently has 0 -> 0.4 assigned to it, next one could be  >=0.4, <0.6
         * - In ItemHandling.cs, create a new list where breadItems and alcoholItems are with [SerializeField]
         * - In ItemHandling.cs, there's a function called GetCurrentBiome - add an else if statement in the same layout as the bread one, just with the new biome name
         * - In the inspector under supermarket, drag the objects for the biome into the list
         */
        List<GameObject> chunks = new();

        
        
        foreach(GameObject chunk in GameObject.FindGameObjectsWithTag("Chunk")) // loop here used to be able to get biome for all chunks if chunk number changes in future
        {
            chunks.Add(chunk);
        }

        float[] biomeNoise = new float[chunks.Count];

        for (int i = 0; i < chunks.Count; i++) // for every chunk
        {
            biomeNoise[i] = noiseMap[i, 0];
            Debug.Log("Child count: " + chunks[i].transform.childCount);
            if (biomeNoise[i] < 0.4)
            {
                for (int j = 0; j < chunks[i].transform.childCount; j++) // add biome name to list for each shelf in chunk
                {
                    biomes.Add("Bread");
                }
            }
            else
            {
                for (int j = 0; j < chunks[i].transform.childCount; j++)
                {
                    biomes.Add("Alcohol");
                }
            }
        }

        Debug.Log(string.Join(", ", biomes) + " Length: " + biomes.Count);

        return biomes;
    }

}
