using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelDesignerInterface : Editor
{
    private int loadLevelNumber;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelGenerator LevelGeneratorScript = (LevelGenerator) target;

        if (GUILayout.Button("simpleFloor"))
        {
            LevelGeneratorScript.generateSimpleFloor();
        }
        if (GUILayout.Button("downFloor"))
        {
            LevelGeneratorScript.generateDownFloor();
        }
        GUILayout.Space(25);
        if (GUILayout.Button("enemy"))
        {
            LevelGeneratorScript.spawnEnemy();
        }
        GUILayout.Space(25);
        if (GUILayout.Button("checkpoint"))
        {
            LevelGeneratorScript.spawnCheckPoint();
        }

        GUILayout.Space(100);
        if (GUILayout.Button("reset spawn point"))
        {
            LevelGeneratorScript.resetSpawnPoint();
        }
        if (GUILayout.Button("reset all"))
        {
            LevelGeneratorScript.resetAll();
        }
        if (GUILayout.Button("save levelChunk"))
        {
            LevelGeneratorScript.saveLevelChunk();
        }
        loadLevelNumber = EditorGUILayout.IntField("chunk number to load:", loadLevelNumber);
        if (GUILayout.Button("load level chunk"))
        {
            LevelGeneratorScript.loadLevelChunk(loadLevelNumber, false);
        }
        if (GUILayout.Button("load chunk prefabs list"))
        {
            LevelGeneratorScript.loadChunkPrefabList();
        }
        GUILayout.Space(25);
        if (GUILayout.Button("REMOVE LEVEL CHUNK FILES"))
        {
            LevelGeneratorScript.removeAllChunkFiles();
        }
    }
}