using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MixiGenerator {
    
    DataCharManager CharManager { get { return DataManager.instance.CharacterManager; } }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public ProfileManager.CharacterData Generate(int _tiers)
    {
        Job job = Job.WARRIOR;
        return Generate(_tiers, job);
    }

    public ProfileManager.CharacterData Generate(int _tiers, Job _job)
    {
        string id = "Mixi" + ProfileManager.instance.profile.Characters.Count;
        ProfileManager.CharacterData chara = new ProfileManager.CharacterData(id);

        chara.Job = _job;
        //Color
        var colors = CharManager.GetColors(_tiers);
        chara.ColorId = GameUtils.GetRandom(colors).Id;
        
        SetEquipement(chara, _job, _tiers);
        SetLooks(chara, _job, _tiers);
        SetSkills(chara, _job, _tiers);

        return chara;
    }
    
    void SetEquipement(ProfileManager.CharacterData _chara, Job _job, int _tiers)
    {
        //For each type of equipement
        for (int i = 0; i < Utils.EnumCount(EquipmentType.ACCESSORY); i++)
        {
            EquipmentType type = (EquipmentType)i;
            var randomEqpmnt = GetRandomEquipment(_job, type,_tiers);
            _chara.AddEquipement(randomEqpmnt.Id, type);
        }
    }

    void SetLooks(ProfileManager.CharacterData _chara, Job _job, int _tiers)
    {
        //For each type of skills
        for (int i = 0; i < Utils.EnumCount(LooksType.EYEBROWS); i++)
        {
            LooksType type = (LooksType)i;
            var randomLooks = GetRandomLooks(_job, type,_tiers);
            _chara.AddLooks(randomLooks.Id, type);
        }
    }

    void SetSkills(ProfileManager.CharacterData _chara, Job _job, int _tiers)
    {

    }

    #region CHARACTER_GENERATION
    
    public DataCharManager.BuildData GetRandomLooks(Job _job, LooksType _type, int _tiers)
    {
        var listOfLooks = CharManager.GetLooks(_type, _job, _tiers);
        int r = Random.Range(0, listOfLooks.Count - 1);
        return listOfLooks[r];
    }

    public DataCharManager.BuildData GetRandomEquipment(Job _job, EquipmentType _type, int _tiers)
    {
        var listOfEquipements = CharManager.GetEquipements(_type, _job, _tiers);
        int r = Random.Range(0, listOfEquipements.Count - 1);
        return listOfEquipements[r];
    }

    #endregion
}
