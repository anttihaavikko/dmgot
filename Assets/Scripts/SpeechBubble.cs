using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

	public TextMeshPro textArea;
    public Transform helpIcon;
    public SpriteRenderer helpImage;
    public Sprite[] helpSprites;

	private Vector3 hiddenSize = Vector3.zero;
	private Vector3 shownSize;

	private bool shown;
	private string message = "";
	private int messagePos = -1;
    private bool hidesWithAny = false;

    public bool done = false;

	private AudioSource audioSource;
	public AudioClip closeClip;

	public GameObject clickHelp;

	private List<string> messageQue;

	public Color hiliteColor;
    string hiliteColorHex;

    bool useColors = true;
    private bool canSkip = false;

    private string[] options;
    private string[] optionActions;
    private int optionSelection;

    // Use this for initialization
    void Awake () {
		textArea.text = "";
		shownSize = transform.localScale;
		transform.localScale = hiddenSize;
		audioSource = GetComponent<AudioSource> ();

		messageQue = new List<string> ();

        SetColor(hiliteColor);

        Invoke("EnableSkip", 0.25f);
	}

    void ShowHelp()
    {
        Tweener.Instance.ScaleTo(helpIcon, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);
    }

    void HideHelp()
    {
        Tweener.Instance.ScaleTo(helpIcon, new Vector3(2f, 0f, 1f), 0.3f, 0f, TweenEasings.QuarticEaseIn);
    }

    void EnableSkip()
    {
        canSkip = true;
    }

    void ShowBank()
    {
        ShowMessage("My balance: ($" + Manager.Instance.cash + ")\nFertilizers in stock: " + Manager.Instance.fertilizers);
    }

    void ShowMail()
    {
        ShowMessage("Mail etc etc");
    }

    void ShowShop()
    {
        ShowMessage("Shop etc etc");
    }

    void ShowBrowse()
    {
        ShowMessage("Browsing");
    }

    // Update is called once per frame
    void Update () {

        if(Manager.Instance.menuing)
        {
            int y = 0;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) y = 1;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) y = -1;

            optionSelection = (optionSelection + y) % options.Length;
            if (optionSelection == -1)
                optionSelection = options.Length - 1;

            UpdateMenu();

            if(Input.GetButtonDown("Interact"))
            {
                Invoke(optionActions[optionSelection], 0f);
                Hide();
                Manager.Instance.menuing = false;
            }
        }

        if (Input.GetButtonUp("Interact"))
            EnableSkip();

		if (shown && (Input.GetButtonDown("Interact") || (hidesWithAny && Input.anyKeyDown)) && canSkip) {
            HideHelp();

			if (!done) {
				done = true;
				messagePos = -1;
                textArea.text = useColors ? message.Replace("(", "<color=" + hiliteColorHex + ">").Replace(")", "</color>") : message;
			} else {
				if (messageQue.Count > 0) {
					PopMessage ();
				} else {
					Hide ();
				}
			}
		}

		if (shown) {
			transform.localScale = Vector3.MoveTowards (transform.localScale, shownSize, Time.deltaTime * 5f);
		} else {
			transform.localScale = Vector3.MoveTowards (transform.localScale, hiddenSize, Time.deltaTime * 5f);
		}

		if (Random.value < 0.1f) {
			return;
		}

		if (messagePos >= 0 && !done) {
			messagePos++;

			string msg = message.Substring (0, messagePos);

			int openCount = msg.Split('(').Length - 1;
			int closeCount = msg.Split(')').Length - 1;

            if (openCount > closeCount && useColors) {
				msg += ")";
			}

            textArea.text = useColors ? msg.Replace("(", "<color=" + hiliteColorHex + ">").Replace(")", "</color>") : msg;

			string letter = message.Substring (messagePos - 1, 1);

			if (messagePos == 1 || letter == " ") {
                AudioManager.Instance.PlayEffectAt(25, transform.position, 0.5f);
                AudioManager.Instance.PlayEffectAt(1, transform.position, 0.75f);
            }

			if (messagePos >= message.Length) {
				messagePos = -1;

				done = true;
			}
		}
	}

	public int QueCount() {
		return messageQue.Count;
	}

	public void SkipMessage() {
		done = true;
		messagePos = -1;
		textArea.text = message;
	}

    void UpdateMenu()
    {
        message = "\n";

        for(int i = 0; i < options.Length; i++)
        {
            message += optionSelection == i ? "> (" + options[i] + ") <" : options[i];
            message += "\n";
        }

        message += "\n";

        textArea.text = message.Replace("(", "<color=" + hiliteColorHex + ">").Replace(")", "</color>");
    }

    public void ShowMessage(string str, bool colors = true) {
        hidesWithAny = false;
        helpImage.transform.localScale = Vector3.zero;
        canSkip = false;
        Invoke("EnableSkip", 0.25f);

        AudioManager.Instance.PlayEffectAt(9, transform.position, 1f);
        AudioManager.Instance.PlayEffectAt(27, transform.position, 0.7f);

        if (str.Contains("[IMAGE1]"))
        {
            hidesWithAny = true;
            str = " ";
            helpImage.sprite = helpSprites[0];
            Tweener.Instance.ScaleTo(helpImage.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);
            HideHelp();
        }
        else
        {
            Invoke("ShowHelp", 2f);
        }

        useColors = colors;

        //AudioManager.Instance.Highpass ();

		if (closeClip) {
			audioSource.PlayOneShot (closeClip, 1f);
		}

		done = false;
		shown = true;
		message = str;
		textArea.text = "";

		Invoke ("ShowText", 0.2f);

        if (str.Contains("[OPTIONS1]"))
        {
            HideHelp();
            Manager.Instance.menuing = true;
            CancelInvoke("ShowText");
            CancelInvoke("ShowHelp");
            string[] opts = { "Bank", "Mail", "Browse", "Shop", "Nothing" };
            string[] optActs = { "ShowBank", "ShowMail", "ShowBrowse", "ShowShop", "Hide" };
            optionSelection = 0;
            options = opts;
            optionActions = optActs;
            SkipMessage();
            UpdateMenu();
            HideHelp();
        }
    }

	public void QueMessage(string str) {
		messageQue.Add (str);
	}

	public void CheckQueuedMessages() {
		if (messageQue.Count > 0 && !shown) {
			PopMessage ();
		}
	}

	private void PopMessage() {
		string msg = messageQue [0];
		messageQue.RemoveAt (0);
		ShowMessage (msg);
	}

	private void ShowText() {
		messagePos = 0;
	}

	public void HideAfter (float delay) {
		Invoke ("Hide", delay);
	}

	public void Hide() {


        //AudioManager.Instance.Highpass (false);

        AudioManager.Instance.PlayEffectAt (9, transform.position, 1f);
        AudioManager.Instance.PlayEffectAt(27, transform.position, 0.7f);

        if (closeClip) {
			audioSource.PlayOneShot (closeClip, 1f);
		}

		shown = false;
		textArea.text = "";
	}

	public bool IsShown() {
		return shown;
	}

	public void SetColor(Color color) {
        hiliteColorHex = "#" + ColorUtility.ToHtmlStringRGB (color);
	}
}
