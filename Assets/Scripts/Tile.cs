﻿using System.Collections;
using System.Collections.Generic;
using Anima2D;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject grass, fertilizer, plant, fruit, planted;
    public Color[] plantColors, deadColors;
    public int type = 0;
    public SpriteRenderer plantSprite, fruitSprite;

    public Transform[] fruitPos;
    public Sprite[] fruitSprites;

    public const int NONE = 0;
    public const int FERTILIZED = 1;
    public const int GRASS = 2;
    public const int PLANTED = 3;
    public const int PLANT = 4;
    public const int FRUIT = 5;
    public const int DEAD = 6;

    private Vector3 fertScale, plantedScale;

    // Start is called before the first frame update
    void Start()
    {
        fertScale = fertilizer.transform.localScale;
        plantedScale = planted.transform.localScale;
        //fertilizer.transform.localScale = Vector3.zero;
        //planted.transform.localScale = Vector3.zero;
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

        Randomize();

        UpdateView();
    }

    private void UpdateView()
    {
        if (type >= PLANT) plantSprite.color = plantColors[Random.Range(0, plantColors.Length)];
        if (type == DEAD) plantSprite.color = deadColors[Random.Range(0, deadColors.Length)];

        fertilizer.SetActive(type == FERTILIZED);
        grass.SetActive(type >= GRASS);
        plant.SetActive(type >= PLANT);
        fruit.SetActive(type == FRUIT);
        planted.SetActive(type == PLANTED);

        //Tweener.Instance.ScaleTo(fertilizer.transform, fertScale, 0.3f, 0f, TweenEasings.BounceEaseOut);
        //Tweener.Instance.ScaleTo(planted.transform, plantedScale, 0.3f, 0f, TweenEasings.BounceEaseOut);
    }

    private void Randomize()
    {
        if(type >= PLANT)
        {
            plant.transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
        }

        if (type == FRUIT)
        {
            fruitSprite.sprite = fruitSprites[Random.Range(0, fruitSprites.Length)];
            fruit.transform.position = fruitPos[Random.Range(0, fruitPos.Length)].position;
        }

        float mod = Random.value < 0.5f ? -1f : 1f;
        plant.transform.localScale = new Vector3(plant.transform.localScale.x * mod, plant.transform.localScale.y, plant.transform.localScale.z);
    }
}
