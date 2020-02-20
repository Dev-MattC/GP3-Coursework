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

public class SaveAndLoadScript : MonoBehaviour
{
    GameDataScript gameDataScript;
    PlayerBehavioursScript playerBehavioursScript;
    EnemyScript enemyScript;

    string dbConnectionString; //Create a place to store the database location

    int playerID;

    GameObject player;
    GameObject enemy;

    // Use this for initialization
    void Start()
    {
        GameObject gameData = GameObject.Find("GameDataObject"); //Get all game objects needed
        player = GameObject.Find("Player"); 
        enemy = GameObject.Find("Enemy");

        gameDataScript = gameData.GetComponent<GameDataScript>();//This is used to transfer the PK between all scenes
        playerBehavioursScript = player.GetComponent<PlayerBehavioursScript>();//This is used to allow us to load new data into player and enemy scripts
        enemyScript = enemy.GetComponent<EnemyScript>();

        dbConnectionString = "URI=file:" + Application.dataPath + "/Database.sqlite"; //filepath

        playerID = gameDataScript.playerKey;


        if (gameDataScript.loaded)
        {
            StartCoroutine("Wait"); //wait so the other objects can fully load (See IEnumerator at bottom of this script)

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPlayer(int id)
    {
        using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
        {
            connection.Open(); //open the connection
            using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
            {

                string SqlCommand = String.Format("SELECT * FROM PlayerTable WHERE primary_key  = (\"{0}\")", id); //Select the row for this current player ID

                dbCommand.CommandText = SqlCommand;

                using (IDataReader reader = dbCommand.ExecuteReader()) //Read the file
                {
                    while (reader.Read())
                    {
                        playerBehavioursScript.energy = reader.GetInt32(2); //Set the players energy
                        playerBehavioursScript.LoadColourFromDB(); //Set the players colour
                        player.transform.position = new Vector3(reader.GetFloat(3), 0.5f, reader.GetFloat(4)); //Set the players position

                    }
                    reader.Close();
                }
            }
            connection.Close(); //Close
        }
    }

    public void LoadEnemy(int id)
    {
        using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
        {
            connection.Open(); //open the connection
            using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
            {

                string SqlCommand = String.Format("SELECT * FROM EnemyTable WHERE primary_key  = (\"{0}\")", id); //Select correct row of enemytable for this player id

                dbCommand.CommandText = SqlCommand;

                using (IDataReader reader = dbCommand.ExecuteReader()) //Read the file
                {
                    while (reader.Read())
                    {
                        enemyScript.enemyEnergy = reader.GetInt32(1); //Assign the enemy the energy that was saved
                        enemyScript.LoadEnemyColourFromDB(); //Update the enemies colour
                        enemy.transform.position = new Vector3(reader.GetFloat(2), 0.5f, reader.GetFloat(3)); //Move the enemy to where it was when it was saved

                    }
                    reader.Close();
                }
            }
            connection.Close(); //Close
        }
    }

    public void Save()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = enemy.transform.position;
        using (IDbConnection connection = new SqliteConnection(dbConnectionString)) //Set up the connection
        {
            connection.Open(); //open the connection
            using (IDbCommand dbCommand = connection.CreateCommand()) //Create the command
            {
                string SqlCommand = String.Format("UPDATE PlayerTable SET positionx = (\"{0}\") WHERE primary_key = (\"{1}\")", playerPos.x, gameDataScript.playerKey); // Use this string to store everything needed to reload the player
                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write

                SqlCommand = String.Format("UPDATE PlayerTable SET positionz = (\"{0}\") WHERE primary_key = (\"{1}\")", playerPos.z, gameDataScript.playerKey); //stores players z pos
                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write

                SqlCommand = String.Format("UPDATE PlayerTable SET energy = (\"{0}\") WHERE primary_key = (\"{1}\")", playerBehavioursScript.energy, gameDataScript.playerKey); //Stores players energy

                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write

                //Write to Enemy-------------------------------------------------------------------------------------------------------------------------------------------------
                //This used to be 2 lines (SqlCommand = String.Format("UPDATE EnemyTable SET positionx = (\"{0}\"), positionz = (\"{1}\"), enemy_energy = (\"{2}\") WHERE primary_key = (\"{3}\")", enemy.transform.position.x, enemy.transform.position.z, enemyScript.enemyEnergy, gameDataScript.playerKey); // Use this string to store everything needed to reload the enemy)
                //But although this worked with SDL it would not save properly using this so I broke it up

                SqlCommand = String.Format("UPDATE EnemyTable SET positionx = (\"{0}\") WHERE primary_key = (\"{1}\")", enemyPos.x, gameDataScript.playerKey); // Use this string to store everything needed to reload the player
                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write

                SqlCommand = String.Format("UPDATE EnemyTable SET positionz = (\"{0}\") WHERE primary_key = (\"{1}\")", enemyPos.z, gameDataScript.playerKey); //Stores Enemies z pos
                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write

                SqlCommand = String.Format("UPDATE EnemyTable SET enemy_energy = (\"{0}\") WHERE primary_key = (\"{1}\")", enemyScript.enemyEnergy, gameDataScript.playerKey); //Sotres enemies energy

                dbCommand.CommandText = SqlCommand; //Get the command
                dbCommand.ExecuteScalar(); //Write


                connection.Close(); //Close
            }
        }
    }

    public void Exit()
    {
        Time.timeScale = 1; //Resets timescale if we come back to this scene
        SceneManager.LoadScene("End Scene");
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.01f); //Waits for the objects to fully load then loads previous player and enemy positions and energies
        LoadPlayer(playerID); //This would throw errors without this wait in place previously
        LoadEnemy(playerID);
    }
}
