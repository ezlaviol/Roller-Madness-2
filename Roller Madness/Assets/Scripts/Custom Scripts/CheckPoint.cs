using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	public GameObject CheckpointCanvas;
	public int livesGranted = 0;

	private bool reached = false;
	private Vector3 checkpointPosition;
	private Quaternion checkpointRotation;

	// On startup,
	void Start () 
	{
		// store initial position and rotation
		checkpointPosition = transform.position;
		checkpointRotation = transform.rotation;

		CheckpointCanvas.SetActive(false); // this disables the checkpoint UI	
	}

	// if the player touches this object,
	void OnTriggerEnter (Collider other)
	{
		if ((other.tag == "Player" ))
		{
			reached = true; // say checkpoint is reached
			// update respawn position of player to this object's location
			other.gameObject.GetComponent<Health>().updateRespawn(checkpointPosition, checkpointRotation);
			// add extra life to player
			other.gameObject.GetComponent<Health>().ApplyBonusLife(livesGranted);

			CheckpointCanvas.SetActive(true); // this brings up the checkpoint UI
		}
	}

	void OnTriggerExit (Collider other)
	{
		if ((other.tag == "Player" ))
		{
			CheckpointCanvas.SetActive(false); // this disables the checkpoint UI again
		}

		// if the checkpoint is reached,
		if (reached) {
			DestroyObject(this.gameObject);	// destroy it
		}
	}
}
