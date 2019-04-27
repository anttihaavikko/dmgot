using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject grass, fertilizer, plant, fruit;
    public Color plantColor, deadColor;
    public int type = 0;
    public SpriteRenderer plantSprite;

    public const int NONE = 0;
    public const int FERTILIZED = 1;
    public const int GRASS = 2;
    public const int PLANTED = 3;
    public const int PLANT = 4;
    public const int FRUIT = 5;
    public const int DEAD = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(int t)
    {
        type = t;
        UpdateView();
    }

    public void Interact()
    {
        if (type == NONE || type == GRASS)
            type++;

        if (type == DEAD)
            type = GRASS;

        if (type == FRUIT)
            type = PLANT;

        UpdateView();
    }

    public void Grow()
    {
        if (type == FERTILIZED || type == PLANTED || type == PLANT || type == FRUIT)
            type++;

        UpdateView();
    }

    private void UpdateView()
    {
        if (type >= PLANT) plantSprite.color = plantColor;
        if (type == DEAD) plantSprite.color = deadColor;

        fertilizer.SetActive(type == FERTILIZED);
        grass.SetActive(type >= GRASS);
        plant.SetActive(type >= PLANT);
        fruit.SetActive(type == FRUIT);
    }
}
