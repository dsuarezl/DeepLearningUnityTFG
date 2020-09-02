using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

using TMPro;


public class TankAgentComunicacion : TankAgent
{

    public int enemies;


    public override void CollectObservations(VectorSensor sensor)
    {


        if (area == null)
        {

            for (int i = 0; i < 6 + 2 * enemies; i++)
            {
                sensor.AddObservation(dummyValue);
            }

        }
        else
        {
            //Health of the tank( 1 value)
            sensor.AddObservation(health / maxHealth);


            //Shooting power (1 float = 1 value)
            sensor.AddObservation((power - minPower) / (maxPower - minPower));

            //Shooting angle (1 float = 1 value)
            sensor.AddObservation((angle - minAngle) / (maxAngle - minAngle));

            //Tank position relative to parent object X 1 value



            float minX = -area.getSizeX() / 2;
            float maxX = area.getSizeX() / 2;

            float minZ = -area.getSizeZ() / 2;
            float maxZ = area.getSizeZ() / 2;


            sensor.AddObservation((transform.localPosition.x - minX) / (maxX - minX));

            //Tank position relative to parent object Z 1 value
            sensor.AddObservation((transform.localPosition.z - minZ) / (maxZ - minZ));

            //Rotation y of the tank (1 float = 1 value)
            sensor.AddObservation(transform.rotation.y / 360.0f);

            //Add position of visualized tanks from other allies
            foreach (Vector3 position in area.getVisualized(team))
            {

                sensor.AddObservation((position.x - minX) / (maxX - minX));


                sensor.AddObservation((position.z - minZ) / (maxZ - minZ));
            }

            for (int i = 0; i < enemies - area.getVisualized(team).Count; i++)
            {
                sensor.AddObservation(dummyValue);


                sensor.AddObservation(dummyValue);
            }
        }
    }







}