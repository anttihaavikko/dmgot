using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

	public TextMeshPro textArea;
    public Transform helpIcon;

	private Vector3 hiddenSize = Vector3.zero;
	private Vector3 shownSize;

	private bool shown;
	private string message = "";
	private int messagePos = -1;

	public bool done = false;

	private AudioSource audioSource;
	public AudioClip closeClip;

	public GameObject clickHelp;

	private List<string> messageQue;

	public Color hiliteColor;
    string hiliteColorHex;

    bool useColors = true;
    private bool canSkip = false;

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

    // Update is called once per frame
    void Update () {

        if (Input.GetButtonUp("Interact"))
            EnableSkip();

		if (shown && Input.GetButtonDown("Interact") && canSkip) {
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

		if (Random.value < 0.2f) {
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
                //AudioManager.Instance.PlayEffectAt(14, Vector3.zero, 0.5f);
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

	public void ShowMessage(string str, bool colors = true) {
        canSkip = false;
        Invoke("EnableSkip", 0.25f);

        //AudioManager.Instance.PlayEffectAt (13, Vector3.zero, 0.5f);

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

        Invoke("ShowHelp", 2f);
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

        //AudioManager.Instance.PlayEffectAt(13, Vector3.zero, 0.3f);
        //AudioManager.Instance.Highpass (false);

//		AudioManager.Instance.PlayEffectAt (8, transform.position, 0.4f);

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
