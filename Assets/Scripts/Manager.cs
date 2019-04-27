using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

    public int[] grid;
    public bool justStarted = true;
    public bool hasData = false;
    public bool didSleep = false;

    public int cash = 0;
    public int day = 1;

    public bool hasSeenDead = false;
    public bool hasSeenFruit = false;

    private static Manager instance = null;
	public static Manager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
            grid = new int[Field.GRIDSIZE * Field.GRIDSIZE];
            DontDestroyOnLoad(instance.gameObject);
        }
	}
}
