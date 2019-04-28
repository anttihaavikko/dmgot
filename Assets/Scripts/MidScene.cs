using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MidScene : MonoBehaviour
{
    public Text text1, text2;

    public void Show(string first, string second)
    {
        text1.text = first;
        text2.text = second;
        Tweener.Instance.ScaleTo(text1.transform, Vector3.one, 0.3f, 1f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(text2.transform, Vector3.one, 0.3f, 1.4f, TweenEasings.BounceEaseOut);
        Invoke("DoSound", 1f);
        Invoke("DoSound", 1.4f);
        Invoke("Hide", 2f);
    }

    void Hide()
    {
        Tweener.Instance.ScaleTo(text1.transform, new Vector3(2f, 0f, 1f), 0.3f, 1.4f, TweenEasings.QuarticEaseIn);
        Tweener.Instance.ScaleTo(text2.transform, new Vector3(2f, 0f, 1f), 0.3f, 1f, TweenEasings.QuarticEaseIn);
        Invoke("HideSound", 1.4f);
        Invoke("HideSound", 1f);
    }

    void DoSound()
    {
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 1.5f);
        AudioManager.Instance.PlayEffectAt(28, Vector3.zero, 0.75f);
    }

    void HideSound()
    {
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 0.8f);
        AudioManager.Instance.PlayEffectAt(26, Vector3.zero, 1f);
    }
}
