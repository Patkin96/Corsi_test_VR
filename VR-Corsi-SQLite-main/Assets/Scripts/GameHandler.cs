using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public Material glow, cubeColor;
    public int cubeNumber = 2;
    List<int> actualSequence;
    public int[] sequence;
    List<int> answears;
    //List<bool> correctedAnswears;
    int activeScene;
    int peakCubeNumber;
    public int maxCubeNumber = 9;
    public  GameObject nextButton;
    public bool showingCubes = false;
    private GameObject cube;
    bool playable = false;
    public AudioSource audioSource;
    public AudioClip audioClip;
    // Start is called before the first frame update
    void Start()
    {
        activeScene = SceneManager.GetActiveScene().buildIndex;

        Debug.Log("Active Scene Number: " + activeScene);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed");
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator ShowCubesCoroutine()
    {
        //Debug.Log("coroutine started");
        nextButton.SetActive(false);
        showingCubes = true;

        answears = new List<int>();
        //correctedAnswears = new List<bool>();

        actualSequence = new List<int>();

        
        while (actualSequence.Count != cubeNumber)
        {
            int randomNumber = UnityEngine.Random.Range(1, 9);
            if (!actualSequence.Contains(randomNumber))
            {
                actualSequence.Add(randomNumber);
            }
        }

        yield return new WaitForSeconds(2);

        foreach (var id in actualSequence)
        {
            cube = GameObject.Find($"Cube{id}");
            cube.GetComponent<MeshRenderer>().material = glow;
            //Debug.Log("glow");
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("difficulty"));
            cube.GetComponent<MeshRenderer>().material = cubeColor;
            //Debug.Log("red");
        }

        showingCubes = false;
        playable = true;
    }

    public void NextGame()
    {
        StartCoroutine(ShowCubesCoroutine());
    }

    public void DetectGrabIn(int id)
    {
        if (playable)
        {
            if (answears.Count < cubeNumber)
            {
                Debug.Log("answear count before add: " + answears.Count);
                answears.Add(id);
                audioSource.PlayOneShot(audioClip);
                Debug.Log("answear was added: " + id);
            }
            Debug.Log("answear count after add: " + answears.Count);
            if (answears.Count == cubeNumber)
            {
                CheckAnswears();
            }
        }
    }

    public void HoverEnter(int id)
    {
        if (showingCubes == false)
        {
            cube = GameObject.Find($"Cube{id}");
            cube.GetComponent<MeshRenderer>().material = glow;
        }
    }

    public void HoverExit(int id)
    {
        if (showingCubes == false)
        {
            cube = GameObject.Find($"Cube{id}");
            cube.GetComponent<MeshRenderer>().material = cubeColor;
        }
    }

    bool allCorrect = true;

    private void CheckAnswears()
    {
        Debug.Log("check answears was called");

        Debug.Log("reversed: " + PlayerPrefs.GetInt("reversed"));
        if (PlayerPrefs.GetInt("reversed") == 0)
        {
            for (int i = 0; i < cubeNumber; i++)
            {
                if (answears[i] != actualSequence[i])
                {
                    allCorrect = false;
                    break;
                }
            }

        }
        else if (PlayerPrefs.GetInt("reversed") == 1)
        {
            actualSequence.Reverse();
            List<int> reversedSequence = actualSequence;
                

            for (int i = 0; i < cubeNumber; i++)
            {
                if (answears[i] != reversedSequence[i])
                {
                    allCorrect = false;
                    break;
                }
            }
        }


        if (allCorrect)
        {
            if (cubeNumber < maxCubeNumber)
            {
                cubeNumber++;
                //StartCoroutine(ShowCubesCoroutine());
                nextButton.SetActive(true);
                playable = false;
            }
            else
            {
                //end of the game --> set playerprefs --> load mainmenu scene
                peakCubeNumber = maxCubeNumber;
                Debug.Log("Peak cube number: " + peakCubeNumber);
                PlayerPrefs.SetInt("span", peakCubeNumber);
                PlayerPrefs.SetInt("save", 1);
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            //end of the game --> set playerprefs --> load mainmenu scene
            if (cubeNumber == 2)
            {
                peakCubeNumber = 0;
            }
            else
            {
                peakCubeNumber = cubeNumber - 1;
            } 
            Debug.Log("Peak cube number: " + peakCubeNumber);
            PlayerPrefs.SetInt("span", peakCubeNumber);
            PlayerPrefs.SetInt("save", 1);
            SceneManager.LoadScene(0);
        }
    }
}
