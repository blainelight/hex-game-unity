using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    [SerializeField]
    private int movementPoints = 20;
    public int MovementPoints { get => movementPoints; }

    [SerializeField]
    private float movementDuration = 1, rotationDuration = 0.3f;

    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();

    public event Action<Unit> MovementFinished;

    private void Awake()
    {
        glowHighlight = GetComponent<GlowHighlight>();
    }

    public void Deselect()
    {
        glowHighlight.ToggleGlow(false);
    }

    public void Select()
    {
        glowHighlight.ToggleGlow();
    }

    public void MoveThroughPath(List<Vector3> currentPath)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(RotateCoroutine(firstTarget, rotationDuration));
    }

    private IEnumerator RotateCoroutine(Vector3 endPosition, float rotationDuration)
    {
        Quaternion startRotation = transform.rotation;
        endPosition.y = transform.position.y;
        Vector3 direction = endPosition - transform.position;
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);

        if(Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false)
        {
            float timeElapsed = 0;
            while (timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration; //gives a value between 0 and 1
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                yield return null; //runs the code in every fram; pauses the coroutine until the next frame, allowing the object to move smoothly over time. So, yield is a powerful tool for writing asynchronous or time-spread code in Unity
            }
            transform.rotation = endRotation; // to ensure we are currently facing the direction 
        }
        StartCoroutine(MovementCoroutine(endPosition)); //animates player to move from 1 hex to another
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition)
    {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;

        while (timeElapsed < movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration; 
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpStep);
            yield return null; 
        }
        transform.position = endPosition; 

        if (pathPositions.Count > 0)
        {
            Debug.Log("Selecting the next position!");
            StartCoroutine(RotateCoroutine(pathPositions.Dequeue(), rotationDuration));
        }
        else
        {
            Debug.Log("Movement Finished!");
            MovementFinished?.Invoke(this);
        }
    }
}
