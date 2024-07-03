using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Custom/Create LevelData Asset")]
public class LevelData : ScriptableObject
{
    public List<LevelInfo> levels = new List<LevelInfo>();
}

[Serializable]
public struct LevelInfo
{
    public int Level;
    //public int Rows;
    public int StartingRow;

    [SerializeField]
    public List<EnemyType> Rows;
}