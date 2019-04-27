using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmerAnim : MonoBehaviour
{
    public Farmer farmer;

    public void Interact() {
        farmer.DoInteract();
    }

    public void Bleed()
    {
        farmer.Bleed();
    }
}
