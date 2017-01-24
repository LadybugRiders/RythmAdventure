using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Scrolls xp on a Text component and fill a gauge at the same time
/// </summary>
public class UIXpScrollerManager : MonoBehaviour {

    [SerializeField] SpriteGauge m_gauge;
    [SerializeField] Text m_xpText;

    [SerializeField] Text m_levelText;
    
    protected bool m_scrolling = false;

    protected int m_currentNumber = 0;
    protected int m_targetNumber = 0;

    protected float m_timeByUnit = 1;
    protected float m_time = 0;
    protected int m_direction = 1;

    StoredLevelUpStats currentData;
    private int m_currentIndex = 0;
    private int m_currentLevel;

    public delegate void onScrollFinished(UIXpScrollerManager _scroller);
    protected onScrollFinished m_scrollFinishedDelegate = null;

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_scrolling)
        {
            m_time += Time.deltaTime;
            if (m_time >= m_timeByUnit)
            {
                m_time = 0;
                m_currentNumber += m_direction;
                SetGaugeValue();
                m_xpText.text = "" + m_currentNumber;
                if (m_currentNumber == 0)
                {
                    _ZeroReached();
                }
                if (m_currentNumber == m_targetNumber)
                {
                    TargetReached();
                }
            }
        }
    }

    void SetGaugeValue()
    {
        if(m_gauge != null)
        {

            int begin = currentData.beginNumbers[m_currentIndex];

            int length = Mathf.Abs( begin );
            float prog = 0;
            
            prog = Mathf.Abs(m_currentNumber - begin) / (float)length;

            if(m_gauge != null)
                m_gauge.SetValue(prog);
        }
    }

    protected virtual void _ZeroReached()
    {

    }

    protected virtual void TargetReached()
    {
        m_currentIndex++;
        if (m_currentIndex >= currentData.targetNumbers.Count)
        {
            m_scrolling = false;
            if( m_scrollFinishedDelegate != null)
            {
                m_scrollFinishedDelegate(this);
            }
        }
        else
        {
            //Set begin text and value
            m_currentNumber = currentData.beginNumbers[m_currentIndex];
            m_xpText.text = "" + m_currentNumber;
            //end
            m_targetNumber = currentData.targetNumbers[m_currentIndex];

            m_currentLevel++;
            if (m_levelText != null)
                m_levelText.text = "" + m_currentLevel;
        }
    }

    public virtual void Scroll(StoredLevelUpStats stats, float _duration, onScrollFinished _delegate = null)
    {
        currentData = stats;
        m_targetNumber = stats.targetNumbers[0];
        //level
        m_currentLevel = stats.oldLevel;
        if(m_levelText != null)
            m_levelText.text = "" + m_currentLevel;
        //xp
        m_currentNumber = stats.beginNumbers[0];
        m_xpText.text = "" + m_currentNumber;

        m_scrolling = true;
                
        //Get direction of the scroll
        int delta = m_targetNumber - m_currentNumber;
        m_direction = delta < 0 ? -1 : 1;
        //compute the speed
        m_timeByUnit = _duration / Mathf.Abs(stats.deltaXp);
        m_time = 0;

        m_scrollFinishedDelegate = _delegate;
    }

    public bool Scrolling
    {
        get
        {
            return m_scrolling;
        }
    }

    [System.Serializable]
    public class StoredLevelUpStats
    {
        public string charId;

        public int oldLevel = 0;
        public int newLevel = 0;
        public int oldXp = 0;
        public int newXp = 0;

        public int oldXpRequired = 0;
        public int newXpRequired = 0;
        
        public List<int> beginNumbers;
        public List<int> targetNumbers;

        public int deltaXp = 0;

        public bool isMaxLevel = false;

        public StoredLevelUpStats(string _teammateIndex)
        {
            charId = _teammateIndex;
        }

        public void Process()
        {
            var charManager = DataManager.instance.CharacterManager;
            ProfileManager.CharacterData mate = ProfileManager.instance.GetCharacter(charId);

            beginNumbers = new List<int>();
            targetNumbers = new List<int>();

            //Build all data needed to scroll several levels experiences
            for (int l = oldLevel + 1; l <= newLevel; l++)
            {
                var levelupdata = charManager.GetLevel(mate.Job, l);
                //add start
                int startValue = (l == oldLevel + 1) ? (oldXpRequired - oldXp) : levelupdata.XpNeeded;
                beginNumbers.Add(startValue);
                //end value
                int endValue = (l == newLevel) ? (newXpRequired - mate.Xp) : 0;
                targetNumbers.Add(endValue);

                deltaXp += startValue;
            }
        }

        public bool HasLeveledUp
        {
            get
            {
                return oldLevel != newLevel;
            }
        }
    }
}
