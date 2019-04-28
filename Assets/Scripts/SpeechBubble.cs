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
        ShowMessage("My balance: ($" + Manager.Instance.cash + ")\nFertilizers in stock: (" + Manager.Instance.fertilizers + ")");
    }

    void ShowMail()
    {
        if (Manager.Instance.messages.Count == 0)
        {
            ShowMessage("No (new) messages.");
        }
        else
        {
            if (Manager.Instance.messages.Count > 1)
                QueMessage("You have (" + Manager.Instance.messages.Count + ") new messages!");
            else
                QueMessage("You have (" + Manager.Instance.messages.Count + ") new message!");

            for (int i = 0; i < Manager.Instance.messages.Count; i++)
            {
                if(Manager.Instance.messages.Count > 1)
                {
                    QueMessage("Message (" + (i + 1) + ")");
                }
                var sections = Manager.Instance.messages[i].Split('|');
                foreach(var s in sections)
                    QueMessage(s);
            }

            Manager.Instance.messages.Clear();
        }

        CheckQueuedMessages();
    }

    void ShowShop()
    {
        QueMessage("What would you like to buy?");
        QueMessage("[SHOP]");
        CheckQueuedMessages();
    }

    void ShowBrowse()
    {
        string[] websites = {
            "(bloodfruitfarmers.org)\n--------------------------\n(anonymous) posted...|(Hot) tip!|(Eating) before going to (bed) makes your (blood) level stabilize better.",
            "Error (404)\n\nFile not found!",
            "(bloodfruitfarmers.org)\n--------------------------\n(conway) posted...|There is something (off) about these (plants).|I think I have to complain to the (B3/S23) committee...",
            "(webmd.com)\n--------------------------\n(breaking news)|(Five) plantations worth of (blood loss) is enough to (kill) a man.|Any (more) than that and you start (seeing colors).",
            "Error (500)\n\nInternal Server Error",
            "(webmd.com)\n--------------------------\n(breaking news)|Humans can (only) go for around a (week without any food).|Don't be a (tool),\n(feed) yourself!",
            "(bloodfruitfarmers.org)\n--------------------------\n(admin) posted...|Fruit (price) skyrockets!|The (blood fruit) are currently going on a crazy price of ($5) per fruit.|Now is the (time to sell) if you've been hoarding them!",
            "Please (update) and (restart) your browser...",
            "(isitchristmas.com)\n--------------------------\n(NO)",
            "(news.com)\n--------------------------\n(Science discovery)|There has been new (research) done on the (Blood Fruit) juice.|Some (scientist) believe that it is (highly addictive) and even (harmful) to the human body.",
            "(isitchristmas.com)\n--------------------------\n(YES)",
            "(news.com)\n--------------------------\n(Celebrity divorce)|The famous country singer duo (Sam & Sally) are getting divorced.|The reason behind this is most likely (Sam's) misuse of (Blood Fruit) juice.",
            "(ldjam.com)\n--------------------------\n(@pov) posted|It's almost time for the (Ludum Dare 44).|Are you ready? The theme is (Your life is currency).",
            "(news.com)\n--------------------------\n(Divorce backlash)|The famous country singer duo (Sam & Sally) were having some marital problems.|(Sally) threatened to get (a divorce) if (Sam) didn't stop his (Blood Fruit) juice snorting habits.|Yesterday (morning), the pair was (found dead) at their apartment.|It is suspected that (Sam) killed his wife in a bad (Blood Fruit) juice trip...|...and then (followed) it by (killing himself).",
        };

        var sections = websites[Manager.Instance.day - 1].Split('|');
        foreach (var s in sections)
            QueMessage(s);

        CheckQueuedMessages();
    }

    void Fertilizer1()
    {
        if(Manager.Instance.cash >= 1)
        {
            ShowMessage("Bought (1) fertilizer!");
            Manager.Instance.cash -= 1;
            Manager.Instance.fertilizers += 1;
        }
        else
        {
            ShowMessage("Insufficient funds!");
        }
    }

    void Fertilizer5()
    {
        if (Manager.Instance.cash >= 3)
        {
            ShowMessage("Bought (5) fertilizers!");
            Manager.Instance.cash -= 3;
            Manager.Instance.fertilizers += 5;
        }
        else
        {
            ShowMessage("Insufficient funds!");
        }
    }

    void Fertilizer10()
    {
        if (Manager.Instance.cash >= 5)
        {
            ShowMessage("Bought (10) fertilizers!");
            Manager.Instance.cash -= 5;
            Manager.Instance.fertilizers += 10;
        }
        else
        {
            ShowMessage("Insufficient funds!");
        }
    }

    // Update is called once per frame
    void Update () {

        if(Manager.Instance.menuing)
        {
            int y = 0;

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) y = 1;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) y = -1;

            if(Mathf.Abs(y) > 0)
            {
                AudioManager.Instance.PlayEffectAt(25, transform.position, 0.5f);
                AudioManager.Instance.PlayEffectAt(1, transform.position, 0.75f);
            }

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

        if (str.Contains("[IMAGE"))
        {
            var idx = int.Parse(str.Substring(6, 1)) - 1;
            if(str.Contains("[IMAGE1]")) {
                hidesWithAny = true;
            }
            str = " ";
            helpImage.sprite = helpSprites[idx];
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

        if (str.Contains("[SHOP]"))
        {
            HideHelp();
            Manager.Instance.menuing = true;
            CancelInvoke("ShowText");
            CancelInvoke("ShowHelp");
            string[] opts = { "1 Fertilizer / ($1)", "5 Fertilizers / ($3)", "10 fertilizers / ($5)", "Nothing" };
            string[] optActs = { "Fertilizer1", "Fertilizer5", "Fertilizer10", "Hide" };
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
