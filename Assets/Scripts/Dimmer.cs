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
    }

    public void Open()
    {
        Tweener.Instance.ScaleTo(upper, new Vector3(1, 0, 1), speed, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(lower, new Vector3(1, 0, 1), speed, 0f, TweenEasings.BounceEaseOut);
    }
}
