using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Data;
using Mono.Data.Sqlite;

public class ModifyUser : MonoBehaviour
{
    //public InputField username;
    //public InputField date;
    public TMP_InputField username;
    public TMP_InputField date;
    public Dropdown gender;
    public Dropdown hand;
    public Dropdown glasses;
    //public InputField sickness;
    public TMP_InputField sickness;
    public Button getUserButton;
    public Button modifyUserButton;
    public Text messageBoxMessage;
    public GameObject messageBox;
    public Text messageBoxTitle;
    User user;
    string usernameValid;
    bool userEmpty = true;

    readonly string getUserURL = "http://localhost/sqlconnect/VR-Corsi-Server/getUser.php";
    readonly string modifyUserURL = "http://localhost/sqlconnect/VR-Corsi-Server/modifyUser.php";
    string dbName;

    private void Start()
    {
        dbName = "URI=file:" + Application.dataPath + "/Database.db";
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }


    public void GetUser()
    {
        string usernameString = username.text;

        //StartCoroutine(GetUserCoroutine(usernameString));
        GetUserSQLite(usernameString);
    }

    IEnumerator GetUserCoroutine(string usernameString)
    {

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();


        form.Add(new MultipartFormDataSection("username", usernameString));


        UnityWebRequest webRequest = UnityWebRequest.Post(getUserURL, form);
        yield return webRequest.SendWebRequest();


        if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            messageBoxTitle.text = "Hálózati hiba!";
            messageBoxMessage.text = "Nem lehet elérni a szervert. Ellenõrizze az internet kapcsolatot!";
            messageBox.SetActive(true);
        }

        Debug.Log("Result: " + webRequest.downloadHandler.text);


