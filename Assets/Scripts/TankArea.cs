using UnityEngine;
using MLAgents;
using System.Collections;
using TMPro;


public class TankArea : MonoBehaviour
{
    public float minDistance = 10;
    public float borderSize = 2;

    public TextMeshPro redAlive;

    public TextMeshPro blueAlive;

    private Vector3 min;
    private Vector3 max;

    public GameObject redTeam;
    public GameObject blueTeam;


    public GameObject redTeamSpawn;

    public GameObject blueTeamSpawn;

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

        resetArea();
    }

    public void resetArea(){

        foreach (TankAgent tank in redTeam.GetComponentsInChildren<TankAgent>())
        {
            placeTank(tank);
        }


        foreach (TankAgent tank in blueTeam.GetComponentsInChildren<TankAgent>())
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

        if(tank.getTeam() == "red"){
            tank.transform.position = randomPosition(redTeamSpawn.GetComponent<Renderer>());
        }else if(tank.getTeam() == "blue"){
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


    public int aliveInTeam(GameObject team)
    {
        int alives = 0;
        foreach (TankAgent tank in team.GetComponentsInChildren<TankAgent>())
        {
            if (!tank.isDead())
            {
                alives++;
            }
        }

        return alives;
    }


    public void teamWins(GameObject teamWinner, GameObject teamLoser)
    {
        foreach (TankAgent tank in teamWinner.GetComponentsInChildren<TankAgent>())
        {
            tank.AddReward(1);
            tank.EndEpisode();

        }

        foreach (TankAgent tank in teamLoser.GetComponentsInChildren<TankAgent>())
        {
            tank.AddReward(-1);
            tank.EndEpisode();

        }

    }

    private void Update()
    {


        if (aliveInTeam(redTeam) == 0)
        {
            teamWins(blueTeam,redTeam);
        }else if(aliveInTeam(blueTeam) == 0){

            teamWins(redTeam,blueTeam);
        }

        redAlive.SetText(aliveInTeam(redTeam).ToString());
        blueAlive.SetText(aliveInTeam(blueTeam).ToString());
    }

    public Vector3 randomPosition(Renderer spawn)
    {
        min = spawn.bounds.min;
        max = spawn.bounds.max;

        return new Vector3((Random.Range(min.x + borderSize, max.x - borderSize)), 1, (Random.Range(min.z + borderSize, max.z - borderSize))); ;
    }

}
