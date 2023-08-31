using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }
    }
}