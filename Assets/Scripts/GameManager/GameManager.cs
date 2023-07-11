using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;

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

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        GUILayout.BeginVertical();

        foreach (string name in players.Keys)
        {
            GUI.color = Color.red;
            GUILayout.Label(name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
