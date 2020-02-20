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

public class MainMenuScript : MonoBehaviour {

    //Used for UI in scene
    public InputField nameField;
    public Text nameText;
    public Text playerIdText;
    public GameObject okButton;
    public GameObject startButton;
    public GameObject quitButton;
    public GameObject loadButton;

    string dbConnectionString; //Create a place to store the database location

    GameDataScript gameDataScript;


    // Use this for initialization
    void Start () {
        GameObject gameData = GameObject.Find("GameDataObject");
        dbConnectionString = "URI=file:" + Application.dataPath + "/Database.sqlite"; //filepath

        gameDataScript = gameData.GetComponent<GameDataScript>();//This is used to transfer the PK between all scenes

        //Set to false whenever main menu reloads
        okButton.SetActive(false);
        startButton.SetActive(true);
        quitButton.SetActive(true);
        loadButton.SetActive(true);
        nameText.enabled = false;
        playerIdText.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadGame()
    {
        SceneManager.LoadScene("Game Scene");
    }

    public void GetPlayerID()
    {
        string name;

        if(nameField.text != "")
        {
            name = nameField.text; //Get the name in the field

            using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
            {
                connection.Open(); //open the connection
                using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
                {
                    string SqlCommand = String.Format("INSERT INTO PlayerTable(player_name, positionx) VALUES (\"{0}\",\"{1}\")", name, 6.0f); // Use this string to store Sqlite commands

                    dbCommand.CommandText = SqlCommand; //Get the command
                    dbCommand.ExecuteScalar(); //Write

                    SqlCommand = String.Format("INSERT INTO EnemyTable(enemy_energy) VALUES (\"{0}\")", 0);

                    dbCommand.CommandText = SqlCommand;
                    dbCommand.ExecuteScalar();

                    

                    SqlCommand = String.Format("SELECT * FROM PlayerTable ORDER BY primary_key DESC limit 1");

                    dbCommand.CommandText = SqlCommand;

                    using (IDataReader reader = dbCommand.ExecuteReader()) //Read the file
                    {
                        while(reader.Read())
                        {
                            gameDataScript.playerKey = reader.GetInt32(0); //Get the primary key                            
                        }
                        reader.Close();
                    }
                }
                connection.Close(); //Close
            }
            playerIdText.text = "Your Player ID is: " + gameDataScript.playerKey + " Remeber this if you wish to save your game"; //Shows the player their ID
            //Tidies UI
            playerIdText.enabled = true;
            okButton.SetActive(true);
            startButton.SetActive(false);
            quitButton.SetActive(false);
            loadButton.SetActive(false);
        }
        else
        {
            nameText.enabled = true;
        }
    }

    public void LoadFromID()
    {
        string name;

        if (nameField.text != "")
        {
            name = nameField.text;

            using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
            {
                connection.Open(); //open the connection
                using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
                {

                    string SqlCommand = String.Format("SELECT * FROM PlayerTable");

                    dbCommand.CommandText = SqlCommand;

                    using (IDataReader reader = dbCommand.ExecuteReader()) //Read the file
                    {
                        while (reader.Read())
                        {
                            if(name == reader.GetInt32(0).ToString() && reader.GetFloat(3) != 6.0f) //Check if the user has input a valid ID 6.0f is used to check if the game has been saved
                            {
                                gameDataScript.loaded = true;
                                gameDataScript.playerKey = reader.GetInt32(0); //Set player key to load enemy and player from it in main game
                                LoadGame();
                            }
                            else
                            {
                                nameText.enabled = true;
                            }
                        }
                        reader.Close();
                    }
                }
                connection.Close(); //Close
            }
        }
        else
        {
            nameText.enabled = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
