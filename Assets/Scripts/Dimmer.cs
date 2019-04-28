using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimmer : MonoBehaviour
{
    public Transform upper, lower;
    public const float speed = 0.8f;

    private void Awake()
    {
        upper.transform.localScale = Vector3.one;
        lower.transform.localScale = Vector3.one;
    }

    // Start is called before the first frame update
    void Start()
    {
        Open();
    }

    public void Close()
    {
        Tweener.Instance.ScaleTo(upper, new Vector3(1, 1, 1), speed, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(lower, new Vector3(1, 1, 1), speed, 0f, TweenEasings.BounceEaseOut);

        MoveSound();
        Invoke("EndSound", speed * 0.3f);
    }

    public void Open()
    {
        Tweener.Instance.ScaleTo(upper, new Vector3(1, 0, 1), speed, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(lower, new Vector3(1, 0, 1), speed, 0f, TweenEasings.BounceEaseOut);

        MoveSound();
        Invoke("EndSound", speed * 0.3f);
    }

    void EndSound()
    {
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 1.5f * 0.75f);
        AudioManager.Instance.PlayEffectAt(28, Vector3.zero, 0.75f * 0.75f);
        Invoke("SecondEndSound", 0.2f);
    }

    void SecondEndSound()
    {
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 1.5f * 0.5f);
        AudioManager.Instance.PlayEffectAt(28, Vector3.zero, 0.75f * 0.5f);
    }

    void MoveSound()
    {
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 0.8f);
        AudioManager.Instance.PlayEffectAt(26, Vector3.zero, 1f);
    }
}
