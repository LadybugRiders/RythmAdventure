using UnityEngine;
using System.Collections;

public class BattleActionEffect : MonoBehaviour {

    [SerializeField] protected SpriteRenderer m_effectSprite;

    protected bool m_launched = false;

    protected Animation m_animationComponent;
    protected Animator m_animator;

    // Use this for initialization
    protected virtual void Awake () {
        m_animationComponent = GetComponent<Animation>();
        m_animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        
    }

    virtual public void Launch(Vector3 _origin, Vector3 _destination)
    {
        m_launched = true;
    }

    virtual public void Die()
    {
        m_launched = false;
        gameObject.SetActive(false);
    }

    public bool IsLaunched
    {
        get
        {
            return m_launched;
        }
    }
}
