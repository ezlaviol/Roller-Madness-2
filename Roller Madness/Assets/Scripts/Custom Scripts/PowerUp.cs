using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Ball; // need this to import the ball code

public class PowerUp : MonoBehaviour
{
	public GameObject PowerUpCanvas;

	// On startup,
	void Start () 
	{
		PowerUpCanvas.SetActive(false); // this disables the powerup UI
	}

	// if the player touches this object,
	void OnTriggerEnter (Collider other)
	{
		if ((other.tag == "Player" ))
		{
			PowerUpCanvas.SetActive(true); // this enables the powerup UI
			// the power-up command is called in its code
			other.gameObject.GetComponent<Ball>().PowerUpSpicy();
		}
	}
}
