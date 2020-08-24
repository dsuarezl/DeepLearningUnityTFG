using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{


    public string[] tankTag;
    private void OnTriggerEnter(Collider col)
    {
        foreach(string tag in tankTag){
             if(col.gameObject.tag == tag){
                  col.gameObject.GetComponent<TankAgent>().takeDamage();
                  
             }
        }

        Destroy(this.gameObject);

        
    }

    private   void OnCollisionEnter(Collision collision){

        Destroy(this.gameObject);
    }

}
