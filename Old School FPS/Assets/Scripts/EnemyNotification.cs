using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNotification : MonoBehaviour
{
    public LevelManager manager;

    private void Start()
    {
        
    }
    public void Notify()
    {
        manager.weakEnemiesLeft--; 
    }
}
