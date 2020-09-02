using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using TMPro;
using System.Collections.Generic;


public class TankAreaComunicacion : TankArea
{

    public  void Update()
    {

        foreach (int team in aliveTeams)
        {
            visualized[team].Clear();


            foreach (TankAgent tank in this.teams[team])
            {


                foreach (RayPerceptionOutput.RayOutput ro in RayPerceptionSensor.Perceive(tank.gameObject.GetComponent<RayPerceptionSensorComponent3D>().GetRayPerceptionInput()).RayOutputs)
                {


                    if (ro.HitTagIndex == 1)
                    {
                   
                        ro.HitGameObject.GetComponent<ParticleSystem>().Play();
                        visualized[team].Add(ro.HitGameObject.transform.localPosition);
                    }

                }





            }
        }
    }
}
