using UnityEngine;
using MLAgents;
using System.Collections;
using TMPro;
 

public class TankArea : MonoBehaviour
{
    public TankAgent tank1;
    public TankAgent tank2;

    public float minDistance = 10;
    public float borderSize = 2;


    public TextMeshPro redReward;

    
    public TextMeshPro blueReward;


    private Vector3 min;
    private Vector3 max;


    public float getSizeX(){
        return this.GetComponent<Renderer>().bounds.size.x;
    }

    public float getSizeZ(){
        return this.GetComponent<Renderer>().bounds.size.z;
    }
 

    public void Start()
    {
        min = this.GetComponent<Renderer>().bounds.min;
        max = this.GetComponent<Renderer>().bounds.max;
        placeTanks();
   
    }


    public void placeTanks(){
        placeTank(tank1,tank2);
        placeTank(tank2,tank1);
    }
    public void ResetArea() {
            
            tank1.EndEpisode();
            tank2.EndEpisode();
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
        tank.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }

    private void Update(){

        if(tank1.isDead()){
            tank2.SetReward(1f);
            tank1.SetReward(-1f);


            ResetArea();

        }else if(tank2.isDead()){

            tank1.SetReward(1f);
            tank2.SetReward(-1f);

            ResetArea();
        }

        redReward.SetText(tank2.GetCumulativeReward().ToString("0.00"));
        blueReward.SetText(tank1.GetCumulativeReward().ToString("0.00"));
    }

    public Vector3 randomPosition(){
        return new Vector3 ((Random.Range(min.x +borderSize, max.x -borderSize)), 1, (Random.Range(min.z +borderSize, max.z -borderSize)));;
    }

}
