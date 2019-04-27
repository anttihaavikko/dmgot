using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

public class Farmer : MonoBehaviour
{
    public Field field;
    public Animator anim;
    public LayerMask blockMask, spotMask;
    public bool isOutside = true;
    public Dimmer dimmer;
    public MidScene mid;
    public Spot doorSpot;
    public SpeechBubble bubble;
    public PostProcessingBehaviour filters;
    public Fruit tableFruit;
    public EffectCamera cam;
    public Transform bleedPoint;

    private Vector3 pos;
    private bool locked;
    private bool helpShown;

    private Spot spot;
    private string sceneToSwitch;


    // Start is called before the first frame update
    void Start()
    {
        UpdateSaturation();

        if (Manager.Instance.justStarted)
        {
            transform.position = Vector3.zero;
            Manager.Instance.justStarted = false;
        }
        else
        {
            Invoke("ShowHelpAfterDim", Dimmer.speed);
        }

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

        if (Application.isEditor && Input.GetKeyDown(KeyCode.Q))
        {
            bubble.ShowMessage("This is a test (message) that is (there) or something!");
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

            CancelInvoke("ShowHelpAfterDim");

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

    void ResetTriggers()
    {
        anim.ResetTrigger("Act");
        anim.ResetTrigger("Cut");
    }

    private void Interact()
    {
        if(bubble.IsShown())
        {
            return;
        }

        if (field && isOutside && !spot)
        {
            ResetTriggers();

            var tileType = field.GetTileType(pos);

            if (tileType == Tile.DEAD)
            {
                field.fertilizers++;
            }

            if(tileType == Tile.FRUIT)
            {
                Manager.Instance.cash += 5;
            }

            if (tileType == Tile.GRASS)
            {
                anim.SetTrigger("Cut");
                locked = true;
                Invoke("Unlock", 1.5f);
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
        field.SaveGrid();
        dimmer.Close();
        sceneToSwitch = scene;
        Invoke("DoChangeScene", Dimmer.speed);
    }

    void DoChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneToSwitch);
    }

    public void Sleep()
    {
        Manager.Instance.day++;
        Manager.Instance.didSleep = true;
        dimmer.Close();
        mid.Show("DAY " + Manager.Instance.day, "$" + Manager.Instance.cash);
        Invoke("OpenDimmer", 4f);

        if(Manager.Instance.hasEaten && Manager.Instance.cuts < 5)
        {
            Manager.Instance.cuts = 0;
            Invoke("UpdateSaturation", 4f);
        }

        Manager.Instance.hasEaten = false;
    }

    void OpenDimmer()
    {
        if (tableFruit)
            tableFruit.Show();

        dimmer.Open();
    }

    public void Bleed()
    {
        Manager.Instance.cuts++;
        UpdateSaturation();

        cam.BaseEffect(Manager.Instance.cuts * 1f);

        EffectManager.Instance.AddEffect(0, bleedPoint.position);
    }

    public void UpdateSaturation()
    {
        ColorGradingModel.Settings g = filters.profile.colorGrading.settings;
        g.basic.saturation = 1f - 0.2f * Manager.Instance.cuts;
        filters.profile.colorGrading.settings = g;
    }

    public void Eat()
    {
        if (!tableFruit || !tableFruit.gameObject.activeInHierarchy)
        {
            bubble.ShowMessage("Unfortunately, there is (nothing) to eat.");
        }

        if (tableFruit && Manager.Instance.cash > 0)
        {
            Manager.Instance.cash--;
            Manager.Instance.hasEaten = true;
            ResetTriggers();
            anim.SetTrigger("Act");
            tableFruit.gameObject.SetActive(false);
        }
    }
}
