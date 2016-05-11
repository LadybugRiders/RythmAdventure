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
}
