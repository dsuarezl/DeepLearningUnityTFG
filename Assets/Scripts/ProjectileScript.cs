using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    private string targetTag;
    private TankAgent shooter;


    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "redTank" || col.gameObject.tag == "blueTank"){
            col.gameObject.GetComponent<TankAgent>().takeDamage();
        }

        Destroy(this.gameObject);
        
    }

}
