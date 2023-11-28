using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LogicGame
{
    [CustomEditor(typeof(LevelCreator))]
    public class NewLevelCreatorInspector : Editor
    {
        Dictionary<ElementTypes, Texture> _textureHolder = new Dictionary<ElementTypes, Texture>();
        private SerializedProperty _levelProperty;
        private ElementTypes _currentSelected = ElementTypes.Empty;

        private void OnEnable()
        {
            _levelProperty = serializedObject.FindProperty("level");

            _textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png"));
            _textureHolder.Add(ElementTypes.Player, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Player.png"));
            _textureHolder.Add(ElementTypes.Wall, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/Wall.png"));
            _textureHolder.Add(ElementTypes.WallWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/WallWord.png"));
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
            _textureHolder.Add(ElementTypes.PlayerWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/PlayerWord.png"));
            _textureHolder.Add(ElementTypes.IsWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/IsWord.png"));
            _textureHolder.Add(ElementTypes.YouWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/YouWord.png"));


        }

        private void OnDisable()
        {
            // Clean up any necessary resources or event listeners here
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            base.OnInspectorGUI();

            GUILayout.Label("Current Selected: " + _currentSelected.ToString());

            var levelCreator = (LevelCreator)target;
            var rows = (int)Mathf.Sqrt(levelCreator.level.Count);

            GUILayout.BeginVertical();
            int r;
            for (r = rows - 1; r >= 0; r--)
            {
                // Inside the nested loops for generating buttons
                for (r = rows - 1; r >= 0; r--)
                {
                    GUILayout.BeginHorizontal();
                    for (var c = 0; c < rows; c++)
                    {
                        var index = c + (rows * r);
                        var enumProperty = _levelProperty.GetArrayElementAtIndex(index);
                        var currentEnumValue = (ElementTypes)enumProperty.enumValueIndex;
                        var texture = _textureHolder[currentEnumValue];

                        if (GUILayout.Button(texture, GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            // Handle setting enum value when button is clicked
                            enumProperty.enumValueIndex = (int)_currentSelected;
                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    GUILayout.EndHorizontal();
                }

            }
            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            var count = 0;
            var keys = new List<ElementTypes>(_textureHolder.Keys);
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var texture = _textureHolder[key];

                count++;
                if (GUILayout.Button(texture, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    _currentSelected = key;
                }

                if (count % 4 == 0 || i == keys.Count - 1)
                {
                    GUILayout.EndHorizontal();
                    if (i != keys.Count - 1)
                    {
                        GUILayout.BeginHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();
            
          
        }
    }
}
