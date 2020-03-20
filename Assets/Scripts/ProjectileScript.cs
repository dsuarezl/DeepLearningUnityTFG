using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{


    private bool hit;
    private bool hitTarget;
    private TankAgent target;


    public void setTarget(TankAgent target){
        this.target =  target;
    }
    
    void awake(){
        hit = false;
        hitTarget = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
      
        hit = true;
        if(other.gameObject.tag == "tank" && other.GetComponent<TankAgent>() == target){
            hitTarget = true;
        }

        
    }

    public bool hitted(){
        return hit;
    }

    public bool hittedTarget(){
        return hitTarget;
    }

}
