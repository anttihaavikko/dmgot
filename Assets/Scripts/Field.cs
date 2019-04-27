using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    public Tile tile;
    public Text interactText;

    private readonly int gridSize = 10;
    private Tile[] grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = new Tile[gridSize * gridSize];
        for(int i = 0; i < grid.Length; i++)
        {
            var val = 0;

            var pos = new Vector3(-gridSize * 0.5f + i % gridSize, -gridSize * 0.5f * 0.75f + Mathf.Floor(i / gridSize) * 0.75f, 0f);
            var t = Instantiate(tile, pos, Quaternion.identity);
            t.transform.SetParent(transform);

            t.Setup(val);
            grid[i] = t;
        }
    }

    public void Entered(Vector3 pos)
    {
        var index = GetTileIndex(pos);

        interactText.text = index > -1 ? "INTERACT" : "";

        if(index == -1)
        {
            interactText.text = "";
            return;
        }

        switch (grid[index].type) {
            case Tile.NONE:
                interactText.text = "FERTILIZE";
                return;
            case Tile.GRASS:
                interactText.text = "PLANT";
                return;
            case Tile.FRUIT:
                interactText.text = "HARVEST";
                return;
            case Tile.DEAD:
                interactText.text = "CLEAN";
                return;
            default:
                interactText.text = "";
                return;
        }
    }

    public void Interact(Vector3 pos)
    {
        var index = GetTileIndex(pos);

        if(index > -1)
        {
            grid[index].Interact();
        }
    }

    private int GetTileIndex(Vector3 pos)
    {
        var x = pos.x + gridSize * 0.5f;
        var y = pos.y / 0.75f + gridSize * 0.5f;
        if (x < 0 || x > gridSize - 0.1f || y < 0 || y > gridSize - 0.1f)
        {
            return -1;
        }

        return Mathf.RoundToInt(x + gridSize * y);
    }

    private int Neighbors(int index)
    {
        var count = 0;

        int x = index % gridSize;
        int y = (int)Mathf.Floor(index / gridSize);

        if (y < gridSize && IsPlant(x + (y + 1) * gridSize)) count++;
        if (y > 0 && IsPlant(x + (y - 1) * gridSize)) count++;
        if (x < gridSize && IsPlant(x + 1 + y * gridSize)) count++;
        if (x > 0 && IsPlant(x - 1 + y * gridSize)) count++;

        return count;
    }

    private bool IsPlant(int i)
    {
        return i >= 0 && i < gridSize * gridSize && (grid[i].type == Tile.PLANT || grid[i].type == Tile.FRUIT);
    }

    // Update is called once per frame
    void Update()
    {
        if(Application.isEditor && Input.GetKeyDown(KeyCode.Tab))
        {
            Grow();
        }
    }

    void Grow()
    {
        var next = new int[gridSize * gridSize];
        var index = 0;
        foreach (var t in grid)
        {
            t.Grow();
            next[index] = t.type;
            index++;
        }

        for (int i = 0; i < next.Length; i++)
        {
            var c = Neighbors(i);

            if (grid[i].type == Tile.PLANT || grid[i].type == Tile.FRUIT)
            {
                if (c != 2 && c != 3)
                {
                    next[i] = Tile.DEAD;
                }
            }

            if (grid[i].type == Tile.GRASS)
            {
                if (c == 3)
                {
                    next[i] = Tile.PLANT;
                }
            }
        }

        for (int i = 0; i < next.Length; i++)
        {
            grid[i].Setup(next[i]);
        }
    }
}
