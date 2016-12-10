using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class LevelGenerator : MonoBehaviour
{
    public GameObject simpleFloor, enemy, checkpoint, chunkDefaultLight, playerPref;
    public GameObject objectsGroup, levelGroup, envGroup;
    public string levelDirectoryPath;
    private List<GameObject> levelChunkObjects = new List<GameObject>();
    public Vector3 spawnPoint;
    private List<GameObject> levelChunkPrefabs = new List<GameObject>();
    private List<GameObject> sceneLevelChunkList = new List<GameObject>();
    private List<GameObject> sceneLevelGroups = new List<GameObject>();
    private GameObject mainLevelGroup;
    public GameObject LevelHierarchyPrefab;
    private float levelChunkWidth = 0f;
    private bool initLoad = true;
    private Vector3 chunkOffsetSpawnPoint = new Vector3(100, 0, 0);
    private Vector3 floorPrefabWidth;
    public int gameplayChunkCount = 5;
    private GameObject playerCubeObject;
    private List<float[]> chunksBoundries = new List<float[]>();
    private List<List<GameObject>> chunkGameObjectsList = new List<List<GameObject>>();
    public Vector3 defaultLightOffsetVector;
    private GameManager gameManager;
    public int levelWallHeght;

    public float lightYOffset = 5;

    private Dictionary<string, GameObject> chunkStructure =
        new Dictionary<string, GameObject>();

    public void loadChunkPrefabList()
    {
        levelChunkPrefabs.Clear();
        DirectoryInfo dir = new DirectoryInfo("Assets\\Resources\\");
        FileInfo[] info = dir.GetFiles("*.prefab");

        //Debug.Log("Currently saved prefab chunks:");

        for (int j = 0; j < info.Length; j++)
        {
            GameObject obj = null;
            obj = (GameObject) Resources.Load(info[j].Name.Substring(0, info[j].Name.Length - 7));
            //Debug.Log("obj " + obj.ToString());
            levelChunkPrefabs.Add(obj);
        }
    }

    public void generateSimpleFloor()
    {
        GameObject newFloor = Instantiate(simpleFloor, spawnPoint, Quaternion.identity) as GameObject;
        ;
        spawnPoint = spawnPoint + new Vector3(-simpleFloor.GetComponent<Renderer>().bounds.size.x, 0, 0);
        newFloor.transform.parent = envGroup.transform;
        levelChunkObjects.Add(newFloor);
        levelChunkWidth = simpleFloor.GetComponent<Renderer>().bounds.size.x;
    }

    public void generateDownFloor()
    {
        GameObject newFloor =
            Instantiate(simpleFloor, spawnPoint + new Vector3(0, -simpleFloor.GetComponent<Renderer>().bounds.size.x, 0),
                Quaternion.identity) as GameObject;
        GameObject newWall = Instantiate(simpleFloor,
            spawnPoint + new Vector3(simpleFloor.GetComponent<Renderer>().bounds.size.x / 2,
                -simpleFloor.GetComponent<Renderer>().bounds.size.x / 2, 0),
            Quaternion.Euler(0, 0, 90)) as GameObject;
        newFloor.transform.parent = envGroup.transform;
        newWall.transform.parent = envGroup.transform;
        newWall.tag = "downFloorWall";
        levelChunkObjects.Add(newFloor);
        levelChunkObjects.Add(newWall);

        levelChunkWidth = newFloor.GetComponent<Renderer>().bounds.size.x;


        spawnPoint = spawnPoint +
                     new Vector3(-simpleFloor.GetComponent<Renderer>().bounds.size.x,
                         -simpleFloor.GetComponent<Renderer>().bounds.size.x, 0);
    }

    public void spawnEnemy()
    {
        GameObject newEnemy =
            Instantiate(enemy, spawnPoint + new Vector3(simpleFloor.GetComponent<Renderer>().bounds.size.x, 3, 0),
                Quaternion.identity) as GameObject;
        newEnemy.transform.parent = objectsGroup.transform;
        newEnemy.tag = "enemy";
        levelChunkObjects.Add(newEnemy);
    }

    public void spawnCheckPoint()
    {
        GameObject newCheckPoint =
            Instantiate(checkpoint,
                spawnPoint + new Vector3(simpleFloor.GetComponent<Renderer>().bounds.size.x, 0.5f, 0),
                Quaternion.identity) as GameObject;
        newCheckPoint.transform.parent = envGroup.transform;
        levelChunkObjects.Add(newCheckPoint);
    }

    public void resetSpawnPoint()
    {
        spawnPoint = new Vector3(0, 0, 0);
    }

    public void loadLevelChunk(int LevelNumber, bool init)
    {
        Vector3 spawnOffset;
        if (LevelNumber > levelChunkPrefabs.Count)
        {
            //Debug.Log("No level: " + LevelNumber.ToString());
            return;
        }

        GameObject loadedLevelChunk =
            (GameObject) Instantiate(levelChunkPrefabs[LevelNumber],
                new Vector3(0f, 0f, 0f), Quaternion.identity);

        Vector3 loadedChunkBounds = calculateChunkBounds(loadedLevelChunk);

        chunksBoundries.Add(new float[2] {spawnPoint.x, spawnPoint.x + loadedChunkBounds.x});

        addLightToChunk(chunksBoundries[chunksBoundries.Count - 1]);

        //Debug.Log("loaded new chunk: " + loadedLevelChunk.name);
        int levelGroupCount = loadedLevelChunk.transform.childCount;

        //Debug.Log("levelGroupCount " + levelGroupCount.ToString());

        if (levelGroup == null)
        {
            resetAll();
        }

        if (initLoad)
        {
            //Debug.Log("spawnOffset zero");
            spawnOffset = new Vector3(0f, 0f, 0f);
        }
        else
        {
            //Debug.Log("spawnOffset spawn point + staring chunk offset");
            spawnOffset = new Vector3(spawnPoint.x - floorPrefabWidth.x, spawnPoint.y, 0f);
        }

        GameObject loadedObjectsGroup = loadedLevelChunk.transform.GetChild(0).gameObject;
        GameObject loadedEnvGroup = loadedLevelChunk.transform.GetChild(1).gameObject;
        chunkGameObjectsList.Add(new List<GameObject>());
        List<GameObject> currentlyAddedChunk = chunkGameObjectsList[chunkGameObjectsList.Count - 1];
        int loadedEnvGroupCount = loadedEnvGroup.transform.childCount;

        for (int j = loadedEnvGroupCount - 1; j > -1; j--)
        {
            GameObject chunkGroupObject = loadedEnvGroup.transform.GetChild(j).gameObject;
            chunkGroupObject.transform.parent = envGroup.transform;
            Quaternion chunkObjectRotation = chunkGroupObject.transform.rotation;
            chunkGroupObject.transform.rotation = Quaternion.identity;
            chunkGroupObject.transform.Translate(spawnOffset);
            chunkGroupObject.transform.rotation = chunkObjectRotation;
            currentlyAddedChunk.Add(chunkGroupObject);
        }
        int loadedObjectsGroupCount = loadedObjectsGroup.transform.childCount;

        for (int j = loadedObjectsGroupCount - 1; j > -1; j--)
        {
            GameObject chunkGroupObject = loadedObjectsGroup.transform.GetChild(j).gameObject;
            chunkGroupObject.transform.parent = objectsGroup.transform;
            Quaternion chunkObjectRotation = chunkGroupObject.transform.rotation;
            chunkGroupObject.transform.rotation = Quaternion.identity;
            chunkGroupObject.transform.Translate(spawnOffset);
            chunkGroupObject.transform.rotation = chunkObjectRotation;
            currentlyAddedChunk.Add(chunkGroupObject);
            Debug.Log("chunkGroupObject.tag " + chunkGroupObject.tag);
            if (chunkGroupObject.tag == "floor")
            {
                Debug.Log("FOUND floor obj!!!");
                addWallToFloor(chunkGroupObject);
            }
        }
        spawnPoint.x += loadedChunkBounds.x;

        if (!initLoad)
        {
            spawnPoint.x -= floorPrefabWidth.x;
        }
        if (!init && gameManager && gameManager.getChunkRevert())
        {
            gameManager.setRevertPosition(Convert.ToDecimal(spawnOffset.x));
        }

        spawnPoint.y += loadedChunkBounds.y;
        Object.DestroyImmediate(loadedLevelChunk);
        initLoad = false;
    }


    public void saveLevelChunk()
    {
        string newPrefabPath = "Assets/Resources/levelChunk" + levelChunkPrefabs.Count + ".prefab";
        UnityEditor.PrefabUtility.CreatePrefab(newPrefabPath, levelGroup);
        loadChunkPrefabList();
    }

    private void createLevelGroups()
    {
        levelGroup = (GameObject) Instantiate(LevelHierarchyPrefab, spawnPoint, Quaternion.identity);
        envGroup = levelGroup.transform.GetChild(0).gameObject;
        objectsGroup = levelGroup.transform.GetChild(1).gameObject;
    }

    private Vector3 calculateChunkBounds(GameObject newChunk)
    {
        // First find a center for your bounds.
        Vector3 center = Vector3.zero;
        //Debug.Log(string.Format("wiidth zero {0} {1} {2}", center.x.ToString(), center.y.ToString(), center.z.ToString()));
//        center /= transform.childCount; //center is average center of children

        foreach (Transform child in newChunk.transform)
        {
            foreach (Transform child2 in child.transform)
            {
                if (child2 != null)
                {
                    if (center.x > child2.transform.position.x)
                    {
                        center.x = child2.GetComponent<Renderer>().bounds.center.x;
                    }
                    if (center.y > child2.transform.position.y)
                    {
                        center.y = child2.GetComponent<Renderer>().bounds.center.y;
                    }
                }
            }
        }

        return center;
    }

    public void setChunkOffset()
    {
        GameObject tmpSimpleFloor = (GameObject) Instantiate(simpleFloor, chunkOffsetSpawnPoint, Quaternion.identity);
        floorPrefabWidth = new Vector3(tmpSimpleFloor.GetComponent<Renderer>().bounds.size.x, 0f, 0f);
        Object.DestroyImmediate(tmpSimpleFloor);
    }

    public void resetAll()
    {
        initLoad = true;
        //Debug.Log("reset all gameObjects ");
        foreach (GameObject obj in levelChunkObjects)
        {
            //Debug.Log("reset all levelChunkObjects ");
            Object.DestroyImmediate(obj);
        }
        foreach (GameObject obj in sceneLevelGroups)
        {
            //Debug.Log("reset all sceneLevelGroups ");
            Object.DestroyImmediate(obj);
        }
        sceneLevelGroups.Clear();
        levelChunkObjects.Clear();
        Object.DestroyImmediate(levelGroup);
        //TODO: delete player cube;
        resetSpawnPoint();

        createLevelGroups();
        setChunkOffset();
    }

    public GameObject spawnPlayer()
    {
        GameObject player = Instantiate(playerPref, spawnPoint, Quaternion.identity) as GameObject;
        player.transform.Translate(new Vector3(0f, player.GetComponent<Renderer>().bounds.size.y, 0f));
        return player;
    }

    public GameObject gameplayInit()
    {
        resetAll();
        loadChunkPrefabList();

        playerCubeObject = spawnPlayer();

        return playerCubeObject;
    }


    public void generateNextChunk(bool init)
    {
        int chunkIndex = UnityEngine.Random.Range(0, levelChunkPrefabs.Count);
        loadLevelChunk(chunkIndex, init);
    }

    public decimal getPlayerXPos()
    {
        float playerXFloat = playerCubeObject.transform.position.x;
        return Convert.ToDecimal(playerXFloat);
    }

    private int detectCurrentPlayerChunk()
    {
        decimal playerX = getPlayerXPos();

        for (int i = 0; i < chunksBoundries.Count; i++)
        {
            decimal chunkBoundStart, chunkBoundEnd;
            chunkBoundStart = Convert.ToDecimal(chunksBoundries[i][0]);
            chunkBoundEnd = Convert.ToDecimal(chunksBoundries[i][1]);
            if (chunkBoundStart >= playerX && playerX >= chunkBoundEnd)
            {
                return i;
            }
        }
        return -1;
    }

    public bool updateLevel()
    {
        int currentChunk = detectCurrentPlayerChunk();

        if (currentChunk == -1 && chunkGameObjectsList.Count == 0)
        {
            for (int i = 0; i < gameplayChunkCount; i++)
            {
                generateNextChunk(true);
            }
        }
        else
        {
            Debug.Log("chunkGameObjectsList " + chunkGameObjectsList.Count);

            int updateChunkIndex = Convert.ToInt32(chunkGameObjectsList.Count * 0.5);

            Debug.Log("udpate chunk index " + updateChunkIndex + " current chunk " + currentChunk);
            if (currentChunk == updateChunkIndex)
            {
                removeChunk(0);
                addChunk();
            }
        }
        return false;
    }


    private void removeChunk(int chunkIndex)
    {
        List<GameObject> chunkToTrash = chunkGameObjectsList[chunkIndex];
        for (int i = 0; i < chunkToTrash.Count; i++)
        {
            Debug.Log("removing index " + i);

            GameObject gameObjectToRemove = chunkToTrash[i];

            Object.DestroyImmediate(gameObjectToRemove);
        }
        Debug.Log("chunkGameObjectsList count before2 " + chunkGameObjectsList.Count + " chunkIndex " + chunkIndex);
        chunkGameObjectsList.RemoveAt(chunkIndex);
        chunksBoundries.RemoveAt(chunkIndex);
        Debug.Log("chunkGameObjectsList count after2 " + chunkGameObjectsList.Count);
//        EditorApplication.isPaused = true;
    }

    private void addWallToFloor(GameObject floorGameObject)
    {
        //
//        floorGameObject
        float floorTileWidth = floorPrefabWidth.x;
        float halfFloorTileWidth = Convert.ToSingle(floorPrefabWidth.x * 0.5);
        Vector3 baseVector = floorGameObject.transform.position;
        Vector3 backWallVector = new Vector3(0f, halfFloorTileWidth, halfFloorTileWidth);
        Vector3 frontWallVector = new Vector3(0f, -halfFloorTileWidth, -halfFloorTileWidth);
        Quaternion wallRotationQ = Quaternion.Euler(-90f, 0f, 0f);

        for (int i = 0; i < levelWallHeght; i++)
        {
            GameObject backWall =
                Instantiate(simpleFloor, baseVector + backWallVector, wallRotationQ) as GameObject;
            GameObject frontWall =
                Instantiate(simpleFloor, baseVector + frontWallVector, wallRotationQ) as GameObject;
            backWall.transform.parent = floorGameObject.transform.parent;
            frontWall.transform.parent = floorGameObject.transform.parent;
            backWallVector.y += floorTileWidth;
            frontWallVector.y -= floorTileWidth;
        }
    }

    private void addChunk()
    {
        generateNextChunk(false);
    }

    void Start()
    {
        gameManager = GetComponent(typeof(GameManager)) as GameManager;
    }

    public void addLightToChunk(float[] chunkBounds)
    {
        //TODO: think about nice light for every chunk (3 directional lights rotated differently)
        //TODO: maybe some special color light for each chunk with dif color
//        Vector3 lightVectorSpawn = new Vector3(chunkBounds[0], spawnPoint.y, 0f);
//        GameObject newChunkLight = Instantiate(chunkDefaultLight, lightVectorSpawn, Quaternion.Euler(60, 0, 0)) as GameObject;
//        transform.parent = envGroup.transform;
//        newChunkLight.transform.Translate(defaultLightOffsetVector);
    }

    public void removeAllChunkFiles()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets\\Resources\\");
        FileInfo[] info = dir.GetFiles("*.prefab");

        Debug.Log("Currently saved prefab chunks:");

        for (int j = 0; j < info.Length; j++)
        {
            Debug.Log("Removing chunk file prefab: " + info[j].FullName);
            FileUtil.DeleteFileOrDirectory(info[j].FullName);
        }
    }
}