using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages user inpur and time 
public class gameManager : MonoBehaviour
{
    public bool GameOver = false;
    public float BoardPeriod;
    public float AttackPeriod;

    public static gameManager instance;

    void Awake(){
        if (instance == null) {
            instance = this;
        }
    }
    //player input
}
