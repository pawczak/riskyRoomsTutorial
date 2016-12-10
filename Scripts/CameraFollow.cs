using UnityEngine;
using System.Collections;


public class CameraFollow : MonoBehaviour
{
    public GameObject Player;
    [SerializeField] float smoothing = 1f; //Amount of smoothing to apply to the cameras movement

    [SerializeField] Vector3 offset = new Vector3(0f, 25f, -29f);
    //The offset of the camera from the player (how far back and above the player the camera should be)

    void FixedUpdate()
    {


        //Use the player's position and offset to determine where the camera should be
        Vector3 targetCamPos = Player.transform.position + offset;
        //Smoothly move from the current position to the desired position using a Lerp, which is short
        //for linear interpolation. Basically, it takes where you are, where you want to be, and an amount of time
        //and then tells you where you will be along that line
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }

    public void setPlayer(GameObject playerGameObject)
    {
        Player = playerGameObject;
    }

    public void transferCamera()
    {
        //TODO:code function
    }
}