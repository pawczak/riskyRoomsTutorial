using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;

    // Use this for initialization
    public void setGameManager(GameManager gameManagerInstance)
    {
        gameManager = gameManagerInstance;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "enemy")
        {
            gameManager.restart();
        }
    }
}