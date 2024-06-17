using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDetectorColor : MonoBehaviour
{
    private GameHandlerColor gameHandlerColor;
    private GameObject cube;

    void Awake()
    {
        gameHandlerColor = GameObject.FindObjectOfType<GameHandlerColor>();
    }
    public void GrabIn(int id)
    {
        if (gameHandlerColor.showingCubes == false)
        {
            gameHandlerColor.DetectGrabIn(id);
        }
    }

    public void GrabOut()
    {
        if (gameHandlerColor.showingCubes == false)
        {
            gameHandlerColor.DetectGrabOut();
        }
    }

    public void NextGame()
    {
        gameHandlerColor.NextGame();
    }

}
