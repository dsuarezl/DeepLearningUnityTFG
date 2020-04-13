using UnityEngine;
using MLAgents;
using System.Collections;
using TMPro;
using System.Collections.Generic;


public class TankArea : MonoBehaviour
{
    public float minDistance = 10;
    public float borderSize = 2;

    public TextMeshPro redAlive;

    public TextMeshPro blueAlive;

    private Vector3 min;
    private Vector3 max;

    public int defaultRedTeamNumber;
    public int defaultBlueTeamNumber;
    private int redTeamNumber;
    private int blueTeamNumber;

    public TankAgent redTankPrefab;

    public TankAgent blueTankPrefab;

    public GameObject redTeamSpawn;

    public GameObject blueTeamSpawn;

    private List<TankAgent> redTeam;
    private List<TankAgent> blueTeam;


    public float getSizeX()
    {
        return this.GetComponent<Renderer>().bounds.size.x;
    }

    public float getSizeZ()
    {
        return this.GetComponent<Renderer>().bounds.size.z;
    }


    public void Start()
    {
        redTeam = new List<TankAgent>();
        blueTeam = new List<TankAgent>();
        resetArea();
    }

    public void resetArea()
    {

        redTeamNumber = (int)Academy.Instance.FloatProperties.GetPropertyWithDefault("red_team_number", defaultRedTeamNumber);
        blueTeamNumber = (int)Academy.Instance.FloatProperties.GetPropertyWithDefault("blue_team_number", defaultBlueTeamNumber);


        int count = 0;
        while (redTeam.Count < redTeamNumber && count < 30)
        {
            TankAgent tank = Instantiate(redTankPrefab);
            tank.setArea(this);
            redTeam.Add(tank);
            count++;
        }
        foreach (TankAgent tank in redTeam)
        {
            placeTank(tank);
        }

        count = 0;
        while (blueTeam.Count < blueTeamNumber && count < 30)
        {
            TankAgent tank = Instantiate(blueTankPrefab);
            tank.setArea(this);
            blueTeam.Add(tank);
            count++;
        }
        foreach (TankAgent tank in blueTeam)
        {
            placeTank(tank);
        }

    }



    public void placeTank(TankAgent tank)
    {

        Rigidbody rigidbody = tank.GetComponent<Rigidbody>();


        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        tank.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

        if (tank.getTeam() == "red")
        {
            tank.transform.position = randomPosition(redTeamSpawn.GetComponent<Renderer>());
        }
        else if (tank.getTeam() == "blue")
        {
            tank.transform.position = randomPosition(blueTeamSpawn.GetComponent<Renderer>());
        }
    }

    public void placeTank(TankAgent tank, TankAgent target)
    {
        Rigidbody rigidbody = tank.GetComponent<Rigidbody>();


        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        tank.transform.position = randomPosition(this.GetComponent<Renderer>());

        int count = 0;
        while (Vector3.Distance(tank.transform.position, target.transform.position) < minDistance && count < 30)
        {
            tank.transform.position = randomPosition(this.GetComponent<Renderer>());
            count++;

        }
        tank.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }


    public void teamWins(List<TankAgent> teamWinner, List<TankAgent> teamLoser)
    {
        foreach (TankAgent tank in teamWinner)
        {
            tank.AddReward(1);
            tank.EndEpisode();

        }

        foreach (TankAgent tank in teamLoser)
        {
            tank.AddReward(-1);
            tank.EndEpisode();

        }

    }


    public void removeTankFromTeam(TankAgent tank)
    {
        if (tank.getTeam() == "red")
        {
            redTeam.Remove(tank);
        }
        else if (tank.getTeam() == "blue")
        {
            blueTeam.Remove(tank);
        }
    }

    private void Update()
    {

        if (redTeam.Count == 0)
        {
            teamWins(blueTeam, redTeam);
            resetArea();
        }
        else if (blueTeam.Count == 0)
        {

            teamWins(redTeam, blueTeam);
            resetArea();
        }

        redAlive.SetText(redTeam.Count.ToString());
        blueAlive.SetText(blueTeam.Count.ToString());
    }

    public Vector3 randomPosition(Renderer spawn)
    {
        min = spawn.bounds.min;
        max = spawn.bounds.max;

        return new Vector3((Random.Range(min.x + borderSize, max.x - borderSize)), 1, (Random.Range(min.z + borderSize, max.z - borderSize))); ;
    }

}
