using UnityEngine;
using Unity.MLAgents;

using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

public class counterWins2 : MonoBehaviour
{

    public int[] wins;
    public int matchesPlayed;

    public void Start()
    {
        wins = new int[2];
       wins[0] = 0;
        wins[1] = 0;
        matchesPlayed = 0;

    }


    public void updateWins(int team)
    {
        wins[team]++;
        matchesPlayed++;
         
        Debug.Log("0-> " + wins[0] + "\t1-> " + wins[1]  + "    m:" + matchesPlayed);
        
        if (matchesPlayed == 1000)
        {

            Debug.Log("0-> " + (wins[0] / 1000) + "\t1-> " + (wins[1] / 1000));
            EditorApplication.isPlaying = false;
        }
    }

}