using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStopwatch : MonoBehaviour
{

    [SerializeField]
    Text timeDisplay;

    [SerializeField]
    GameObject player;

    private Vector3 initialPlayerPosition;
    private float elapsedTime;
    private bool isRunning;
    private bool hasRanBefore = false;

    private void Start()
    {
        ResetStopwatch();
        initialPlayerPosition = player.transform.position;
    }

    private void FixedUpdate()
    {

        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            timeDisplay.text = Math.Round((decimal)elapsedTime, 2) +"s";
        }else if(!hasRanBefore)
        {
            if(Vector3.Distance(initialPlayerPosition, player.transform.position) > 1)
            {
                StartStopwatch();
            }
        }
    }

    public void StartStopwatch()
    {
        hasRanBefore = true;
        isRunning = true;
    }

    public void StopStopwatch()
    {
        isRunning = false;
    }

    public void ResetStopwatch()
    {
        elapsedTime = 0f;
        isRunning = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
