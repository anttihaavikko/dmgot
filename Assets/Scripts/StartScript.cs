using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    public Dimmer dimmer;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CloseDimmer", 4.5f);
        Invoke("DoStart", 5.5f);
    }

    void CloseDimmer()
    {
        dimmer.Close();
    }

    void DoStart()
    {
        SceneManager.LoadSceneAsync("Hut");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
