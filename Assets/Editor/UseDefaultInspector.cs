using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace AG3958
{
	/// <summary>
	/// Use the default IMGUI Inspector for serialized types that are broken in EditorAttributes
	/// </summary>
	[CustomEditor(typeof(CheckpointSystem))]
	public class UseDefaultInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			EditorGUILayout.LabelField("Using Default IMGUI");
		}
	}

}

#endif