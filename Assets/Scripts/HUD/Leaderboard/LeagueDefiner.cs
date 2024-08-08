using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeagueDefiner 
{
    private string Wanderer;
    private string Explorer;
    private string Pathfinder;
    private string Trailblazer;
    List<string> leagues;

    public LeagueDefiner() { }

    public List<string> GetLeagues()
    {
        leagues.Add(Wanderer);
        leagues.Add(Explorer);
        leagues.Add(Pathfinder);
        leagues.Add(Trailblazer);

        return leagues;
    }
}
