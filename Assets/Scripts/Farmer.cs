using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public Field field;
    public Animator anim;

    private Vector3 pos;
    private bool locked;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (locked)
            return;

        DoMove();

        if(Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    private void DoMove()
    {
        var x = 0f;
        var y = 0f;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) x = -1f;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) x = 1f;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) y = -1f;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) y = 1f;

        var dx = Mathf.Abs(x) > 0.2f ? Mathf.Sign(x) * 1f : 0;
        var dy = Mathf.Abs(y) > 0.2f ? Mathf.Sign(y) * 1f : 0;

        pos += new Vector3(dx, dy * 0.75f, 0);

        Tweener.Instance.MoveTo(transform, pos, 0.07f, 0f, TweenEasings.BounceEaseOut);

        field.Entered(pos);
    }

    private void Interact()
    {
        if(field)
        {
            anim.ResetTrigger("Act");
            anim.ResetTrigger("Cut");

            if(field.GetTileType(pos) == Tile.GRASS)
            {
                anim.SetTrigger("Cut");
                locked = true;
                Invoke("Unlock", 1f);
            }
            else
            {
                anim.SetTrigger("Act");
                field.Interact(pos);
                locked = true;
                Invoke("Unlock", 0.15f);
            }
        }
    }

    public void DoInteract()
    {
        field.Interact(pos);
    }

    private void Unlock()
    {
        locked = false;
    }
}
