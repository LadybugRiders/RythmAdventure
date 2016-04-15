using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapNodesManager : MonoBehaviour {

    [SerializeField] Transform poolNodesObject;
    List<MapNode> nodes;

    [SerializeField] GameObject m_pathsContainerObject;
    [SerializeField] GameObject m_pathTemplate;
    
	// Use this for initialization
	void Start () {
        ListNodes();
        BuildPaths();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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

        //Instantiate template in scene
        var go = Instantiate(m_pathTemplate);
        Transform pathTransform = go.transform;
        go.name = "Path_" + node1.name + "_" + node2.name;
        //position
        pathTransform.parent = m_pathsContainerObject.transform;
        pathTransform.position = mid;
        //Scale
        Utils.SetLocalScaleX(pathTransform, mag);
        //Rotation
        float angle = Utils.AngleBetweenVectors(Vector3.right, toNode2 );
        Utils.SetLocalAngleZ(pathTransform, angle);

    }
}
