using UnityEngine;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using TMPro;
using System.Collections.Generic;


public class TankArea : MonoBehaviour
{
    public float borderSize;
    private Vector3 min;
    private Vector3 max;

    public TextMeshPro[] aliveText;

    //Number of teams
    public int numberOfTeams;

    //Number of tanks alive in a team
    private int[] aliveNumbers;

    //Default team size
    public int[] defaultTeamNumbers;

    //Efective team size
    private int[] teamNumbers;

    //Ids of alive teams
    private List<int> aliveTeams;

    //Prefabs of the team tanks
    public TankAgent[] tankPrefabs;

    //Spawns of the teams
    public GameObject[] teamSpawns;

    //Array of teams
    private List<TankAgent>[] teams;

    //Number of agents who have requested the reset of the area
    private int reset;

    //total number of agents in the area
    private int totalAgents;

    //sets of tanks visualized
    private HashSet<Vector3>[] visualized;

    //Radious of sphere cast
    public int sphereRadius = 30;

    public GameObject obstacles;

    public int getAliveInTeam(int teamId)
    {
        return aliveNumbers[teamId];
    }
    public float getSizeX()
    {
        return this.GetComponent<Renderer>().bounds.size.x;
    }

    public int getTeamNumber(int team)
    {
        return teamNumbers[team];
    }

    public float getSizeZ()
    {
        return this.GetComponent<Renderer>().bounds.size.z;
    }

    public HashSet<Vector3> getVisualized(int team)
    {
        return visualized[team];
    }



    public void Start()
    {

        aliveNumbers = new int[numberOfTeams];

        teamNumbers = new int[numberOfTeams];

        teams = new List<TankAgent>[numberOfTeams];
        visualized = new HashSet<Vector3>[numberOfTeams];

        reset = 0;
        totalAgents = 0;

        aliveTeams = new List<int>();

        for (int i = 0; i < teams.Length; i++)
        {
            teams[i] = new List<TankAgent>();
            visualized[i] = new HashSet<Vector3>();
        }

        resetArea();
    }


    public void adjustTeamNumber(List<TankAgent> team, int teamNumber, TankAgent prefab)
    {

        if (team.Count != teamNumber)
        {

            int count = 0;

            if (team.Count < teamNumber)
            {
                while (team.Count < teamNumber && count < 30)
                {
                    TankAgent tank = Instantiate(prefab);
                    tank.setArea(this);
                    team.Add(tank);
                    count++;
                }
            }
            else
            {

                while (team.Count > teamNumber && count < 30)
                {
                    removeTankFromTeam(team[0]);
                    count++;
                }
            }
        }

    }
    public void resetArea()
    {

        reset = 0;
        totalAgents = 0;

        obstacles.SetActive(Academy.Instance.EnvironmentParameters.GetWithDefault("obstacles", (obstacles.activeSelf ? 1f : 0f)) == 1f  );

        for (int i = 0; i < teams.Length; i++)
        {
            if (!aliveTeams.Contains(i))
                aliveTeams.Add(i);
        }

        for (int i = 0; i < numberOfTeams; i++)
        {
            teamNumbers[i] = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("team_" + i + "_number", defaultTeamNumbers[i]);
            aliveNumbers[i] = teamNumbers[i];
            totalAgents += teamNumbers[i];
        }




        for (int i = 0; i < numberOfTeams; i++)
        {

            adjustTeamNumber(teams[i], teamNumbers[i], tankPrefabs[i]);

            foreach (TankAgent tank in teams[i])
            {
                tank.resetStats();
                placeTank(tank);
            }

        }

    }

    public void placeTank(TankAgent tank)
    {

        Rigidbody rigidbody = tank.GetComponent<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        tank.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

        tank.transform.position = randomPosition(teamSpawns[tank.getTeam()].GetComponent<Renderer>());

    }


    public void teamWins(List<TankAgent> teamWinner)
    {

        foreach (List<TankAgent> team in teams)
        {
            if (team == teamWinner)
            {
                foreach (TankAgent tank in team)
                {
                    tank.AddReward(1);
                    tank.EndEpisode();

                }
            }
            else
            {
                foreach (TankAgent tank in team)
                {
                    tank.AddReward(-1);
                    tank.EndEpisode();

                }

            }
        }

    }

    public void requestReset()
    {
        reset++;

        Debug.Log(reset + " /" + totalAgents);
        if (reset >= totalAgents)
        {
            resetArea();
        }
    }


    public void removeTankFromTeam(TankAgent tank)
    {
        teams[tank.getTeam()].Remove(tank);

        Destroy(tank.gameObject);
    }


    public int aliveInTeam(List<TankAgent> team)
    {
        int alive = 0;

        foreach (TankAgent tank in team)
        {
            if (!tank.isDead())
                alive++;
        }
        return alive;
    }

    public void tie()
    {
        foreach (List<TankAgent> team in teams)
        {
            foreach (TankAgent tank in team)
            {
                tank.SetReward(0);
                tank.EndEpisode();
            }
        }

    }


    public void tankDead(int teamId)
    {
        aliveNumbers[teamId]--;


    }
    private void Update()
    {

        for (int i = 0; i < aliveTeams.Count; i++)
        {
            int id = aliveTeams[i];

            aliveText[id].SetText(aliveNumbers[id].ToString());

            if (aliveNumbers[id] <= 0)
            {
                aliveTeams.Remove(id);
                i--;
            }
        }

        if (aliveTeams.Count == 1)
        {
            teamWins(teams[aliveTeams[0]]);
        }
        else
        if (aliveTeams.Count == 0)
        {
            tie();
        }


        foreach (int team in aliveTeams)
        {
            visualized[team].Clear();


            foreach (TankAgent tank in this.teams[team])
            {
                


                if (!tank.isDead())
                {

                    foreach (RayPerceptionOutput.RayOutput ro in RayPerceptionSensor.Perceive(tank.gameObject.GetComponent<RayPerceptionSensorComponent3D>().GetRayPerceptionInput()).RayOutputs)
                    {


                        if (ro.HitTagIndex == 0)
                        {
                            ro.HitGameObject.GetComponent<ParticleSystem>().Play();
                            visualized[team].Add(ro.HitGameObject.transform.localPosition);
                        }

                    }

                }



            }
        }
    }

    public Vector3 randomPosition(Renderer spawn)
    {
        min = spawn.bounds.min;
        max = spawn.bounds.max;

        return new Vector3((Random.Range(min.x + borderSize, max.x - borderSize)), 1, (Random.Range(min.z + borderSize, max.z - borderSize))); ;
    }

}
