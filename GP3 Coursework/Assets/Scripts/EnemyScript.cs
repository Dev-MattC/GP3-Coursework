using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour {

    public float speed; //Speed is public so enemy can change it in inspector

    private Rigidbody rb;
    Renderer rend; //Creates a renderer object

    GameObject player;
    PlayerBehavioursScript playerBehavioursScript;

    public int enemyEnergy;
    int delta_energySteal;

    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY; //Freezes the player on the y axis
        rend = this.GetComponent<Renderer>(); //Gets the renderer attached to the game object this script is attached to
        rend.material.color = new Vector4(1.0f, 1.0f, 1.0f); //Sets the colour of the player at the start to be white
        speed = 5; //Set enemy movement speed

        enemyEnergy = 0;
        delta_energySteal = 10;
        rend.material.color = new Vector4(0.1f, 0.1f, 0.0f); //Start enemy as blackish (slightly varied to differentiate from black hole) then they will change to blue
        playerBehavioursScript = player.GetComponent<PlayerBehavioursScript>(); //Get the PlayerBehavioursScript so that the enemy can reuse code
    }

    void Update()
    {
        playerBehavioursScript.CheckPos(rb); //Use the Check position from PlayerBehavioursScript to allow the enemy to wraparound
       
    }

    private void FixedUpdate()
    {
        transform.LookAt(player.transform); //Turn the enemy towards the player

        rb.AddForce(transform.forward * speed); //Moves enemy to player
    }

    void StealEnergy()
    {
        if (playerBehavioursScript.energy > 0)
        {
            playerBehavioursScript.energy -= delta_energySteal; //Decreases player energy
            enemyEnergy += delta_energySteal; //Increases enemy energy
            ChangeColour(); //Make the enemy change towards blue
            playerBehavioursScript.ChangePlayerColour(-delta_energySteal); //Will lower the players colour
        }
    }

    public void LoadEnemyColourFromDB()
    {
        Color colourChange = new Vector4(0.0f, 0.0f, enemyEnergy/100); //Creates a colour which is added to the current colour

        Color currentColour = rend.material.color; //Gets the current colour
        Color newColour = currentColour + colourChange; //Adds the colours together to get the new colour       

        rend.material.color = newColour; // Sets the material colour to the new colour
    }

    void ChangeColour()
    {
        Color colourChange = new Vector4(0.0f, 0.0f, 0.1f); //Creates a colour which is added to the current colour

        Color currentColour = rend.material.color; //Gets the current colour
        Color newColour = currentColour + colourChange; //Adds the colours together to get the new colour       

        rend.material.color = newColour; // Sets the material colour to the new colour
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("blackHole"))
        {
            Destroy(this.gameObject); //Destroys the enemy if they hit a black hole
            SceneManager.LoadScene("End Scene");
        }
        if (collision.gameObject.tag.Equals("Player"))
        {
            StealEnergy(); //Steals energy from the player and increases enemy energy when collision entered
                           //This is done in collision enter so the player can get away from the enemy before this code will run again
        }
    }
}
