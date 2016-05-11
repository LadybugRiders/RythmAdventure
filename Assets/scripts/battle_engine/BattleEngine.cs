using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattleEngine : MonoBehaviour {

	public static BattleEngine _instance = null;
	public enum Difficulty { EASY, MEDIUM, HARD};

	[SerializeField] protected BattleTracksManager m_tracksManager;
	[SerializeField] protected BattleFightManager m_fightManager;

	[SerializeField] protected BattleScoreManager m_scoreManager;
	[SerializeField] protected GameObject m_notesGeneratorObject;
	protected BattleNotesGenerator m_notesGenerator;

	[SerializeField] protected UIBattleManager m_ui;

	//phase switch variables
	[SerializeField] int m_switchAttackBaseCount = 10;
	[SerializeField] int m_switchDefendBaseCount = 6;
	int m_switchCount = 0;
	int m_nextSwitchCount = 0;
    
	bool m_debug = false;
	SongEditorTestLauncher m_testLauncher;

    [SerializeField] BattleDataAsset m_defaultBattleData;

	//Audio
	protected AudioSource m_audioSource;
	protected AudioClip m_audioClip;
	/** Time for a note from its launch to its arrival */
	protected float m_timeShift;
	protected float m_sampleRateToTimeModifier;

	void Awake(){
		_instance = this;
		//TODO remove this. ensures that the data is loaded 
		DataManager.instance.ToString();
	}

	// Use this for initialization
	void Start () {
		m_audioSource = GetComponent<AudioSource> ();
		//Search for TestLauncher. If present, it's in debug mode
		m_testLauncher = FindObjectOfType(typeof(SongEditorTestLauncher)) as SongEditorTestLauncher;
		m_debug = m_testLauncher != null;

		BattleNotesGenerator[] gens = m_notesGeneratorObject.GetComponents<BattleNotesGenerator> ();
		for (int i=0; i < gens.Length; i++) {
			if (gens [i].enabled)
				m_notesGenerator = gens [i];
		}
		TimerEngine.instance.AddTimer (1.0f, "BeginBattle", gameObject);
	}

	void BeginBattle(){		
		if (m_debug) {
			LoadSong( m_testLauncher.songName, m_testLauncher.difficulty);
			m_notesGenerator.BeginDebug(m_timeShift, m_testLauncher.timeBegin);
		} else {
            if( DataManager.instance.BattleData != null)
            {
                BattleDataAsset battleData = DataManager.instance.BattleData;
                LoadSong(battleData);
            }
            else
            {
                //LoadSong(m_songName,m_difficulty);
            }
            //Start Generator
            m_notesGenerator.Begin (m_timeShift);
		}
		//Play song
		m_audioSource.clip = m_audioClip;
		m_audioSource.Play ();
		SwitchPhase ();
		m_nextSwitchCount = m_switchDefendBaseCount;

		//debug 2
		if( m_debug)
			m_audioSource.time = m_testLauncher.timeBegin;
	}

	void LoadResources(BattleDataAsset battleData){
		m_fightManager.Load (battleData);
	}

	public void OnQuitBattle(){
        SceneManager.LoadScene("main_menu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    #region LOADING

    void LoadSong(string _songName, Difficulty _difficulty)
    {
        //Load Data
        string dataSongName = _songName + "_" + _difficulty.ToString().ToLower();
        TextAsset jsonFile = Resources.Load("song_data/" + _songName + "/" + dataSongName) as TextAsset;

        JSONObject jsonData = new JSONObject(jsonFile.text);

        //Load Note Generator
        m_notesGenerator.LoadData(jsonData);

        //Load song music
        string clipName = jsonData.GetField("clipName").str;
        m_audioClip = Resources.Load("songs/" + clipName) as AudioClip;
        m_sampleRateToTimeModifier = 1.0f / m_audioClip.frequency;

        m_timeShift = jsonData.GetField("timeSpeed").n;
        m_tracksManager.SetTimeShift(m_timeShift);

        m_fightManager.Load(null);
    }

    void LoadSong(BattleDataAsset battleData)
    {
        //Load Battle Data
        TextAsset jsonFile = battleData.Song;
        JSONObject jsonData = new JSONObject(jsonFile.text);
        string clipPath = jsonData.GetField("clipPath").ToString();

        //Load Note Generator
        m_notesGenerator.LoadData(jsonData);

        //Load song music
        string clipName = jsonData.GetField("clipName").str;
        m_audioClip = Resources.Load("songs/" + clipName) as AudioClip;
        m_sampleRateToTimeModifier = 1.0f / m_audioClip.frequency;

        m_timeShift = jsonData.GetField("timeSpeed").n;
        m_tracksManager.SetTimeShift(m_timeShift);

        LoadResources(battleData);
    }

    #endregion

    #region SWITCH_ATTACK_DEFENSE

    /** Called from tracks manager when a note is launched.
	 * Checks how many notes has been launched is the current phase and switch if necessary */
    public void OnNoteLaunched(NoteData _data){
		m_switchCount ++;
        //Don't switch between a long note
		if (_data.Type == NoteData.NoteType.LONG && _data.Head) {
			return;
		}
		//when it's time to switch
		if (!m_debug && m_switchCount >= m_nextSwitchCount) {
			SwitchPhase();
		}
	}

	void SwitchPhase(){
		m_tracksManager.SwitchPhase ();		
	}

	public void OnSwitchSuccessful(){
		if (m_tracksManager.PhaseState == BattleTracksManager.BattleState.ATTACK)
			m_nextSwitchCount = m_switchAttackBaseCount;
		else
			m_nextSwitchCount = m_switchDefendBaseCount;
		m_switchCount = 0;
		//UI
		m_ui.SwitchPhase (m_tracksManager.IsAttacking);
	}

	#endregion

	#region FIGHT

	public void OnLaunchMagic(BattleFightMagic _magic){
	}

	public void OnMagicEnded(BattleFightMagic _magic){
	}

	/** Called by the FightManager when a group of actor is dead on one side/
	* 	Disables the track specified */ 
	public void OnDisableTrack(int _index, int _replacementTrack){
		m_tracksManager.DisableTrack (_index, _replacementTrack);
	}

	public void OnFightEnded(bool _win){
        SceneManager.LoadScene("battle_end");
	}

	#endregion

	/** Called by the TracksManager when a note is hit or missed */
	public BattleScoreManager.Accuracy AddNote( NoteData _note, float _accuracy){
		_note.HitAccuracy = m_scoreManager.AddNote (_accuracy);
		m_fightManager.AddNote(_note);
		return _note.HitAccuracy;
	}
    
    #region GETTERS

    public static BattleEngine instance {
		get{
			return _instance;
		}
	}

	public BattleTracksManager.BattleState BattleState{
		get{
			return m_tracksManager.PhaseState;
		}
	}

	public AudioSource AudioSrc {
		get {
			return m_audioSource;
		}
	}

	public float TimeElapsed{
		get{
			return m_audioSource.timeSamples * m_sampleRateToTimeModifier;
		}
	}
	                      
	public bool IsAttacking{
		get{
			return m_tracksManager.IsAttacking;
		}
	}

	#endregion
}
