using UnityEngine;
using MLAgents;
using System.Collections;
 

public class TankArea : MonoBehaviour
{
    public TankAgent tank1;
    public TankAgent tank2;

    public float minDistance = 10;
    public float borderSize = 2;

    private Vector3 min;
    private Vector3 max;

    public void Start()
    {
        min = this.GetComponent<Renderer>().bounds.min;
        max = this.GetComponent<Renderer>().bounds.max;
        ResetArea();
    }
    public void ResetArea() {
            placeTank(tank1,tank2);
            placeTank(tank2,tank1);
            tank1.resetStats();
            tank2.resetStats();
    }

    public void placeTank(TankAgent tank, TankAgent target){
        Rigidbody rigidbody = tank.GetComponent<Rigidbody>();


        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        tank.transform.position = randomPosition();

        int count = 0;
        while(Vector3.Distance(tank.transform.position,target.transform.position) < minDistance && count < 30){
            tank.transform.position = randomPosition();
            count++;
     
        }

      //  Debug.Log(Vector3.Distance(tank.transform.position,target.transform.position));
        tank.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    public Vector3 randomPosition(){
        return new Vector3 ((Random.Range(min.x +borderSize, max.x -borderSize)), 1, (Random.Range(min.z +borderSize, max.z -borderSize)));;
    }

}
