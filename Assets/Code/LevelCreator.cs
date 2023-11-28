using System.Collections.Generic;
using UnityEngine;

namespace LogicGame
{
    public enum ElementTypes
    {
        Empty=0,
        Player,
        Wall,
        Rock,
        Flag,
        Hazard,

       
        WallWord,
        FlagWord,
        RockWord,
        HazardWord,
       
        PushWord,
        WinWord,
        StopWord,
        SinkWord,
        
        IsWord = 14,
        PlayerWord =15,
        YouWord=16

    }

    [CreateAssetMenu()][System.Serializable]
    public class LevelCreator : ScriptableObject
    {
    
        [SerializeField]
        public List<ElementTypes> level = new List<ElementTypes>();

        public LevelCreator()
        {
            level = new List<ElementTypes>();
        }
    }
}