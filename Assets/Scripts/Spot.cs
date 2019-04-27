using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spot : MonoBehaviour
{
    public string action = "USE";
    public UnityEvent method;

    public void Use()
    {
        method.Invoke();
    }
}
