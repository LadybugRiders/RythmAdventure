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
        var chara = ProfileManager.instance.GetCharacter(_characterId);
        LoadEquipment(chara);
        LoadAppearance(chara);
    }

    void LoadEquipment(ProfileManager.CharacterData _chara)
    {
        foreach( var equ in _chara.Equipments)
        {
            if (equ == null)
                continue;
            var eqData = DataManager.instance.CharacterManager.GetEquipement(equ.EquipmentType, equ.Id);
            if( eqData != null)
            {
                string pathToPrefab = "prefabs/equipments/" + equ.EquipmentType.ToString().ToLower();
                pathToPrefab += "/" + eqData.Prefab;
                //Load prefab
                GameObject go = Instantiate(Resources.Load(pathToPrefab)) as GameObject;
                if( go != null)
                {
                    go.transform.SetParent(m_equipmentsGO,false) ;
                }
            }
        }
    }

    void LoadAppearance(ProfileManager.CharacterData _chara)
    {
        foreach( var look in _chara.Looks )
        {
            if (look.Id == null)
                continue;
            var lookData = DataManager.instance.CharacterManager.GetLooks(look.LooksType, look.Id);
            if( lookData != null)
            {
                string pathToImage = "images/equipments" + look.LooksType.ToString().ToLower();
                pathToImage += "/" + lookData.Prefab;
                var sprite = Resources.Load(pathToImage) as Sprite;
                switch(lookData.type)
                {
                    case LooksType.EYES:
                        m_eyes.sprite = sprite;
                        break;
                    case LooksType.EYEBROWS:
                        m_eyebrows.sprite = sprite;
                        break;
                    case LooksType.FACE:
                        break;
                }
            }
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
