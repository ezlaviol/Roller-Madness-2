using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager
using TMPro; // import TextMeshPro

public class GameManager : MonoBehaviour {

	public static GameManager gm;

	[Tooltip("If not set, the player will default to the gameObject tagged as Player.")]
	public GameObject player;

	public enum gameStates {Playing, Death, GameOver, BeatLevel};
	public gameStates gameState = gameStates.Playing;

	public int score=0;
	public int scoreAlt=0;
	public bool canBeatLevel = false;
	public int beatLevelScore=0;
	public int beatLevelScoreAlt=0;
	
	public string altScoreName="";
	public bool altScoreTracker=false;

	public GameObject mainCanvas;
	public TMP_Text mainScoreDisplay;
	public TMP_Text altScoreDisplay;
	public GameObject gameOverCanvas;
	public TMP_Text gameOverScoreDisplay;
	public TMP_Text altGameOverScoreDisplay;

	[Tooltip("Only need to set if canBeatLevel is set to true.")]
	public GameObject beatLevelCanvas;

	public AudioSource backgroundMusic;
	public AudioClip gameOverSFX;

	[Tooltip("Only need to set if canBeatLevel is set to true.")]
	public AudioClip beatLevelSFX;

	public GameObject PauseCanvas;

	private Health playerHealth;

	// private variables
	GameObject _player;
	Vector3 _spawnLocation;
	Scene _scene;

	void Start () {
		if (gm == null) 
			gm = gameObject.GetComponent<GameManager>();

		if (player == null) {
			player = GameObject.FindWithTag("Player");
		}

		playerHealth = player.GetComponent<Health>();

		// setup score display
		Collect (0);
		if (altScoreTracker) {CollectAlt (0);}

		// make other UI inactive
		gameOverCanvas.SetActive (false);
		PauseCanvas.SetActive(false);
		if (canBeatLevel)
			beatLevelCanvas.SetActive (false);
		
		var currentScene = SceneManager.GetActiveScene();

		setupDefaults();
	}

	void Update () {
		// if ESC pressed then pause the game
		if (PauseCanvas!=null) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (Time.timeScale > 0f) {
					PauseCanvas.SetActive(true); // this brings up the pause UI
					Time.timeScale = 0f; // this pauses the game action
				} else {
					Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
					PauseCanvas.SetActive(false); // remove the pause UI
				}
			}
		}

		switch (gameState)
		{
			case gameStates.Playing:
				if (playerHealth.isAlive == false)
				{
					// update gameState
					gameState = gameStates.Death;

					// set the end game score
					gameOverScoreDisplay.text = mainScoreDisplay.text;
					altGameOverScoreDisplay.text = altScoreDisplay.text;

					// switch which GUI is showing		
					mainCanvas.SetActive (false);
					gameOverCanvas.SetActive (true);
				} else if (canBeatLevel && score>=beatLevelScore && (!altScoreTracker || scoreAlt>=beatLevelScoreAlt)) {
					// update gameState
					gameState = gameStates.BeatLevel;

					// hide the player so game doesn't continue playing
					player.SetActive(false);

					// switch which GUI is showing			
					mainCanvas.SetActive (false);
					beatLevelCanvas.SetActive (true);
				}
				break;
			case gameStates.Death:
				backgroundMusic.volume -= 0.01f;
				if (backgroundMusic.volume<=0.0f) {
					AudioSource.PlayClipAtPoint (gameOverSFX,gameObject.transform.position);

					gameState = gameStates.GameOver;
				}
				break;
			case gameStates.BeatLevel:
				backgroundMusic.volume -= 0.01f;
				if (backgroundMusic.volume<=0.0f) {
					AudioSource.PlayClipAtPoint (beatLevelSFX,gameObject.transform.position);
					
					gameState = gameStates.GameOver;
				}
				break;
			case gameStates.GameOver:
				// nothing
				break;
		}

	}

	// setup all the variables, the UI, and provide errors if things not setup properly.
	void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player");
		
		if (_player==null)
			Debug.LogError("Player not found in Game Manager");

		// get current scene
		_scene = SceneManager.GetActiveScene();

		// get initial _spawnLocation based on initial position of player
		_spawnLocation = _player.transform.position;

		// friendly error messages
		if (PauseCanvas==null)
			Debug.LogError ("Need to set PauseCanvas on Game Manager.");
		
		// get stored player prefs
		refreshPlayerState();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {
		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	public void Collect(int amount) {
		score += amount;
		if (canBeatLevel) {
			mainScoreDisplay.text = score.ToString () + " of "+beatLevelScore.ToString ();
		} else {
			mainScoreDisplay.text = score.ToString ();
		}

	}

	public void CollectAlt(int amount) {
		scoreAlt += amount;
		int amountRemaining = beatLevelScoreAlt - scoreAlt;
		string plural = "";
		if (amountRemaining != 1) {plural = "s";}
		if (canBeatLevel) {
			altScoreDisplay.text = amountRemaining.ToString () + " " + altScoreName + plural + " remaining";
		} else {
			altScoreDisplay.text = scoreAlt.ToString ();
		}

	}
}
