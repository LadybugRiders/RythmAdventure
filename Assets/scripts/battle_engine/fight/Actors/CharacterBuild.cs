using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterBuild : MonoBehaviour {
    
    [SerializeField] SpriteRenderer m_body;
    [SerializeField] SpriteRenderer m_eyebrows;
    [SerializeField] SpriteRenderer m_arm;
    [SerializeField] SpriteRenderer m_eyes;

    [SerializeField] Transform m_equipmentsGO;
    Dictionary<EquipmentType, GameObject> m_equipments = new Dictionary<EquipmentType, GameObject>();

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    #region RENDERING

    public void SetColor(Color _color)
    {
        m_body.color = _color;
        m_arm.color = _color;
        //m_eyebrows.color = _color;
    }

    public void Load(string _characterId)
    {
        LoadEquipment(_characterId);
    }

    void LoadEquipment(string _characterId)
    {
        var chara = ProfileManager.instance.GetCharacter(_characterId);
        foreach( var equ in chara.Equipments)
        {
            var eqData = DataManager.instance.CharacterManager.GetEquipement(equ.Type, equ.Id);
            if( eqData != null)
            {
                string pathToPrefab = "prefabs/equipments/" + equ.Type.ToString().ToLower();
                pathToPrefab += "/" + eqData.Prefab;
                //Load prefab
                Debug.Log(eqData.Prefab);
                GameObject go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
                if( go != null)
                {
                    go.transform.SetParent(m_equipmentsGO,false) ;
                }
            }
        }
    }

    void LoadAppearance()
    {
        //change color
        JSONObject colorObject = DataManager.instance.GameData.GetField("playerColor");
        if (colorObject != null)
        {
            Color c = new Color();
            c.r = colorObject[0].n;
            c.g = colorObject[1].n;
            c.b = colorObject[2].n;
            c.a = 1;
            SetColor(c);
        }
    }

    #endregion

    #region PROPERTIES

    public SpriteRenderer Arm { get { return m_arm; } }
    public SpriteRenderer Eyebrows { get { return m_eyebrows; } }
    public SpriteRenderer Body { get { return m_body; } }
    public SpriteRenderer Eyes { get { return m_eyes; } }

    #endregion
}
