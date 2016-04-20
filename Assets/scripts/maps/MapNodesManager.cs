using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class MapNodesManager : MonoBehaviour {

    [SerializeField] Transform poolNodesObject;
    List<MapNode> nodes;

    [SerializeField] GameObject m_pathsContainerObject;
    [SerializeField] GameObject m_pathTemplate;
    [SerializeField] float m_bodyScaleMult = 1.0f;

    [SerializeField] MapNode m_starterNode;
    MapNode m_currentNode;
    [SerializeField] MapCharacter m_player;

    
	void Start () {
        ListNodes();
        BuildPaths();
        //place the player at start
        Utils.Set2DPosition( m_player.transform, m_starterNode.transform.position);
	}

    void OnNodeTouchDown(object data)
    {
        Debug.Log("NODE DOWN");
    }

    void OnNodeTouchRelease()
    {
        Debug.Log("NODE RELEASe");
    }

    void Update () {
	
	}

    #region NODES_BUILDING
    /// <summary>
    /// Search for nodes components and store them in the list
    /// </summary>
    void ListNodes()
    {
        if (nodes == null)
            nodes = new List<MapNode>();
        foreach(Transform t in poolNodesObject)
        {
            MapNode node = t.GetComponent<MapNode>();
            nodes.Add(node);
        }
    }

    void BuildPaths()
    {
        foreach( var node in nodes)
        {
            foreach (var nodeChild in node.Children)
            {
                if( nodeChild != null )
                    BuildPath(node, nodeChild);
            }
        }
    }

    void BuildPath(MapNode node1, MapNode node2)
    {
        //Compute path's center position
        Vector3 toNode2 = node2.transform.position - node1.transform.position ;
        float mag = toNode2.magnitude;
        Vector3 mid = node1.transform.position + toNode2.normalized * mag *0.5f;
        mid.z = 1;

        //Instantiate template in scene
        var go = Instantiate(m_pathTemplate);
        Transform pathTransform = go.transform;
        go.name = "Path_" + node1.name + "_" + node2.name;
        //position
        pathTransform.parent = m_pathsContainerObject.transform;
        pathTransform.position = mid;
        //Scale
        Utils.SetLocalScaleX(pathTransform, mag * m_bodyScaleMult);
        //Rotation
        float angle = Utils.AngleBetweenVectors(Vector3.right, toNode2 );
        Utils.SetLocalAngleZ(pathTransform, angle);

    }
    #endregion
}
