using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LogicGame
{
    [CustomEditor(typeof(LevelCreator))] 
    public class LevelCreatorInspector : Editor
    {
        Dictionary<ElementTypes, Texture> textureHolder = new Dictionary<ElementTypes, Texture>();
        private void OnEnable()
        {
            //for editor images when done
            //template: textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png"));
            textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png"));
            textureHolder.Add(ElementTypes.IsWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/IsWord.png"));
            textureHolder.Add(ElementTypes.Player, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Player.png"));
            textureHolder.Add(ElementTypes.Wall, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Wall.png"));
            textureHolder.Add(ElementTypes.PlayerWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/PlayerWord.png"));
            textureHolder.Add(ElementTypes.WallWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/WallWord.png"));
            textureHolder.Add(ElementTypes.YouWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/YouWord.png"));
            textureHolder.Add(ElementTypes.PushWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/PushWord.png"));
            textureHolder.Add(ElementTypes.WinWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/WinWord.png"));
            textureHolder.Add(ElementTypes.StopWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/StopWord.png"));
            textureHolder.Add(ElementTypes.Flag, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Flag.png"));
            textureHolder.Add(ElementTypes.FlagWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/FlagWord.png"));
            textureHolder.Add(ElementTypes.Rock, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Rock.png"));
            textureHolder.Add(ElementTypes.RockWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/RockWord.png"));
            textureHolder.Add(ElementTypes.Hazard, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Hazard.png"));
            textureHolder.Add(ElementTypes.HazardWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/HazardWord.png"));
            textureHolder.Add(ElementTypes.SinkWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/SinkWord.png"));
            

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
                    if (GUILayout.Button(textureHolder[levelCreator.level[c+((rows)*r)]],GUILayout.Width(50),GUILayout.Height(50)))
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
            foreach(KeyValuePair<ElementTypes,Texture> e in textureHolder)
            {
                count++;
                if (GUILayout.Button(e.Value, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    _currentSelected = e.Key;
                }
                if (count % 4 == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }
            GUILayout.EndVertical();
        }
    }
}
