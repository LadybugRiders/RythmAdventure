using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BattleDataAsset : ScriptableObject  {

    public string Name;

    public TextAsset Song;

    public List<GameObject> Enemies;
}
