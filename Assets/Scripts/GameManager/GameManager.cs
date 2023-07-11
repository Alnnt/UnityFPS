using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

    [SerializeField]
    public MatchingSettings MatchingSettings;

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    private void Awake()
    {
        Singleton = this;
    }

    public void RegisterPlayer(string name, Player player)
    {
        player.transform.name = name;
        players.Add(name, player);
    }

    public void UnRegisterPlayer(string name)
    {
        players.Remove(name);
    }

    public Player GetPlayer(string name)
    {
        return players[name];
    }
    
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        GUILayout.BeginVertical();

        GUI.color = Color.red;
        foreach (string name in players.Keys)
        {
            Player player = GetPlayer(name);
            GUILayout.Label(name + " - " + player.GetHealth());
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
