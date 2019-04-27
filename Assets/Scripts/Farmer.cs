using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Farmer : MonoBehaviour
{
    public Field field;
    public Animator anim;
    public LayerMask blockMask, spotMask;
    public bool isOutside = true;
    public Dimmer dimmer;
    public Spot doorSpot;

    private Vector3 pos;
    private bool locked;
    private bool helpShown;

    private Spot spot;
    private string sceneToSwitch;


    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;

        Invoke("ShowHelpAfterDim", Dimmer.speed);
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

    private void ShowHelpAfterDim()
    {
        field.ShowHelp(doorSpot.action);
        spot = doorSpot;
        helpShown = true;
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

        var target = pos + new Vector3(dx, dy * 0.75f, 0);

        var ray = pos - target;
        var walking = Physics2D.Raycast(target, ray, ray.magnitude, blockMask);

        if(walking)
        {
            return;
        }

        pos = target;

        if (Mathf.Abs(dx) > 0f || Mathf.Abs(dy) > 0f) {
            Tweener.Instance.MoveTo(transform, pos, 0.07f, 0f, TweenEasings.BounceEaseOut);

            if(helpShown)
            {
                field.AnimateHelpOut();
                helpShown = false;
            }

            if (isOutside)
                field.Entered(pos);

            var hit = Physics2D.Raycast(pos, Vector2.up, 0.1f, spotMask);
            if (hit)
            {
                helpShown = true;
                spot = hit.collider.GetComponent<Spot>();
                field.ShowHelp(spot.action);
            }
            else
            {
                spot = null;
            }
        }
    }

    private void Interact()
    {
        if(field && isOutside && !spot)
        {
            anim.ResetTrigger("Act");
            anim.ResetTrigger("Cut");

            var tileType = field.GetTileType(pos);

            if (tileType == Tile.DEAD)
            {
                field.fertilizers++;
            }

            if (tileType == Tile.GRASS)
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

        if(spot)
        {
            spot.Use();
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

    public void ChangeScene(string scene)
    {
        dimmer.Close();
        sceneToSwitch = scene;
        Invoke("DoChangeScene", Dimmer.speed);
    }

    void DoChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneToSwitch);
    }
}
