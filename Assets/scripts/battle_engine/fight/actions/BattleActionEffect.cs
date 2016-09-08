using UnityEngine;
using System.Collections;

public class BattleActionEffect : MonoBehaviour {

    [SerializeField] protected SpriteRenderer m_effectSprite;

    protected bool m_launched = false;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    virtual public void Launch(Vector3 _origin, Vector3 _destination)
    {
    }

    virtual public void Die()
    {
        m_launched = false;
    }

    public bool IsLaunched
    {
        get
        {
            return m_launched;
        }
    }
}
