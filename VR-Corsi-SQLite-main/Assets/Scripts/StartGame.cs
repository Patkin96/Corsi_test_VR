using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class StartGame : MonoBehaviour
{
    //public InputField username;
    //public InputField difficulty;
    public TMP_InputField username;
    public TMP_InputField difficulty;
    public Button corsiTestButton;
    public Text messageBoxMessage;
    public GameObject messageBox;
    public Text messageBoxTitle;
    public Dropdown gender;
    public Dropdown reversed;
    public Button corsiTestColorButton;

    readonly string url = "http://localhost/sqlconnect/VR-Corsi-Server/checkUserNew.php";
    string dbName;
    readonly int corsiTestBoy = 3;
    readonly int corsiTestGirl = 4;
    readonly int corsiColorTestBoy = 6;
    readonly int corsiColorTestGirl = 7;
    //readonly int corsiRTest = 4;

    private void Start()
    {
        dbName = "URI=file:" + Application.dataPath + "/Database.db";
    }

    public void CorsiTest()
    {
        float diff = float.Parse(difficulty.text);
        PlayerPrefs.SetFloat("difficulty", diff);
        string usernameString = username.text;
        PlayerPrefs.SetString("username", usernameString);
        string reversedString = reversed.options[reversed.value].text;
        if (reversedString == "nem")
        {
            PlayerPrefs.SetInt("reversed", 0);
            PlayerPrefs.SetInt("test", 1);
        }
        else
        {
            PlayerPrefs.SetInt("reversed", 1);
            PlayerPrefs.SetInt("test", 2);
        }
        
        string genderString = gender.options[gender.value].text;
        if (genderString == "fiú")
        {
            //StartCoroutine(StartTest(corsiTestBoy, usernameString));
            StartGameSQLite(corsiTestBoy, usernameString);
        }
        else
        {
            //StartCoroutine(StartTest(corsiTestGirl, usernameString));
            StartGameSQLite(corsiTestGirl, usernameString);
        }
        
    }

    public void CorsiColorTest()
    {
        float diff = float.Parse(difficulty.text);
        PlayerPrefs.SetFloat("difficulty", diff);
        string usernameString = username.text;
        PlayerPrefs.SetString("username", usernameString);
        string reversedString = reversed.options[reversed.value].text;
        if (reversedString == "nem")
        {
            PlayerPrefs.SetInt("reversed", 0);
            PlayerPrefs.SetInt("test", 3);
        }
        else
        {
            PlayerPrefs.SetInt("reversed", 1);
            PlayerPrefs.SetInt("test", 4);
        }

        string genderString = gender.options[gender.value].text;
        if (genderString == "fiú")
        {
            //StartCoroutine(StartTest(corsiColorTestBoy, usernameString));
            StartGameSQLite(corsiColorTestBoy, usernameString);
        }
        else
        {
            //StartCoroutine(StartTest(corsiColorTestGirl, usernameString));
            StartGameSQLite(corsiColorTestGirl, usernameString);
        }

    }

    IEnumerator StartTest(int test, string usernameString)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        form.Add(new MultipartFormDataSection("username", usernameString));

        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            messageBoxTitle.text = "Hálózati hiba!";
            messageBoxMessage.text = "Nem lehet elérni a szervert. Ellenõrizze az internet kapcsolatot!";
            messageBox.SetActive(true);
        }

        Debug.Log("ErrorCode: " + webRequest.downloadHandler.text);

        if (webRequest.downloadHandler.text == "0")
        {
            SceneManager.LoadScene(test);
        }
        else if (webRequest.downloadHandler.text == "2")
        {
            messageBoxTitle.text = "Adatbázis hiba!";
            messageBoxMessage.text = "Sikertelen felhasználó ellenõrzés!";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "1")
        {
            messageBoxTitle.text = "Szerver hiba!";
            messageBoxMessage.text = "A szerver nem tud csatlakozni az adatbázishoz.";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "3")
        {
            messageBoxTitle.text = "Hiba!";
            messageBoxMessage.text = "Nincs ilyen nevû felhasználó az adatbázisba!";
            messageBox.SetActive(true);
        }
    }

    public void StartGameSQLite(int test, string usernameString)
    {
        using (var conn = new SqliteConnection(dbName))
        {
            conn.Open();
            int RowCount = -1;
            using (var command = conn.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM users WHERE username = '{usernameString}'";
                command.CommandType = CommandType.Text;


                RowCount = Convert.ToInt32(command.ExecuteScalar());
            }

            Debug.Log("Rows: " + RowCount);

            if (RowCount == 0)
            {
                messageBoxTitle.text = "Hiba!";
                messageBoxMessage.text = "Nem létezik ilyen nevû felhasználó!";
                messageBox.SetActive(true);
                conn.Close();
                return;
            }

            conn.Close();
            Debug.Log("database closed");

            SceneManager.LoadScene(test);
        }
    }

    public void OK()
    {
        messageBox.SetActive(false);
    }

    public void VerifyInputs()
    {
        bool usernameIsValid = (username.text.Length >= 5 && username.text.Length <= 16);
        bool difficultyIsValid = difficulty.text != "";

        if (usernameIsValid && difficultyIsValid)
        {
            corsiTestButton.interactable = true;
            corsiTestColorButton.interactable = true;
        }
        else
        {
            corsiTestButton.interactable = false;
            corsiTestColorButton.interactable = false;

        }
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
