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

        public string ColorId = "0";

        public CharacterData(string _id)
        {
            Id = _id;
            Equipments = new List<EquipmentData>()
            {
                new EquipmentData(EquipmentType.HAT, null),
                new EquipmentData(EquipmentType.WEAPON, null),
                new EquipmentData(EquipmentType.ACCESSORY, null),
            };
            Looks = new List<LooksData>()
            {
                new LooksData(LooksType.EYES, null),
                new LooksData(LooksType.EYEBROWS, null),
                new LooksData(LooksType.FACE, null),
            };
        }

        public string GetEquipmentId(EquipmentType _equipmentType)
        {
            return Equipments.Find(x => x.EquipmentType == _equipmentType).Id;
        }

        [System.Serializable]
        public class EquipmentData
        {
            public string Id;
            public EquipmentType EquipmentType;

            public EquipmentData(EquipmentType _type, string id)
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

            public LooksData(LooksType _type, string id)
            {
                Id = id;
                LooksType = _type;
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

}