        if (webRequest.downloadHandler.text == "3")
        {
            messageBoxTitle.text = "Hiba!";
            messageBoxMessage.text = "Nem létezik ilyen nevû felhasználó!";
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
            messageBoxMessage.text = "Sikertelen felhasználó ellenõrzés!";
            messageBox.SetActive(true);
        }
        else
        {
            user = JsonUtility.FromJson<User>(webRequest.downloadHandler.text);

            userEmpty = false;
            modifyUserButton.interactable = true;

            date.text = user.date.Replace("-",".");

            if (user.gender == "fiú")
            {
                gender.value = 0;
            }
            else
            {
                gender.value = 1;
            }

            if (user.hand == "jobb")
            {
                hand.value = 0;
            }
            else
            {
                hand.value = 1;
            }

            if (user.glasses == "nem")
            {
                glasses.value = 0;
            }
            else
            {
                glasses.value = 1;
            }

            if (user.sickness == "nincs")
            {
                sickness.text = "";
            }
            else
            {
                sickness.text = user.sickness;
            }

            date.interactable = true;
            gender.interactable = true;
            hand.interactable = true;
            glasses.interactable = true;
            sickness.interactable = true;
        }


    }

    public void GetUserSQLite(string usernameString)
    {
        usernameValid = usernameString;
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

            using (var command = conn.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM users WHERE username = '{usernameString}'";
                
                using (IDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    userEmpty = false;
                    modifyUserButton.interactable = true;

                    date.text = reader["date"].ToString();

                    if (reader["gender"].ToString() == "fiú")
                    {
                        gender.value = 0;
                    }
                    else
                    {
                        gender.value = 1;
                    }

                    if (reader["hand"].ToString() == "jobb")
                    {
                        hand.value = 0;
                    }
                    else if (reader["hand"].ToString() == "bal")
                    {
                        hand.value = 1;
                    } else
                    {
                        hand.value = 2;
                    }

                    if (reader["glasses"].ToString() == "nem")
                    {
                        glasses.value = 0;
                    }
                    else
                    {
                        glasses.value = 1;
                    }

                    if (reader["sickness"].ToString() == "nincs")
                    {
                        sickness.text = "";
                    }
                    else
                    {
                        sickness.text = reader["sickness"].ToString();
                    }

                    date.interactable = true;
                    gender.interactable = true;
                    hand.interactable = true;
                    glasses.interactable = true;
                    sickness.interactable = true;
                }
            }

            conn.Close();
        }
    }

    public void SaveUser()
    {
        string usernameString = username.text;
        string dateString = date.text;
        string genderString = gender.options[gender.value].text;
        string handString = hand.options[hand.value].text;
        string glassesString = glasses.options[glasses.value].text;
        string sicknessString = sickness.text;

        Debug.Log("Sickness: " + sickness.text.Length);

        //StartCoroutine(SaveUserCoroutine(usernameString, dateString, genderString, handString, glassesString, sicknessString));
        SaveUserSQLite(usernameString, dateString, genderString, handString, glassesString, sicknessString);
    }

    IEnumerator SaveUserCoroutine(string usernameString, string dateString, string genderString, string handString, string glassesString, string sicknessString)
    {

        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        if (sicknessString.Length == 0)
        {
            sicknessString = "-";
        }

        form.Add(new MultipartFormDataSection("username", usernameString));
        form.Add(new MultipartFormDataSection("date", dateString));
        form.Add(new MultipartFormDataSection("gender", genderString));
        form.Add(new MultipartFormDataSection("hand", handString));
        form.Add(new MultipartFormDataSection("glasses", glassesString));
        form.Add(new MultipartFormDataSection("sickness", sicknessString));

        UnityWebRequest webRequest = UnityWebRequest.Post(modifyUserURL, form);
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
        else if (webRequest.downloadHandler.text == "3")
        {
            messageBoxTitle.text = "Hiba!";
            messageBoxMessage.text = "Létezik már ilyen nevû felhasználó!";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "1")
        {
            messageBoxTitle.text = "Szerver hiba!";
            messageBoxMessage.text = "A szerver nem tud csatlakozni az adatbázishoz.";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "4")
        {
            messageBoxTitle.text = "Adatbázis hiba!";
            messageBoxMessage.text = "Sikertelen mentés!";
            messageBox.SetActive(true);
        }
        else if (webRequest.downloadHandler.text == "2")
        {
            messageBoxTitle.text = "Adatbázis hiba!";
            messageBoxMessage.text = "Sikertelen felhasználó ellenõrzés!";
            messageBox.SetActive(true);
        }

    }

    public void SaveUserSQLite(string usernameString, string dateString, string genderString, string handString, string glassesString, string sicknessString)
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

            

            if (sicknessString.Length == 0)
            {
                using (var command = conn.CreateCommand())
                {
                    string newSickenessString = "nincs";
                    command.CommandText = $"UPDATE users SET gender = '{genderString}', date = '{dateString}', hand = '{handString}', glasses = '{glassesString}', sickness = '{newSickenessString}' WHERE username = '{usernameString}'";

                    command.ExecuteNonQuery();
                }
                messageBoxTitle.text = "Sikeres mentés!";
                messageBoxMessage.text = "Az adatok beszúrásra kerültek az adatbázisba.";
                messageBox.SetActive(true);
                username.text = "";
                date.text = "";
                sickness.text = "";
                gender.value = 0;
                hand.value = 0;
                glasses.value = 0;
            }
            else
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"UPDATE users SET gender = '{genderString}', date = '{dateString}', hand = '{handString}', glasses = '{glassesString}', sickness = '{sicknessString}' WHERE username = '{usernameString}'";

                    command.ExecuteNonQuery();
                }
                messageBoxTitle.text = "Sikeres mentés!";
                messageBoxMessage.text = "Az adatok beszúrásra kerültek az adatbázisba.";
                messageBox.SetActive(true);
                username.text = "";
                date.text = "";
                sickness.text = "";
                gender.value = 0;
                hand.value = 0;
                glasses.value = 0;
            }

            conn.Close();
        }
    }

    public void OK()
    {
        messageBox.SetActive(false);
    }

    public void VerifyInputs()
    {
        bool usernameIsValid = (username.text.Length >= 5 && username.text.Length <= 16);
        //bool dateIsValid = ValidateDate(date.text);

        if (usernameIsValid)
        {
            getUserButton.interactable = true;
        }
        else
        {
            getUserButton.interactable = false;
        }

        if (userEmpty == false)
        {
            if (username.text != usernameValid)
            {
                date.interactable = false;
                gender.interactable = false;
                hand.interactable = false;
                glasses.interactable = false;
                sickness.interactable = false;

                date.text = "";
                gender.value = 0;
                hand.value = 0;
                glasses.value = 0;
                sickness.text = "";
                userEmpty = true;
                modifyUserButton.interactable = false;
            }
        }
        
    }

    private bool ValidateDate(string date)
    {
        try
        {
            // for US, alter to suit if splitting on hyphen, comma, etc.
            string[] dateParts = date.Split('.');

            // create new date from the parts; if this does not fail
            // the method will return true and the date is valid

            DateTime testDate = new
                DateTime(Convert.ToInt32(dateParts[0]),
                Convert.ToInt32(dateParts[1]),
                Convert.ToInt32(dateParts[2]));


            return true;
        }
        catch
        {
            // if a test date cannot be created, the
            // method will return false
            return false;
        }
    }
}
