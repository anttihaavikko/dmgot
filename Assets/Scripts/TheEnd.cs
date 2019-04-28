using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheEnd : MonoBehaviour
{
    public Text[] texts;
    public Color color;
    public Dimmer dimmer;

    private string hilite;
    private int selected = 0;

    // Start is called before the first frame update
    void Start()
    {
        hilite = "#" + ColorUtility.ToHtmlStringRGB(color);

        texts[0].text = "DAY " + Manager.Instance.day;
        texts[1].text = "$" + Manager.Instance.cash;

        texts[2].text = Manager.Instance.endTextOne.Replace("(", "<color=" + hilite + ">").Replace(")", "</color>");
        texts[3].text = Manager.Instance.endTextTwo.Replace("(", "<color=" + hilite + ">").Replace(")", "</color>");

        texts[4].text = Colorized("TRY AGAIN");
        texts[5].text = "QUIT";
    }

    string Colorized(string str)
    {
        return "> <color=" + hilite + ">" + str + "</color> <";
    }

    // Update is called once per frame
    void Update()
    {
        int y = 0;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) y = -1;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) y = 1;

        if(Mathf.Abs(y) > 0)
        {
            AudioManager.Instance.PlayEffectAt(25, Vector3.zero, 0.5f);
            AudioManager.Instance.PlayEffectAt(1, Vector3.zero, 0.75f);
        }

        selected = (selected + y) % 2;

        if(selected == 0)
        {
            texts[4].text = Colorized("TRY AGAIN");
            texts[5].text = "QUIT";
        }
        else
        {
            texts[4].text = "TRY AGAIN";
            texts[5].text = Colorized("QUIT");
        }

        if(Input.GetButtonDown("Interact") || Input.GetKeyDown(KeyCode.Return))
        {
            if(selected == 0)
            {
                dimmer.Close();
                Invoke("TryAgain", 1f);
            }
            else
            {
                Debug.Log("Quit");
                Application.Quit();
            }
        }
    }

    void TryAgain()
    {
        AudioManager.Instance.Highpass(false);
        Manager.Instance.Redo();
        SceneManager.LoadSceneAsync("Hut");
    }
}
