using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BattleDataAsset : ScriptableObject  {

    public SceneAsset scene;

    public string Name;

    public TextAsset Song;

    public Sprite Background;

    public List<GameObject> Enemies;

    public float TimeBegin = 0.0f;

    //For switching
    public int AttackNotesCount = 10;
    public int DefenseNotesCount = 5;
}
