using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavioursScript : MonoBehaviour {

    public float speed; //Speed is public so players can change it in inspector

    bool paused; //Used to pause the game

    private Rigidbody rb;
    Renderer rend; //Creates a renderer object

    public GameObject ground; //Creates a public ground object that is set in the inspector
    public GameObject saveButton;
    public GameObject exitButton;


    //Sets up all the global variables needed to control the player
    public int energy; //this is made public so other scripts can access it
    int delta_energyInc;
    int delta_energyDec;
    int minEnergy;
    int maxEnergy;

    

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY; //Freezes the player on the y axis
        rend = this.GetComponent<Renderer>(); //Gets the renderer attached to the game object this script is attached to
        rend.material.color = new Vector4(1.0f, 1.0f, 1.0f); //Sets the colour of the player at the start to be white
        speed = 10; //Set player movement speed
        

        energy = 0; //Sets player energy at start to be 50 out of 100
        delta_energyInc = 10; //Holds the value for energy increas
        delta_energyDec = -10; //Holds value for energy decrease
        minEnergy = 0; //Minimum energy the player can have
        maxEnergy = 100; //Max energy for the player

        saveButton.SetActive(false);
        exitButton.SetActive(false);

    }



    // Update is called once per frame
    void Update()
    {
        //Check position of player and wrap to other side of world
        CheckPos(rb);

        if (Input.GetKeyDown(KeyCode.Escape)) //Waits for player input so they can pause the game
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0;
                saveButton.SetActive(true);
                exitButton.SetActive(true);
                
            }
            else
            {
                paused = false;
                Time.timeScale = 1;
                saveButton.SetActive(false);
                exitButton.SetActive(false);

            }
        }
    }

    void FixedUpdate()
    {
        //Used for player movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    public void LoadColourFromDB()
    {
        float colourAdded = (energy / 100.0f) * -1; //Reduces the amount to 0.1f or -0.1f as the colours range from 0 to 1, Is multiplied by negative 1 so that positive number lower the colour as (0,0,0) is black
        Color colourChange = new Vector4(colourAdded, colourAdded, colourAdded); //Creates a colour which is added to the current colour

        Color currentColour = rend.material.color; //Gets the current colour
        Color newColour = currentColour + colourChange; //Adds the colours together to get the new colour       

        rend.material.color = newColour; // Sets the material colour to the new colour
    }

    public void CheckPos(Rigidbody rb) //This is made public so that it can be reused in the enemy script
    {
        //Checks the players x and z positions against the size of the the game object ground which is set to the plane in the editor
        //If the player goes over the bounds the position is updated to the other side of the world
        if (rb.position.x > ground.GetComponent<Renderer>().bounds.size.x / 2.0f)
        {
            rb.position = updateX(-ground.GetComponent<Renderer>().bounds.size.x / 2.0f);
        }
        else if (rb.position.x < -ground.GetComponent<Renderer>().bounds.size.x / 2.0f)
        {
            rb.position = updateX(ground.GetComponent<Renderer>().bounds.size.x / 2.0f);
        }
        else if (rb.position.z > ground.GetComponent<Renderer>().bounds.size.z / 2.0f)
        {
            rb.position = updateZ(-ground.GetComponent<Renderer>().bounds.size.z / 2.0f);
        }
        else if (rb.position.z < -ground.GetComponent<Renderer>().bounds.size.z / 2.0f)
        {
            rb.position = updateZ(ground.GetComponent<Renderer>().bounds.size.z / 2.0f);
        }
    }


    public Vector3 updateX(float x) //This is made public so that it can be reused in the enemy script
    {
        return new Vector3(x, transform.position.y, transform.position.z); //Takes in a new x for the player to update to
    }

    public Vector3 updateZ(float z) //This is made public so that it can be reused in the enemy script
    {
        return new Vector3(transform.position.x, transform.position.y, z); //Takes in a new z for the player to update to
    }

    

    //Change the colour of the player so that they are black when full of energy and white when out of energy
    public void ChangePlayerColour(int changeAmount)
    {
        float colourAdded = (changeAmount / 100.0f) *-1; //Reduces the amount to 0.1f or -0.1f as the colours range from 0 to 1, Is multiplied by negative 1 so that positive number lower the colour as (0,0,0) is black
        Color colourChange = new Vector4(colourAdded, colourAdded, colourAdded); //Creates a colour which is added to the current colour
        
        Color currentColour = rend.material.color; //Gets the current colour
        Color newColour = currentColour + colourChange; //Adds the colours together to get the new colour       
        
        rend.material.color = newColour; // Sets the material colour to the new colour
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("HealthyMushroom")) //Checks for collision with a healthy mushroom
        {
            Destroy(collision.gameObject); //Destroys the mushroom
            if (energy != maxEnergy) //Checks if the player is at max energy before adding more energy or adding to the colour
            {
                energy += delta_energyInc;
                ChangePlayerColour(delta_energyInc);
            }
        }
        else if (collision.gameObject.tag.Equals("PoisonousMushroom"))
        {
            Destroy(collision.gameObject); //Destroys the mushroom
            if (energy != minEnergy) //Checks if the player is at max energy before adding more energy or adding to the colour
            {
                energy += delta_energyDec;
                ChangePlayerColour(delta_energyDec);
            }
        }

        if (collision.gameObject.name.Equals("blackHole"))
        {
            Destroy(this.gameObject); //Destroys the player if they hit a black hole
            SceneManager.LoadScene("End Scene");
        }
    }
}
