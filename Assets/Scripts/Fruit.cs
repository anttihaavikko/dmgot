using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        Show();
    }

    public void Show()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        gameObject.SetActive(Manager.Instance.cash > 0);
    }
}
