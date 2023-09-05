using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LogicGame
{
    [CustomEditor(typeof(LevelCreator))] 
    public class LevelCreatorInspector : Editor
    {
        Dictionary<ElementTypes, Texture> _textureHolder = new();
        private void OnEnable()
        {
            //for editor sprites when done
            //template: textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png"));
            _textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png"));
            _textureHolder.Add(ElementTypes.IsWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/IsWord.png"));
            _textureHolder.Add(ElementTypes.Player, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Player.png"));
            _textureHolder.Add(ElementTypes.Wall, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Wall.png"));
            _textureHolder.Add(ElementTypes.PlayerWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/PlayerWord.png"));
            _textureHolder.Add(ElementTypes.WallWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/WallWord.png"));
            _textureHolder.Add(ElementTypes.YouWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/YouWord.png"));
            _textureHolder.Add(ElementTypes.PushWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/PushWord.png"));
            _textureHolder.Add(ElementTypes.WinWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/WinWord.png"));
            _textureHolder.Add(ElementTypes.StopWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/StopWord.png"));
            _textureHolder.Add(ElementTypes.Flag, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Flag.png"));
            _textureHolder.Add(ElementTypes.FlagWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/FlagWord.png"));
            _textureHolder.Add(ElementTypes.Rock, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Rock.png"));
            _textureHolder.Add(ElementTypes.RockWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/RockWord.png"));
            _textureHolder.Add(ElementTypes.Hazard, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Hazard.png"));
            _textureHolder.Add(ElementTypes.HazardWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/HazardWord.png"));
            _textureHolder.Add(ElementTypes.SinkWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/SinkWord.png"));
            

        }

        private ElementTypes _currentSelected = ElementTypes.Empty;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("Current Selected : " + _currentSelected.ToString());

            var levelCreator = (LevelCreator)target;
            var rows = (int)Mathf.Sqrt(levelCreator.level.Count);
            //int currentI = levelCreator.level.Count-1;
            GUILayout.BeginVertical();
            for(var r = rows-1; r >=0; r--)
            {

                GUILayout.BeginHorizontal();
                for(var c = 0; c < rows; c++)
                {
                    if (GUILayout.Button(_textureHolder[levelCreator.level[c+((rows)*r)]],GUILayout.Width(50),GUILayout.Height(50)))
                    {
                        levelCreator.level[c + ((rows) * r)] = _currentSelected;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            var count = 0;
            foreach(KeyValuePair<ElementTypes,Texture> e in _textureHolder)
            {
                count++;
                if (GUILayout.Button(e.Value, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    _currentSelected = e.Key;
                }

                if (count % 4 != 0) continue;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
