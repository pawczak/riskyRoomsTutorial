using System.CodeDom.Compiler;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    public Rigidbody playerCube;
    public float playerMovementSpeed;
    public Vector3 playerMovementVector;
    private bool touchEnabled = false;
    private LevelGenerator generator;
    public GameObject mainCamera;

    private decimal revertPosition;
    [Range(0, 1)] public float revertChunkChance;
    private decimal revertX;
    private bool lockRevert = true;

    private bool gameOver = false;

    // Use this for initialization

    void Start()
    {
        generator = GetComponent(typeof(LevelGenerator)) as LevelGenerator;
        GameObject playerGameObject = generator.gameplayInit();
        playerCube = playerGameObject.GetComponent<Rigidbody>();
        PlayerCollision playerCollisionScript = playerCube.GetComponent<PlayerCollision>() as PlayerCollision;
        playerCollisionScript.setGameManager(this);
        mainCamera.GetComponent<CameraFollow>().setPlayer(playerGameObject);
        mainCamera.GetComponent<CameraFollow>().enabled = true;
        StartCoroutine(startRound(2));
    }

    IEnumerator startRound(float time)
    {
        Debug.Log("Wait...");
        yield return new WaitForSeconds(time);
        // Code to execute after the delay
        Debug.Log("Enabled");
        touchEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") && touchEnabled)
        {
            playerCube.AddForce(playerMovementVector * Time.deltaTime * playerMovementSpeed);
        }
    }

    public void setRevertPosition(decimal newRevertX)
    {
        if (!lockRevert)
        {
            return;
        }
        revertX = newRevertX;
        lockRevert = false;
    }

    void FixedUpdate()
    {
        if (gameOver)
        {
            return;
        }
        generator.updateLevel();


        if (!lockRevert)
        {
            decimal playerX = generator.getPlayerXPos();
//            Debug.Log("checking reverX " + revertX + " playerX " + playerX);
            if (playerX < revertX)
            {
                revertGameplay();
                lockRevert = true;
            }
        }
    }

    public void revertGameplay()
    {
        Debug.Log("revert gameplay!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        //TODO: code revert funcionality
        lockRevert = false;
    }

    public bool getChunkRevert()
    {
        return UnityEngine.Random.Range(0.0f, 1.0f) <= revertChunkChance;
    }

    public void restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}