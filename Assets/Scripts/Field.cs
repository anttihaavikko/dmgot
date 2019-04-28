using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour
{
    public Tile tile;
    public TextMeshPro interactText;
    public Transform helpText;
    public bool isOn = true;
    public SpeechBubble bubble;

    public const int GRIDSIZE = 10;

    private Tile[] grid;
    private bool showingHelp;

    // Start is called before the first frame update
    void Start()
    {
        if(!isOn)
        {
            return;
        }

        grid = new Tile[GRIDSIZE * GRIDSIZE];

        for (int i = 0; i < grid.Length; i++)
        {
            var val = 0;

            if(
                //i == 44 ||
                i == 45 ||
                //i == 46 ||
                i == 54 ||
                i == 55 ||
                i == 56 ||
                //i == 64 ||
                i == 65
                //i == 66
                )
            {
                val = 2;
            }

            var pos = new Vector3(-GRIDSIZE * 0.5f + i % GRIDSIZE, -GRIDSIZE * 0.5f * 0.75f + Mathf.Floor(i / GRIDSIZE) * 0.75f, 0f);
            var t = Instantiate(tile, pos, Quaternion.identity);
            t.transform.SetParent(transform);

            t.Setup(val);
            grid[i] = t;
        }

        if (Manager.Instance.hasData)
        {
            Manager.Instance.hasData = true;

            for (int i = 0; i < grid.Length; i++)
            {
                grid[i].Setup(Manager.Instance.grid[i]);
            }
        }

        if(Manager.Instance.didSleep)
        {
            Grow();
            Manager.Instance.didSleep = false;
        }
    }

    public void Entered(Vector3 pos)
    {
        var index = GetTileIndex(pos);
        UpdateHelpFor(index);
    }

    private void UpdateHelpFor(int index)
    {
        if (index == -1)
        {
            if(showingHelp)
                AnimateHelpOut();

            interactText.text = "";
            return;
        }

        AnimateHelpIn();

        if (!isOn)
        {
            return;
        }

        switch (grid[index].type)
        {
            case Tile.NONE:
                if (Manager.Instance.fertilizers > 0)
                {
                    interactText.text = "FERTILIZE";
                }
                else
                {
                    AnimateHelpOut();
                }
                return;
            case Tile.GRASS:
                interactText.text = "PLANT";
                return;
            case Tile.FRUIT:
                interactText.text = "HARVEST";
                return;
            case Tile.DEAD:
                interactText.text = "GATHER";
                return;
            default:
                AnimateHelpOut();
                return;
        }
    }

    public void AnimateHelpOut()
    {
        showingHelp = false;
        Tweener.Instance.ScaleTo(helpText, new Vector3(1.5f, 0f, 1f), 0.2f, 0f, TweenEasings.QuarticEaseIn);
    }

    private void AnimateHelpIn()
    {
        showingHelp = true;
        Tweener.Instance.ScaleTo(helpText, Vector3.one, 0.2f, 0f, TweenEasings.BounceEaseOut);
    }

    public void ShowHelp(string action)
    {
        interactText.text = action;
        Tweener.Instance.ScaleTo(helpText, Vector3.one, 0.2f, 0f, TweenEasings.BounceEaseOut);
    }

    public int GetTileType(Vector3 pos)
    {
        var index = GetTileIndex(pos);

        if (index > -1)
        {
            return grid[index].type;
        }

        return -1;
    }

    public void Interact(Vector3 pos)
    {
        var index = GetTileIndex(pos);

        if(index > -1)
        {
            var tileType = GetTileType(pos);
            if(tileType == Tile.NONE)
            {
                if(Manager.Instance.fertilizers > 0)
                {
                    Manager.Instance.fertilizers--;
                    grid[index].Interact();
                }
            }
            else
            {
                grid[index].Interact();
            }


            UpdateHelpFor(index);
        }
    }

    private int GetTileIndex(Vector3 pos)
    {
        var x = pos.x + GRIDSIZE * 0.5f;
        var y = pos.y / 0.75f + GRIDSIZE * 0.5f;
        if (x < 0 || x > GRIDSIZE - 0.1f || y < 0 || y > GRIDSIZE - 0.1f)
        {
            return -1;
        }

        return Mathf.RoundToInt(x + GRIDSIZE * y);
    }

    private int Neighbors(int index)
    {
        var count = 0;

        int x = index % GRIDSIZE;
        int y = (int)Mathf.Floor(index / GRIDSIZE);

        if (y < GRIDSIZE && IsPlant(x + (y + 1) * GRIDSIZE)) count++;
        if (y > 0 && IsPlant(x + (y - 1) * GRIDSIZE)) count++;
        if (x < GRIDSIZE && IsPlant(x + 1 + y * GRIDSIZE)) count++;
        if (x > 0 && IsPlant(x - 1 + y * GRIDSIZE)) count++;

        if (y < GRIDSIZE && x < GRIDSIZE && IsPlant(x + 1 + (y + 1) * GRIDSIZE)) count++;
        if (y < GRIDSIZE && x > 0 && IsPlant(x - 1 + (y + 1) * GRIDSIZE)) count++;
        if (y > 0 && x > 0 && IsPlant(x - 1 + (y - 1) * GRIDSIZE)) count++;
        if (y > 0 && x < GRIDSIZE && IsPlant(x + 1 + (y - 1) * GRIDSIZE)) count++;

        return count;
    }

    private bool IsPlant(int i)
    {
        return i >= 0 && i < GRIDSIZE * GRIDSIZE && (grid[i].type == Tile.PLANT || grid[i].type == Tile.FRUIT);
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
        var next = new int[GRIDSIZE * GRIDSIZE];
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

            Invoke("DoTutorialMessages", 1.25f);
        }
    }

    void DoTutorialMessages()
    {
        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i].type == Tile.DEAD && !Manager.Instance.hasSeenDead)
            {
                Manager.Instance.hasSeenDead = true;
                bubble.QueMessage("Oh bummer!\nThat plant (withered away).");
                bubble.QueMessage("Well, at least I can use it as (fertilizer)...");
            }

            if (grid[i].type == Tile.FRUIT && !Manager.Instance.hasSeenFruit)
            {
                Manager.Instance.hasSeenFruit = true;
                bubble.QueMessage("Sweet, I've got some (crop). Time to (harvest)!");
            }
        }

        bubble.CheckQueuedMessages();
    }

    public void SaveGrid()
    {
        if(isOn)
        {
            Manager.Instance.hasData = true;
            for (int i = 0; i < grid.Length; i++)
            {
                Manager.Instance.grid[i] = grid[i].type;
            }
        }
    }
}
