using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects, CustomEditor(typeof(PlanetsSpawner))]
public class DecorationSpawnerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Spawn planets"))
		{
			((PlanetsSpawner)target).SpawnPlanets();
		}

		if (GUILayout.Button("Remove planets"))
		{
			((PlanetsSpawner)target).DeletePlanets();
		}
	}
}
