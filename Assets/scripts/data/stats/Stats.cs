using UnityEngine;
using System.Collections;
using System.Reflection;

public class Stats {

	protected int m_level = 1;

	protected int m_HP= 0;

	protected int m_MP = 0;

	protected int m_attack = 0;
	protected int m_defense = 0;
	protected int m_magic = 0;
    private int m_speed = 0;

    public float blockPerfectModifier = 0.9f;
	public float blockGreatModifier = 0.6f;
	public float blockGoodModifier = 0.3f;
	public float blockBadModifier = -0.3f;

    public Stats() { }

    public Stats(Stats stats)
    {
        Utils.CopyProperties(stats, this);
    }

    public Stats(JSONObject json)
    {
        //stats
        Level = (int)json.GetField("level").f;
        Attack = (int)json.GetField("attack").f;
        Defense = (int)json.GetField("defense").f;
        Magic = (int)json.GetField("magic").f;
        HP = (int)json.GetField("hp").f;
        MP = (int)json.GetField("mp").f;
        Speed = (int)json.GetField("speed").f;
    }

    public Stats Add(Stats _stats)
    {
        Attack += _stats.Attack;
        Defense += _stats.Defense;
        Magic += _stats.Magic;
        HP += _stats.HP;
        MP += _stats.MP;
        Speed += _stats.Speed;
        return this;
    }

    public Stats Subtract(Stats _stats)
    {
        Attack -= _stats.Attack;
        Defense -= _stats.Defense;
        Magic -= _stats.Magic;
        HP -= _stats.HP;
        MP -= _stats.MP;
        Speed -= _stats.Speed;
        return this;
    }

    public int Level {
		get {
			return m_level;
		}
		set {
			m_level = value;
		}
	}

	public int HP {
		get {
			return m_HP;
		}
		set {
			m_HP = value;
		}
	}

	public int MP {
		get {
			return m_MP;
		}
		set {
			m_MP = value;
		}
	}

	public int Attack {
		get {
			return m_attack;
		}
		set {
			m_attack = value;
		}
	}

	public int Defense {
		get {
			return m_defense;
		}
		set {
			m_defense = value;
		}
	}

	public int Magic {
		get {
			return m_magic;
		}
		set {
			m_magic = value;
		}
	}

    public int Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_speed = value;
        }
    }
}
