using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Global { get; private set; }
    
    public GameManager Game { get; private set; }
    public AudioManager Audio { get; private set; }
    public System.Random Random { get; private set; } = new System.Random();

    private void Awake()
    {
        if (Global != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Global = this;
            DontDestroyOnLoad(this.gameObject);
        }
        Game = GetComponentInChildren<GameManager>();
        Audio = GetComponentInChildren<AudioManager>();
    }
}
