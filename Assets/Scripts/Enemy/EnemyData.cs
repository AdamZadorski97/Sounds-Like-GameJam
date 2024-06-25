using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyData", menuName = "Configuration/EnemyData")]
public class EnemyData : ScriptableObject
{

    public float frontFOVAngle = 90f; // Field of view angle for the front zone
    public float frontFOVDistance = 10f; // Distance for the front FOV zone
    public Color frontFOVColor = Color.red; // Color for the front FOV zone

    public LayerMask playerLayer; // Layer for players
    public LayerMask wallLayer; // Layer for walls
    public float checkInterval = 1f; // Interval for checking player distance
    public float attackRange;
  


}
