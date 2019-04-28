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
    public Transform mouth;

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

            Invoke("ShowMoveHelp", 2f);
        }
        else
        {
            Invoke("ShowHelpAfterDim", Dimmer.speed);
        }

        pos = transform.position;
    }

    void ShowMoveHelp()
    {
        bubble.ShowMessage("[IMAGE1]");
    }

    // Update is called once per frame
    void Update()
    {
        if (locked || Manager.Instance.menuing)
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

        if (Application.isEditor && Input.GetKeyDown(KeyCode.E))
        {
            Manager.Instance.cash += 10;
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

            AudioManager.Instance.PlayEffectAt(26, transform.position, 0.5f);
            AudioManager.Instance.PlayEffectAt(1, transform.position, 0.75f);
            AudioManager.Instance.PlayEffectAt(6, transform.position, 0.25f);

            CancelInvoke("ShowHelpAfterDim");
            CancelInvoke("ShowMoveHelp");

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
                Manager.Instance.fertilizers++;

                AudioManager.Instance.PlayEffectAt(0, transform.position, 1f);
                AudioManager.Instance.PlayEffectAt(14, transform.position, 1f);
                AudioManager.Instance.PlayEffectAt(11, transform.position, 1f);
                AudioManager.Instance.PlayEffectAt(20, transform.position, 1f);
            }

            if(tileType == Tile.FRUIT)
            {
                Manager.Instance.cash += 5;

                AudioManager.Instance.PlayEffectAt(20, transform.position, 1f);
                AudioManager.Instance.PlayEffectAt(27, transform.position, 1f);
                AudioManager.Instance.PlayEffectAt(2, transform.position, 1f);
            }

            if(tileType == Tile.NONE)
            {
                PlantSound();
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

    void PlantSound()
    {
        AudioManager.Instance.PlayEffectAt(0, transform.position, 1f);
        AudioManager.Instance.PlayEffectAt(6, transform.position, 1f);
        AudioManager.Instance.PlayEffectAt(11, transform.position, 1f);
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
        Invoke("DoorSound", 0.25f);
        Invoke("DoorSound", 0.75f);
        AudioManager.Instance.PlayEffectAt(14, transform.position, 0.5f);
        AudioManager.Instance.PlayEffectAt(15, transform.position, 0.5f);

        field.SaveGrid();
        dimmer.Close();
        sceneToSwitch = scene;
        Invoke("DoChangeScene", Dimmer.speed);
    }

    void DoorSound()
    {
        AudioManager.Instance.PlayEffectAt(13, transform.position, 1.5f);
    }

    void DoChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneToSwitch);
    }

    public void Sleep()
    {
        AudioManager.Instance.Highpass(true);
        Manager.Instance.day++;
        Manager.Instance.didSleep = true;
        dimmer.Close();

        if(Manager.Instance.cuts >= 5)
        {
            Manager.Instance.endTextOne = "But he never woke up";
            Manager.Instance.endTextTwo = "Due to losing too much blood";
            Invoke("GoEnd", 2f);
            return;
        }

        if (Manager.Instance.day > 14)
        {
            Manager.Instance.endTextOne = "The (two weeks) is done";
            Manager.Instance.endTextTwo = Manager.Instance.cash > 200 ? "And he earned enough for the rent, (congratulations)!" : "But he didn't earn enough for the rent...";
            Invoke("GoEnd", 2f);
            return;
        }

        mid.Show("DAY " + Manager.Instance.day, "$" + Manager.Instance.cash);
        Invoke("OpenDimmer", 4f);

        if (Manager.Instance.hasEaten && Manager.Instance.cuts < 5)
        {
            Manager.Instance.cuts = 0;
            Invoke("UpdateSaturation", 4f);
        }

        Manager.Instance.hasEaten = false;
    }

    void GoEnd()
    {
        ChangeScene("End");
    }

    void OpenDimmer()
    {
        AudioManager.Instance.Highpass(false);

        if (tableFruit)
            tableFruit.Show();

        dimmer.Open();

        Invoke("DoInsideTutorialMessages", 1f);
    }

    public void Bleed()
    {
        Manager.Instance.cuts++;
        UpdateSaturation();

        cam.BaseEffect(Manager.Instance.cuts * 1f);

        EffectManager.Instance.AddEffect(0, bleedPoint.position);

        AudioManager.Instance.PlayEffectAt(0, transform.position, 0.5f);
        AudioManager.Instance.PlayEffectAt(23, transform.position, 0.5f);
        AudioManager.Instance.PlayEffectAt(2, transform.position, 0.5f);

        Invoke("PlantSound", 0.8f);
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
        else
        {
            if (Manager.Instance.cash > 0)
            {
                Manager.Instance.cash--;
                Manager.Instance.hasEaten = true;
                ResetTriggers();
                anim.SetTrigger("Eat");
                tableFruit.gameObject.SetActive(false);

                AudioManager.Instance.PlayEffectAt(25, mouth.position, 1f);
                AudioManager.Instance.PlayEffectAt(26, mouth.position, 1f);
                AudioManager.Instance.PlayEffectAt(27, mouth.position, 1f);
            }
        }
    }

    public void EatEffect()
    {
        EffectManager.Instance.AddEffect(0, mouth.position + Vector3.back);

        AudioManager.Instance.PlayEffectAt(14, mouth.position, 1f);
        AudioManager.Instance.PlayEffectAt(16, mouth.position, 0.6f);
    }

    void DoInsideTutorialMessages()
    {
        if (Manager.Instance.day >= 8 && !Manager.Instance.hasSeenHalf)
        {
            Manager.Instance.hasSeenHalf = true;
            bubble.QueMessage("Uh oh, first the first week is gone already...");
            bubble.QueMessage("...and I'm still missing ($" + (Manager.goalCash - Manager.Instance.cash) + ") from my rent money.");
        }

        bubble.CheckQueuedMessages();
    }

    public void AccessComputer()
    {
        bubble.QueMessage("What should I do?");
        bubble.QueMessage("[OPTIONS1]");
        bubble.CheckQueuedMessages();
    }
}
