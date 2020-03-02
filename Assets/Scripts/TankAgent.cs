using UnityEngine;
using MLAgents;
using System.Collections;


public class TankAgent : Agent
{

    public float angle = 45.0f;
    public float power = 10.0f;
    public float flightDuration = 3.0f;
    public float gravity = 15f;
    public TankAgent target;
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
    public const int maxHealth = 100;
    public const float maxPower = 100;
    public const float minPower = 1;

    public const float minAngle = 1;

    public const float maxAngle = 80;

    public TankArea area;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        dead = false;
        shooting = false;
        myTransform = transform;
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

        projectileObject = Instantiate(ProjectilePrefab) as GameObject;
        Transform projectileTransform = projectileObject.GetComponent<Transform>();

        ProjectileScript projectileScript = projectileObject.GetComponent<ProjectileScript>();

        shooting = true;

        projectileTransform.position = spawnPoint.transform.position;

        float Vx = Mathf.Sqrt(power) * Mathf.Cos(angle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(power) * Mathf.Sin(angle * Mathf.Deg2Rad);


        projectileTransform.rotation = myTransform.rotation;

        float elapse_time = 0;

        while (elapse_time < flightDuration && !projectileScript.hitted())
        {

            Vector3 newPos = new Vector3(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            if (float.IsNaN(newPos.y))
            {

                Debug.Log("ElapsedTime " + elapse_time);
                Debug.Log("DeltaTime" + Time.deltaTime);
                Debug.Log("hitted " + projectileScript.hitted());
                Debug.Log("vector " + new Vector3(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime));
                Debug.Log("Pos " + projectileTransform.position);

                Debug.Log(" ");
            }
            projectileTransform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;


            yield return null;

        }

        if (projectileScript.hitted())
        {
            float dist = Vector3.Distance(target.GetComponent<Transform>().position, projectileTransform.position);

            //Bigger distance less reward
            AddReward((1 / dist) - 0.7f);

        }

        Destroy(projectileObject);
        shooting = false;

    }

    public override void AgentAction(float[] vectorAction)
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


        // Apply a tiny negative reward every step to encourage action
        if (maxStep > 0)
            AddReward(-1f / maxStep);
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


    public override void AgentReset()
    {
        resetStats();
        area.placeTank(this,target);
    }


    public void resetStats()
    {
        if (projectileObject)
        {
            Destroy(projectileObject);
            StopCoroutine("SimulateProjectile");
            shooting = false;
        }


        dead = false;
        health = maxHealth;
    }


    public override void CollectObservations()
    {
        // Whether the tank is shooting (1 float = 1 value)
        AddVectorObs(isShooting());


        // Distance to the target (1 float = 1 value)
        AddVectorObs(Vector3.Distance(target.transform.position, transform.position));

        // Direction to target(1 Vector3 = 3 values)
        AddVectorObs((target.transform.position - transform.position).normalized);

        // Direction tank is facing (1 Vector3 = 3 values)
        AddVectorObs(transform.forward);


        // 1 + 1 + 3 + 3 = 8 total values
    }

    public bool isDead()
    {
        return dead;
    }

    private void FixedUpdate()
    {

        if (health <= 0)
        {
            AddReward(-2f);
            AgentReset();
        }
        else
        {

            // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
            // but for the steps in between, we need to call it explicitly to take action using the results
            // of the previous decision
            if (GetStepCount() % 5 == 0)
            {
                RequestDecision();
            }
            else
            {
                RequestAction();
            }
        }


    }

    private void takeDamage()
    {
        health -= 10;
        AddReward(-1f);
    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "projectile")
        {
            takeDamage();
        }
    }


}