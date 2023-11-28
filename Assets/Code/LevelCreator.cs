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

        IsWord =6,
        PlayerWord=7,
        WallWord,
        FlagWord,
        RockWord,
        HazardWord,

        YouWord =12,
        PushWord,
        WinWord,
        StopWord,
        SinkWord,

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