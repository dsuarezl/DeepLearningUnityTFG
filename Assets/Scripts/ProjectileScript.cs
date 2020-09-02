using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public TankAgent launcherTank;
    public string[] tankTag;
    private void OnTriggerEnter(Collider col)
    {


        foreach (string tag in tankTag)
        {
            if (col.gameObject.tag == tag)
            {
                if (tag != launcherTank.tag)
                {
                    launcherTank.AddReward(0.2f);
                }
                if (tag == launcherTank.tag)
                {
                    launcherTank.AddReward(-0.2f);
                }

                col.gameObject.GetComponent<TankAgent>().takeDamage();

            }
        }

        if (col.tag != this.tag)
            Destroy(this.gameObject);


    }

    public void setTank(TankAgent tank)
    {
        this.launcherTank = tank;
    }

    private void OnCollisionEnter(Collision collision)
    {

        Destroy(this.gameObject);
    }

}
