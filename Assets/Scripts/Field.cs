using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public GameObject tile;

    public int gridSize = 10;

    public int[] grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new int[gridSize * gridSize];
        for(int i = 0; i < grid.Length; i++)
        {
            grid[i] = Random.Range(0, 2);

            var pos = new Vector3(-gridSize * 0.5f + i % gridSize, -gridSize * 0.5f * 0.75f + Mathf.Floor(i / gridSize) * 0.75f, 0f);
            Instantiate(tile, pos, Quaternion.identity);

            if (grid[i] == 1)
            {

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
