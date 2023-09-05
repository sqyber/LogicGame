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

        IsWord = 99,
        PlayerWord =100,
        WallWord,
        FlagWord,
        RockWord,
        HazardWord,

        YouWord=200,
        PushWord,
        WinWord,
        StopWord,
        SinkWord,

    }

    [CreateAssetMenu()][System.Serializable]
    public class LevelCreator : ScriptableObject
    {
    
        [SerializeField]
        public List<ElementTypes> level = new();
    }
}