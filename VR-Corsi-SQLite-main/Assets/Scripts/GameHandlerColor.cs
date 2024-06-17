using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandlerColor : MonoBehaviour
{
    public Material glow, cubeColor, red, yellow, brown, black, pink, purple, orange, blue, green, white;
    public int cubeNumber = 2;
    List<int> actualSequence;
    public int[] sequence;
    List<int> answears;
    List<bool> correctedAnswears;
    int activeScene;
    int peakCubeNumber;
    public int maxCubeNumber = 9;
    public GameObject nextButton;
    public bool showingCubes = false;
    private GameObject cube;
    bool playable = false;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public GameObject colorChanger;
    public Dictionary<int, Material> colors;
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

    IEnumerator ShowColorCoroutine()
    {
        //Debug.Log("coroutine started");
        nextButton.SetActive(false);
        showingCubes = true;

        answears = new List<int>();
        correctedAnswears = new List<bool>();

        actualSequence = new List<int>();

        colors = new Dictionary<int, Material>
        {
            {1, blue },
            {2, brown },
            {3, green },
            {4, orange },
            {5, pink },
            {6, red },
            {7, yellow },
            {8, purple },
            {9, black }
        };


        while (actualSequence.Count != cubeNumber)
        {
            int randomNumber = UnityEngine.Random.Range(1, 9);
            if (!actualSequence.Contains(randomNumber))
            {
                actualSequence.Add(randomNumber);
            }
        }

        /*
        for (int i = 1; i <= cubeNumber; i++)
        {
            actualSequence[i - 1] = sequence[i - 1];
        }
        */

        colorChanger.GetComponent<MeshRenderer>().material = white;
        colorChanger.SetActive(true);
        

        yield return new WaitForSeconds(2);
        /*
        Debug.Log(actualSequence.Length);
        Debug.Log(cubes.Count);
        
        for (int i = 0; i < 2; i++)
        {
            Debug.Log("actualSequence: " + actualSequence[i]);
            Debug.Log("cube: " + cubes[i].name);
        }
        */
        //colorChanger.GetComponent<MeshRenderer>().material = white;
        

        foreach (var id in actualSequence)
        {
            colorChanger.GetComponent<MeshRenderer>().material = colors[id];

            yield return new WaitForSeconds(PlayerPrefs.GetFloat("difficulty"));
        }

        showingCubes = false;
        playable = true;
        colorChanger.SetActive(false);
    }

    public void NextGame()
    {
        StartCoroutine(ShowColorCoroutine());
    }

    public void DetectGrabIn(int id)
    {
        if (playable)
        {
            if (answears.Count < cubeNumber)
            {
                answears.Add(id);
                audioSource.PlayOneShot(audioClip);
                Debug.Log("answear was added: " + id);
            }
            if (answears.Count == cubeNumber)
            {
                CheckAnswears();
            }
        }
    }

    public void DetectGrabOut()
    {
        if (playable)
        {
            if (answears.Count == cubeNumber)
            {
                CheckAnswears();
            }
        }
    }

    bool allCorrect = true;

    private void CheckAnswears()
    {
        Debug.Log("check answears was called");

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
                colorChanger.SetActive(false);
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
