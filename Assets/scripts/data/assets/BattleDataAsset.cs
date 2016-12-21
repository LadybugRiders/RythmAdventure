using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BattleDataAsset : ScriptableObject  {

    public string sceneName;

    public string Name;

    public TextAsset Song;

    public Sprite Background;

    public List<EnemyBattleInfo> Enemies;

    public float TimeBegin = 0.0f;

    //For switching
    public int AttackNotesCount = 10;
    public int DefenseNotesCount = 5;

    [System.Serializable]
    public class EnemyBattleInfo
    {
        [SerializeField] public GameObject Prefab;
        [SerializeField] public string Id;
    }
}
