using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class MapNodesManager : MonoBehaviour {
    //nodes building
    [SerializeField] Transform m_poolNodesObject;
    List<MapNode> m_nodes;
    //paths building
    [SerializeField] GameObject m_pathsContainerObject;
    [SerializeField] GameObject m_pathTemplate;
    [SerializeField] float m_bodyScaleMult = 1.0f;

    //Nodes
    [SerializeField] MapNode m_starterNode;
    MapNode m_currentNode;
    MapNode m_targetNode;
    List<MapNode> m_nodesPath = null; // the path from the current node to the target (when moving)

    [SerializeField] MapCharacter m_player;

    public enum State    { IDLE,MOVING }
    private State m_state;
        
	void Start () {
        ListNodes();
        BuildPaths();
        //place the player at start
        Utils.Set2DPosition( m_player.transform, m_starterNode.transform.position);
        StartTouch();
        m_currentNode = m_nodes[0];
	}

    void OnNodeTouchDown(MapNode node)
    {
        Debug.Log("NODE DOWN");
    }

    void OnNodeTouchRelease(MapNode node)
    {
        Debug.Log("NODE RELEASe");
        //build the path of nodes
        m_nodesPath = new List<MapNode>();
        //Launch the walking
        m_nodesPath = CreatePathProcess(node);
        m_state = State.MOVING;
        m_targetNode = node;
    }

    void Update () {
        UpdateTouch();
        switch (m_state)
        {
            case State.MOVING: Moving(); break;
        }
	}

    void Moving()
    {
        if (m_currentNode == m_targetNode || m_nodesPath.Count == 0)
        {
            m_state = State.IDLE;
        }
        else if(m_player.IsMoving() == false)
        {
            MapNode nextNode = m_nodesPath[0];
            m_nodesPath.RemoveAt(0);
            m_player.GoTo(nextNode);
        }
    }

    List<MapNode> CreatePathProcess(MapNode _targetNode)
    {
        List<MapNode> nodes = new List<MapNode>();
        CreatePath(_targetNode, ref nodes);
        return nodes;    
    }

    bool CreatePath(MapNode processedNode, ref List<MapNode> path)
    {
        if( processedNode == m_currentNode)
        {
            path.Add(processedNode);
            return true;
        }
        foreach (var child in processedNode.Children)
        {
            if (child == null)
                continue;
            if( CreatePath(child, ref path))
            {
                path.Add(processedNode);
                return true;
            }
        }
        foreach (var parent in processedNode.Parents)
        {
            if (parent == null)
                continue;
            if (CreatePath(parent, ref path))
            {
                path.Add(processedNode);
                return true;
            }
        }
        return false;
    }

    #region NODES_BUILDING
    /// <summary>
    /// Search for nodes components and store them in the list
    /// </summary>
    void ListNodes()
    {
        if (m_nodes == null)
            m_nodes = new List<MapNode>();
        foreach(Transform t in m_poolNodesObject)
        {
            MapNode node = t.GetComponent<MapNode>();
            m_nodes.Add(node);
        }
    }

    void BuildPaths()
    {
        foreach( var node in m_nodes)
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
