using UnityEngine;
using System.Collections;

public class BattleNoteLong : BattleNote {
	
	[SerializeField] protected Sprite m_blankSprite;

	[SerializeField] protected bool m_isHead = false;
	[SerializeField] protected BattleNoteLong m_pairNote;
	[SerializeField] protected SpriteRenderer m_bodySprite;

	protected float m_bodyScaleMultiplier = 1.0f;
	protected Transform m_bodyTransform;

	// Use this for initialization
	override protected void Start () {
		m_bodyScaleMultiplier = m_bodySprite.sprite.bounds.extents.x * 2;
		m_bodyTransform = m_bodySprite.gameObject.transform;
		base.Start ();
		m_transform.parent.position = new Vector3 ();
	}
	
	// Update is called once per frame
	override protected void Update () {
		base.Update ();
		if (!m_paused && IsHead && IsOnTrack) {
			UpdateBody();
		}
	}

	/** make the body follow the head */
	void UpdateBody(){
		//only the head gets to modify the body
		if (IsHead == false)
			return;
		float deltaX;
		Vector3 tmpVector = m_startPos;
		if (m_pairNote.CurrentState == State.LAUNCHED) {
			tmpVector = m_pairNote.transform.localPosition;
		}
		//diff between current pos and start pos of the note
		deltaX = m_transform.localPosition.x - tmpVector.x;
		//Change position
		Utils.SetLocalPositionX( m_bodyTransform, m_transform.localPosition.x - deltaX * 0.5f);
		//Change scale
		deltaX = Mathf.Abs (deltaX);
		Utils.SetLocalScaleX( m_bodyTransform, deltaX + deltaX * m_bodyScaleMultiplier);
		//change alpha
		Utils.SetAlpha (m_bodySprite, m_renderer.color.a);
	}

	#region ACTIONS

	//Hit : If HEAD place in slot center
	//     If TAIL , send second hit and Die
	override public void Hit(BattleSlot _slot){
		this.CurrentState = State.HIT;
		if (IsHead) {
			Utils.SetPositionX(m_transform, _slot.transform.position.x);
		} else {
			Die ();
			//notify the head
			m_pairNote.SecondHit();
		}
	}

	// Miss: If HEAD, kill itself and tail ( if launched )
	//		If TAIL : kill itself and head
	override public BattleNote[] Miss(){
		this.CurrentState = State.MISS;
		//Notify other
		if ( m_pairNote.IsOnTrack )
			m_pairNote.Die ();
		Die ();
		return new BattleNote[]{this, m_pairNote};
	}

	/** Called by the tail when it is hit*/
	public void SecondHit(){
		Die ();
	}
	
	/** Makes the note die if needs to be. If the note can be killed, return true */
	override public bool Die(){
		this.CurrentState = State.DEAD;
		if (IsHead) {
			Utils.SetAlpha (m_bodySprite, 0.0f);
			Utils.SetLocalPositionY(m_bodyTransform,-10000);
		}
		Utils.SetLocalPositionY(m_transform,-10000);
		Utils.SetAlpha (m_renderer, 0.0f);
		return true;
	}

	#endregion

	#region LAUNCH

	override protected bool Launch(){		
		//if this==TAIL and head is dead, do not launch
		if (IsHead == false && m_pairNote.IsDead) {
			Die ();
			return false;
		}
		this.CurrentState = State.LAUNCHED;
		Utils.SetLocalPositionY (m_bodyTransform, m_transform.localPosition.y);

        //color
        string colorName = m_track.TracksManager.IsAttacking ? "red_attack_bright" : "green_defense_bright";
        Color color = ColorManager.instance.GetColor(colorName);

        //set sprite
        if ( m_isHead ) {
			if( m_track.TracksManager.IsAttacking ){
				m_renderer.sprite = m_attackSprite;
                //change body color
                m_bodySprite.color = color;
            }
            else{
				m_renderer.sprite = m_defendSprite;
                //change body color
                m_bodySprite.color = color;
            }
		} else {
			m_renderer.sprite = m_blankSprite;
            m_renderer.color = color;

        }
		Utils.SetAlpha (m_renderer, 0.0f);

		return true;
	}
	#endregion

	public bool IsHead {
		get {
			return m_isHead;
		}
	}

	public BattleNoteLong TailNote{
		get{
			if( IsHead ){
				return m_pairNote;
			}
			return null;
		}
	}

	override public bool IsOnTrack {
		get{
			return m_state == State.LAUNCHED || ( m_state == State.HIT && IsHead );
		}
	}
}
