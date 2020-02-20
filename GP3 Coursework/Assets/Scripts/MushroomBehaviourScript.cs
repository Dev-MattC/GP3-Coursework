using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBehaviourScript : MonoBehaviour {

    //Sets up unique variables for the mushroom
    int maxDeathAge;
    int offset;
    float mushroomMaxAge;
    float mushroomCurrentAge;

    //Sets up some check booleans
    bool poisonCheck;
    bool deathCheck;

    bool startShrinkMove;

    Renderer rend; //Creates a renderer object

    // Transforms to act as start and end markers for the journey.
    Transform startMarker;
    Vector3 endMarker;

    // Movement speed in units/sec.
    public float lerpSpeed = 0.1f;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    Vector3 halfScale;
    Vector3 startPos;


    // Use this for initialization
    void Start () {
        maxDeathAge = 60; //Sets the max death age as 600 (10 minutes)
        offset = Random.Range(0, 60); //Provides an offset specific to the mushroom
        mushroomMaxAge = Random.Range(0, maxDeathAge); //Sets the max age the mushroom can be
        mushroomCurrentAge = mushroomMaxAge;

        rend = this.GetComponent<Renderer>();

        //Check if the mushroom is dead or poisoned
        poisonCheck = false;
        deathCheck = false; 

        

        startShrinkMove = false; //Used to tell the update method when to start shrinking the mushroom

        halfScale = new Vector3(0.5f, 0.5f, 0.5f); //Used to tell when the mushroom is half its scale

        startPos = this.transform.position; //Start Position of the mushroom
    }
	
	// Update is called once per frame
	void Update () {
        mushroomCurrentAge -= Time.deltaTime;
        if (mushroomCurrentAge <= mushroomMaxAge / 2 && (!poisonCheck))
        {
            TurnPoisionous();
            poisonCheck = true; //makes sure the function is only called once
        }
        if (mushroomCurrentAge <= 0 && (!deathCheck))
        {
            DestroyMushroom();
            deathCheck = true; //makes sure the function is only called once
        }

        if(!startShrinkMove) //Only does this movement before the mushroom is dead
        {
            transform.position = startPos + new Vector3(Mathf.Sin(Time.time + offset), 0.0f, Mathf.Sin(Time.time + offset)); //Oscillates the mushrooms based on time since start of game and unique offset

        }
        else if (startShrinkMove)
        {
            // Find the distance already covered
            float distCovered = (Time.time - startTime) * lerpSpeed;

            // FCalculate the fraction of journey completed
            float fracJourney = distCovered / journeyLength;

            // Moves the mushroom up
            transform.position = Vector3.Lerp(startMarker.position, endMarker, fracJourney);

            transform.localScale -= new Vector3( 0.01f, 0.01f, 0.01f); //shrinks the mushroom

            if (this.transform.localScale == halfScale) //When the mushroom is 50% its original scale it is destroyed
            {
                Destroy(this.gameObject);
            }

        }
	}

    void TurnPoisionous()
    {
        rend.material.color = Color.green; //Change the mushroom colour to green
        this.gameObject.tag = "PoisonousMushroom"; //Tag the mushroom as poisonus so that it now lowers player energy when collided
    }

    void DestroyMushroom()
    {
        
        //Destroy(this.gameObject);
        // Start time of movement
        startTime = Time.time;

        startMarker = this.transform; //Start Position for lerp
        endMarker = new Vector3(this.transform.position.x, (this.transform.position.y + 1.0f), this.transform.position.z); //End position for lerp

        rend.material.color = Color.black; //Change colour to black to show mushroom is dead
        this.gameObject.tag = "DeadMushroom"; //Change Object tag so player cannot collect it
        // Calculate the journey length.
        journeyLength = Vector3.Distance(startMarker.position, endMarker);
        startShrinkMove = true;
    }
}
