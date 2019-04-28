using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlades : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var limit = 0.3f;
        transform.localPosition = new Vector3(Random.Range(-limit, limit), Random.Range(-limit, limit), 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
