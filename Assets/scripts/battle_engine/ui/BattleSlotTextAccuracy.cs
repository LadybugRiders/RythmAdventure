using UnityEngine;
using System.Collections;
using System.Reflection;

public class BattleSlotTextAccuracy : MonoBehaviour {

	[SerializeField] protected SpriteRenderer m_renderer;
	[SerializeField] protected Sprite m_perfectSprite;
	[SerializeField] protected Sprite m_greatSprite;
	[SerializeField] protected Sprite m_goodSprite;
	[SerializeField] protected Sprite m_badSprite;

	enum State{	HIT, MISS, DEAD};
	State m_state = State.DEAD;

	// Use this for initialization
	void Start () {
		Die ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Play(BattleScoreManager.Accuracy _accuracy){
		if (_accuracy == BattleScoreManager.Accuracy.BAD)
			PlayMiss ();
		else
			PlayHit (_accuracy);
	}

	void PlayHit(BattleScoreManager.Accuracy _accuracy){
		m_state = State.HIT;
		//change texture
		switch (_accuracy) {
			case BattleScoreManager.Accuracy.PERFECT : m_renderer.sprite = m_perfectSprite; break;
			case BattleScoreManager.Accuracy.GREAT : m_renderer.sprite = m_greatSprite; break;
			case BattleScoreManager.Accuracy.GOOD : m_renderer.sprite = m_goodSprite; break;			
		}
		//make visible
		SetAlpha (1);

		//Tween text
		Vector3 destination = transform.localPosition;
		destination.y += 0.45f;
		TweenEngine.instance.PositionTo (transform, destination, 0.3f, "OnTweenEnded");
	}

	void PlayMiss(){
		m_state = State.MISS;
		m_renderer.sprite = m_badSprite;
		//make visible
		SetAlpha (1);
		
		//Tween text
		Vector3 destination = transform.localPosition;
		destination.y -= 0.15f;
		TweenEngine.instance.PositionTo (transform, destination, 0.3f, "OnTweenEnded");
	}

	public void OnTweenEnded(object _go){
		//TimerEngine.instance.AddTimer (1.0f, "Die", gameObject);
		TweenEngine.instance.AlphaTo (m_renderer, 0.0f, 1.0f,"Die");
	}

	public void Die(){
		m_state = State.DEAD;
		SetAlpha (0);
		Utils.SetLocalPositionY (transform, 0.0f);
	}

	void SetAlpha(float _alpha){
		Color color = m_renderer.color;
		color.a = _alpha;
		m_renderer.color = color;
	}

	public bool IsAvailable{
		get{
			return m_state == State.DEAD;
		}
	}
}
