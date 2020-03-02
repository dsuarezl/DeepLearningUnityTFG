using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{


    private bool hit;

    void awake(){
        hit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        hit = true;
    }

    public bool hitted(){
        return hit;
    }

}
