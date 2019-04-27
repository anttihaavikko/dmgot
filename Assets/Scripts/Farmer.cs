using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{

    private float moveDelay = 0f;
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DoMove();
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

        pos += new Vector3(dx, dy * 0.75f, 0);

        Tweener.Instance.MoveTo(transform, pos, 0.07f, 0f, TweenEasings.BounceEaseOut);
    }
}
