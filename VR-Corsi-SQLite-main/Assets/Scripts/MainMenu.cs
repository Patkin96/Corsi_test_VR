using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    int save;
    readonly string url = "http://localhost/sqlconnect/VR-Corsi-Server/insertResultNew.php";
    public Text messageBoxMessage;
    public GameObject messageBox;
    public Text messageBoxTitle;
    string dbName;
    private void Start()
    {
        save = PlayerPrefs.GetInt("save");
        dbName = "URI=file:" + Application.dataPath + "/Database.db";
        Debug.Log("Save: " + save);
        if (save == 1)
        {
            Debug.Log("Save run!");
            PlayerPrefs.SetInt("save", 0);
            string username = PlayerPrefs.GetString("username");
            string span = PlayerPrefs.GetInt("span").ToString();
            string difficulty = PlayerPrefs.GetFloat("difficulty").ToString();
            string test = PlayerPrefs.GetInt("test").ToString();
            //StartCoroutine(SaveResult(username, span, difficulty, test));
            SaveResultsSQLite(username, span, difficulty, test);
        }
        
        
        CreateDataBase();

    }

    public void CreateDataBase()
    {
        if (true)
        {
            PlayerPrefs.SetInt("isDataBaseCreated", 1);

            using (var conn = new SqliteConnection(dbName))
            {
                conn.Open();
                //string sql = "CREATE TABLE IF NOT EXISTS users ( username TEXT NOT NULL, gender TEXT NOT NULL, date NUMERIC NOT NULL, hand TEXT NOT NULL, glasses TEXT NOT NULL, sickness TEXT NOT NULL, PRIMARY KEY('username') )";
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS users ( username TEXT NOT NULL, gender TEXT NOT NULL, date TEXT NOT NULL, hand TEXT NOT NULL, glasses TEXT NOT NULL, sickness TEXT DEFAULT 'nincs' NOT NULL, PRIMARY KEY('username') )";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE IF NOT EXISTS results ( id INTEGER NOT NULL, username TEXT NOT NULL, span INTEGER NOT NULL, difficulty REAL NOT NULL, test INTEGER NOT NULL, time TEXT NOT NULL, PRIMARY KEY(id AUTOINCREMENT) , FOREIGN KEY(username) REFERENCES users(username) )";
                    command.ExecuteNonQuery();

                }
                //sql = "CREATE TABLE IF NOT EXISTS results ( id INTEGER NOT NULL, username TEXT NOT NULL, span INTEGER NOT NULL, difficulty REAL NOT NULL, test INTEGER NOT NULL, time NUMERIC NOT NULL, PRIMARY KEY(id AUTOINCREMENT) , FOREIGN KEY(username) REFERENCES users(username) )";
                //using (var command = new SqliteCommand(sql, conn))
                //{
                //    command.ExecuteNonQuery();

                //}
            conn.Close();
            }
        }
    }

    public void SaveResultsSQLite(string username, string span, string difficulty, string test)
    {
        using (var conn = new SqliteConnection(dbName))
        {
            conn.Open();

            {

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO results (username, span, difficulty, test, time) VALUES ('{username}', '{span}', '{difficulty}', '{test}', DATE())";

                    command.ExecuteNonQuery();
                }

                messageBoxTitle.text = "Sikeres mentés!";
                messageBoxMessage.text = "Az adatok beszúrásra kerültek az adatbázisba.";
                messageBox.SetActive(true);

            }


            conn.Close();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void NewUser()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ModifyUser()
    {
        SceneManager.LoadScene(5);
    }

    IEnumerator SaveResult(string username, string span, string difficulty, string test)
    {
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        string diffFormed = difficulty.Replace(",", ".");

        Debug.Log("Diff form: " + diffFormed);

        form.Add(new MultipartFormDataSection("username", username));
        form.Add(new MultipartFormDataSection("span", span));
        form.Add(new MultipartFormDataSection("difficulty", diffFormed));
        form.Add(new MultipartFormDataSection("test", test));

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
            messageBoxTitle.text = "Sikeres mentés!";
            messageBoxMessage.text = "Az adatok beszúrásra kerültek az adatbázisba.";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "1")
        {
            messageBoxTitle.text = "Szerver hiba!";
            messageBoxMessage.text = "A szerver nem tud csatlakozni az adatbázishoz.";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "2")
        {
            messageBoxTitle.text = "Adatbázis hiba!";
            messageBoxMessage.text = "Nem sikerüt beszúrni az adatokat.";
            messageBox.SetActive(true);
        }

    }
    public void OK()
    {
        messageBox.SetActive(false);
    }
}
