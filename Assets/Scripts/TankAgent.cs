﻿using UnityEngine;
using System.Collections;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;
using TMPro;


public class TankAgent : Agent
{
    public int team;
    public float angle = 45.0f;
    public float power = 10.0f;
    public float flightDuration = 3.0f;
    public float gravity = 15f;
    public GameObject spawnPoint;
    public GameObject ProjectilePrefab;
    private Transform myTransform;
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    private Rigidbody rb;
    private GameObject projectileObject;
    private bool dead;
    private bool shooting;
    private int health;
    public const int maxHealth = 30;
    public const float maxPower = 300;
    public const float minPower = 1;
    public const float minAngle = 1;
    public const float maxAngle = 80;
    public TankArea area;
    private string aliveTag;
    public string deadTag;


    public TextMeshPro healthText;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        healthText.SetText(health.ToString());
        dead = false;
        shooting = false;
        myTransform = transform;
        this.aliveTag = this.gameObject.tag;

    }



    public void Start()
    {

    }


    public void setArea(TankArea area)
    {
        this.area = area;
    }
    public int getTeam()
    {
        return team;
    }

    void shoot()
    {
        StartCoroutine("SimulateProjectile");
    }

    bool isShooting()
    {
        return shooting;
    }



    IEnumerator SimulateProjectile()
    {

        shooting = true;


        projectileObject = Instantiate(ProjectilePrefab) as GameObject;

        Transform projectileTransform = projectileObject.GetComponent<Transform>();

        ProjectileScript projectileScript = projectileObject.GetComponent<ProjectileScript>();

        projectileTransform.position = spawnPoint.transform.position;

        float Vx = Mathf.Sqrt(power) * Mathf.Cos(angle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(power) * Mathf.Sin(angle * Mathf.Deg2Rad);


        projectileTransform.rotation = myTransform.rotation;

        float elapse_time = 0;

        while (elapse_time < flightDuration && projectileObject)
        {

            Vector3 newPos = new Vector3(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
            projectileTransform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;


            yield return null;

        }

        if (elapse_time >= flightDuration || projectileObject)
            Destroy(projectileObject);


        shooting = false;

    }

    public override void OnActionReceived(float[] vectorAction)
    {

        if (!dead)
        {

            // Convert the first action to forward movement
            float forwardAmount = 0f;

            if (vectorAction[0] == 1f)
            {
                forwardAmount = 1f;
            }
            else if (vectorAction[0] == 2f)
            {
                forwardAmount = -1f;
            }

            // Convert the second action to turning left or right
            float turnAmount = 0f;
            if (vectorAction[1] == 1f)
            {
                turnAmount = -1f;
            }
            else if (vectorAction[1] == 2f)
            {
                turnAmount = 1f;
            }

            // Apply movement
            rb.MovePosition(transform.position + transform.forward * forwardAmount * moveSpeed * Time.fixedDeltaTime);
            transform.Rotate(transform.up * turnAmount * turnSpeed * Time.fixedDeltaTime);

            //angle up/down
            if (vectorAction[2] == 1f)
            {
                if (angle - 1 >= minAngle)
                    angle -= 1f;
            }
            else if (vectorAction[2] == 2f)
            {
                if (angle + 1 <= maxAngle)
                    angle += 1f;
            }

            //power up/down
            if (vectorAction[3] == 1f)
            {
                if (power - 1 >= minPower)
                    power -= 1f;
            }
            else if (vectorAction[3] == 2f)
            {
                if (power + 1 <= maxPower)
                    power += 1f;
            }

            //shoot
            if (vectorAction[4] == 1f)
            {
                if (!shooting)
                {
                    shoot();
                }
            }

        }



        // Apply a tiny negative reward every step to encourage action if not shooting
        /* if (!shooting)
             AddReward(-1f / maxStep);*/
    }

    public override float[] Heuristic()
    {
        float forwardAction = 0f;
        float turnAction = 0f;
        float shootAction = 0f;
        float angleAction = 0f;
        float powerAction = 0f;

        //0 1 2
        if (Input.GetKey(KeyCode.W))
        {
            // move forward
            forwardAction = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            // move forward
            forwardAction = 2f;
        }

        //0 1 2
        if (Input.GetKey(KeyCode.A))
        {
            // turn left
            turnAction = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // turn right
            turnAction = 2f;
        }

        //0 1 2
        if (Input.GetKey(KeyCode.Q))
        {
            // turn left
            angleAction = 1f;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // turn right
            angleAction = 2f;
        }


        //0 1 2
        if (Input.GetKey(KeyCode.Z))
        {
            // turn left
            powerAction = 1f;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            // turn right
            powerAction = 2f;
        }

        //0 1
        if (Input.GetKey(KeyCode.L))
        {
            shootAction = 1f;
        }


        // Put the actions into an array and return
        return new float[] { forwardAction, turnAction, angleAction, powerAction, shootAction };
    }


    public override void OnEpisodeBegin()
    {
        area.requestReset();
    }


    private void clearProjectile()
    {
        if (projectileObject)
        {
            Destroy(projectileObject);
            StopCoroutine("SimulateProjectile");
            shooting = false;
        }
    }
    public void resetStats()
    {

        clearProjectile();
        dead = false;

        health = maxHealth;
        this.gameObject.tag = aliveTag;
        healthText.SetText(health.ToString());
    }

    public override void CollectObservations(VectorSensor sensor)
    {


        //Whether the tank is dead ( 1 value)
        sensor.AddObservation(dead);

        //Number of agents alive in the same team
       // sensor.AddObservation(area.getAliveInTeam(team));


        if (dead)
        {

            for (int i = 0; i < 3; i++)
                sensor.AddObservation(-1000);

        }
        else
        {
     

            //Whether the tank is shooting ( 1 value)
           // sensor.AddObservation(isShooting());

            //Shooting power (1 float = 1 value)
            sensor.AddObservation((power - minPower) / (maxPower - minPower));

            //Shooting angle (1 float = 1 value)
            sensor.AddObservation((angle - minAngle) / (maxAngle - minAngle));

            //Tank position relative to parent object X 1 value

           /*float minX = -area.getSizeX() / 2;
            float maxX = area.getSizeX() / 2;

            float minZ = -area.getSizeZ() / 2;
            float maxZ = area.getSizeZ() / 2;

          

            sensor.AddObservation((transform.localPosition.x - minX) / (maxX - minX));

            //Tank position relative to parent object Z 1 value
            sensor.AddObservation((transform.localPosition.z - minZ) / (maxZ - minZ));

            // Distance to the target (1 float = 1 value)
            //AddVectorObs(Vector3.Distance(target.transform.position, transform.position));

            // Direction to target(1 Vector3 = 3 values)
            // AddVectorObs((target.transform.position - transform.position).normalized);*/


            //Rotation y of the tank (1 float = 1 value)
            sensor.AddObservation(transform.rotation.y / 360.0f);
        }


        // 1 + 1 + 1 + 1 + 1 + 1 = 6 total values
    }

    public bool isDead()
    {
        return dead;
    }


    public void takeDamage()
    {
        health -= 10;
        healthText.SetText(health.ToString());

        if (health <= 0)
        {
            dead = true;
            area.tankDead(team);
            clearProjectile();
            this.gameObject.tag = deadTag;
            healthText.SetText("Dead");

        }

    }


}