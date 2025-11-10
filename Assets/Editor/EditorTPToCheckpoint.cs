using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace AG3958
{
    public class EditorTPToCheckpoint : EditorWindow
    {
        private PlayerCore _playerCore;

        [MenuItem("Tools/Jaakko/TP to Checkpoint")]
        public static void Init()
        {
            EditorWindow window = GetWindow<EditorTPToCheckpoint>();
            window.position = new Rect(50f,50f,200f,50f);
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("TP to Checkpoint");
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            _playerCore = FindFirstObjectByType<PlayerCore>();
            if (GUILayout.Button(text: "Set Target Checkpoint"))
            {
                GenericMenu menu = new GenericMenu();
                CheckpointSystem cs = FindFirstObjectByType<CheckpointSystem>();

                foreach (Checkpoint checkpoint in cs.CheckpointList)
                {
                    AddCheckpointMenuItem(menu, checkpoint.CheckpointID, _playerCore, checkpoint);
                }
                menu.ShowAsContext();
            }
            if (GUILayout.Button(text: "Teleport To Target Checkpoint"))
            {
                _playerCore.transform.position = _playerCore.PreviousCheckpoint.CheckpointTarget;
            }
        }

        private void AddCheckpointMenuItem(GenericMenu menu, string menuPath, PlayerCore playerCore, Checkpoint point)
        {
            menu.AddItem(new GUIContent(menuPath), playerCore.PreviousCheckpoint.Equals(point), OnCheckpointSelected, point);
        }

        private void OnCheckpointSelected(object point)
        {
            _playerCore.SetCheckpoint((Checkpoint)point);
        }
    } 
}

#endif