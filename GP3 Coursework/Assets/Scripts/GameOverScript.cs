using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Configuration;
using System.Data;
using System.EnterpriseServices;
using System.Security;

public class GameOverScript : MonoBehaviour {

    string dbConnectionString; //Create a place to store the database location
    public Text namesText;
    public Text scoresText;
    int scores;

    // Use this for initialization
    void Start () {
        dbConnectionString = "URI=file:" + Application.dataPath + "/Database.sqlite"; //filepath
        LoadScores();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadScores()
    {
        using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
        {
            connection.Open(); //open the connection
            using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
            {

                string SqlCommand = String.Format("SELECT * FROM PlayerTable ORDER BY energy DESC limit 5"); //Gets the top 5 players by sorting in energy

                dbCommand.CommandText = SqlCommand;

                using (IDataReader reader = dbCommand.ExecuteReader()) //Read the file
                {
                    while (reader.Read())
                    {
                        namesText.text += "" + reader.GetString(1) + "\n"; //Gets the players name then takes a new line
                        scoresText.text += reader.GetInt32(2).ToString() + "\n"; //Gets the players scores then takes a new line

                    }
                    reader.Close();
                }

            }
            connection.Close(); //Close
        }
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
