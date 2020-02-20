using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMushroomsScript : MonoBehaviour {

    public int numberOfMushrooms;


    // Use this for initialization
    void Start () {

        for (int i = 0; i < numberOfMushrooms; i++)
        {
            CreateMushroom(i);
        }

        //The black hole is also created here
        GameObject blackHole = GameObject.CreatePrimitive(PrimitiveType.Sphere); //Is given the shape sphere
        blackHole.transform.position = new Vector3(3, 0.5f, 3); //position is set
        Renderer rend = blackHole.GetComponent<Renderer>(); //Get the renderer to change its colour
        rend.material.color = Color.black;
        blackHole.name = "blackHole"; //Given a name so that the player and enemy can detect collision
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private GameObject CreateMushroom(int id)
    {
        GameObject mushroom = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        float randomPosX = Random.Range(-4.5f, 4.5f);
        float randomPosZ = Random.Range(-4.5f, 4.5f);


        //Set the position of the object
        mushroom.transform.position = new Vector3(randomPosX, 0.0f, randomPosZ);

        //Change colour of object
        Renderer rend = mushroom.GetComponent<Renderer>();
        rend.material.color = new Vector4(0.7f, 0.3f, 0.3f); //brownish colour for healthy mushroom

        mushroom.name = "mushroom" + id; //Gives the mushroom an id
        mushroom.tag = "HealthyMushroom"; //tags the mushroom

        mushroom.AddComponent<MushroomBehaviourScript>(); //Attaches the mushroom behaviour script to each mushroom created

        return mushroom;
    }
}
