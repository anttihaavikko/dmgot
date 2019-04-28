using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAfter : MonoBehaviour {

	public float delay = 0f;
	private Vector3 fullScale;
	public bool requiresNormalEnding = false;

	void Awake() {
		fullScale = transform.localScale;
	}

	// Use this for initialization
	void Start () {
        transform.localScale = new Vector3(fullScale.x * 2f, 0f, fullScale.z);

        Tweener.Instance.ScaleTo(transform, fullScale, 0.3f, delay, TweenEasings.BounceEaseOut);
        Invoke("DoSound", delay);
    }

	void DoSound() {
		AudioManager.Instance.PlayEffectAt (20, Vector3.zero, 1.5f);
        AudioManager.Instance.PlayEffectAt(28, Vector3.zero, 0.75f);
    }
}
