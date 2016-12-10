using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class EnemyMove : MonoBehaviour
{

    public int speed = 1;
    public float distance;
    public Vector3 deltaMoveVector;
    private Vector3 currentMoveVector;
    private Vector3 startPosition,endPosition;

    // Use this for initialization
	void Start () {
	    currentMoveVector = deltaMoveVector;
	    startPosition = transform.position;
	    endPosition = startPosition + currentMoveVector;
	    distance = Vector3.Distance(startPosition, endPosition);
	}
	
	// Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        transform.Translate(currentMoveVector * Time.deltaTime * speed);
        currentPosition = transform.position;
        float currentDistance = (int)Vector3.Distance(currentPosition, endPosition);
            if (currentDistance == 0)
            {
                currentMoveVector = Vector3.Scale(currentMoveVector , new Vector3(-1f,-1f,-1f));
                endPosition = transform.position + currentMoveVector;
            }

    }
}
