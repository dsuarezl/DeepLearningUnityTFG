using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

using TMPro;


public class TankAgent1v1 : TankAgent
{




    public override void CollectObservations(VectorSensor sensor)
    {


        //Health of the tank( 1 value)
        sensor.AddObservation(health / maxHealth);



        if (dead || area == null)
        {

            //for (int i = 0; i < 4 + 2 * enemies; i++)

            for (int i = 0; i < 3; i++)
                sensor.AddObservation(dummyValue);

        }
        else
        {
            //Shooting power (1 float = 1 value)
            sensor.AddObservation((power - minPower) / (maxPower - minPower));

            //Shooting angle (1 float = 1 value)
            sensor.AddObservation((angle - minAngle) / (maxAngle - minAngle));


            //Rotation y of the tank (1 float = 1 value)
            sensor.AddObservation(transform.rotation.y / 360.0f);


        }


    }


}