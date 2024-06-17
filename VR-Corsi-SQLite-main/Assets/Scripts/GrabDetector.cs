using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDetector : MonoBehaviour
{
    private GameHandler gameHandler;
    private GameObject cube;
    public Material glow;
    public Material red;

    void Awake()
    {
        gameHandler = GameObject.FindObjectOfType<GameHandler>();
    }
    public void GrabIn(int id)
    {
        if (gameHandler.showingCubes == false)
        {
            gameHandler.DetectGrabIn(id);
        }
    }

    public void GrabOut()
    {
        if (gameHandler.showingCubes == false)
        {
            //gameHandler.DetectGrabOut();
        }
    }

    public void NextGame()
    {
        gameHandler.NextGame();
    }

    public void HoverEnter(int id)
    {
        if (gameHandler.showingCubes == false)
        {
            cube = GameObject.Find($"Cube{id}");
            cube.GetComponent<MeshRenderer>().material = glow;
        }
    }

    public void HoverExit(int id)
    {
        if (gameHandler.showingCubes == false)
        {
            cube = GameObject.Find($"Cube{id}");
            cube.GetComponent<MeshRenderer>().material = red;
        }
    }
}
