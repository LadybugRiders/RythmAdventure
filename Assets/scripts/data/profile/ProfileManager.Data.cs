using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class ProfileManager {


    [System.Serializable]
    public class CharacterData
    {
        public string Id;
        public string Name = "temp";
        public int Xp = 0;
        public Job Job;

        //public CharacterBuild Build; // skins

        //Equipment
        public List<EquipmentData> Equipments;
        //Appearance
        public List<LooksData> Looks;

        //Attack
        public ActionData Attack;
        //Magics
        public List<ActionData> Magics;

        public string ColorId = "0";

        public CharacterData(string _id)
        {
            Id = _id;
            Equipments = new List<EquipmentData>();
            Looks = new List<LooksData>();
        }

        public string GetEquipmentId(EquipmentType _equipmentType)
        {
            return Equipments.Find(x => x.EquipmentType == _equipmentType).Id;
        }

        public void AddEquipement(string _id, EquipmentType _type)
        {
            Equipments.Add(new EquipmentData(_id, _type));
        }
        
        public void AddLooks(string _id, LooksType _type)
        {
            Looks.Add(new LooksData(_id, _type));
        }

        [System.Serializable]
        public class EquipmentData
        {
            public string Id;
            public EquipmentType EquipmentType;

            public EquipmentData(string id, EquipmentType _type)
            {
                Id = id;
                EquipmentType = _type;
            }

        }

        [System.Serializable]
        public class LooksData
        {
            public string Id;
            public LooksType LooksType;

            public LooksData(string id, LooksType _type)
            {
                Id = id;
                LooksType = _type;
            }
        }

        [System.Serializable]
        public class ActionData
        {
            public string Id;
            public bool equipped = false;

            public ActionData(string _id)
            {
                Id = _id;
            }
        }
    }

    [System.Serializable]
    public class Map
    {
        public string Name = "";
        public List<Level> Levels = new List<Level>();

        [System.Serializable]
        public class Level
        {
            public string Id;
            public int Score = 0;
        }
    }

    [System.Serializable]
    public class Item
    {
        public string Id;
        public int Quantity;
    }

}
