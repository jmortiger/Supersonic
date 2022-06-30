using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

[RequireComponent(
	typeof(AudioSource),
	typeof(Camera)
	)]
public class Scroller : MonoBehaviour
{
	/**
	 * NOTES:
	 * deltaT from start to 1st beat = 2.833s
	 * velocity at 1st beat (if vInitial = 0) = -27.79173 u/s
	 * deltaY at 1st beat = -39.3669855 u (OFF IN PLAY MODE, RELOAD WHEN TESTING; currently ~-19.83 on 1st play mode load)
	 * Assuming BPM is 170
	 * BPS = 2.8333...
	 * SPB = .352941176
	 * deltaV = -3.46235294118
	 * Distance between beats at vX = 5 u/s: 7.05882 u
	 * 
	 * # of beats: 625 (+/- 1);
	 */
	public const float FIRST_BEAT_TIME = 2.833f;
	public const int NUM_BEATS = 625;
	public const float BPM = 170;
	public const float SECONDS_PER_BEAT = 1 / (BPM / 60);
	//public const float SECONDS_PER_BEAT = 1 / (170 / 60);
	public const float SCROLL_SPEED = 5;
	public static readonly float BEAT_1_DELTA_Y = MyPhysicsHelper.GetDistanceTravelled(0, FIRST_BEAT_TIME, -9.81f/*Physics2D.gravity.y*/); //-39.3669855f;
	public const float BEAT_1_DELTA_X = SCROLL_SPEED * FIRST_BEAT_TIME;
	//public static float DeltaXAtFirstBeat { get => SCROLL_SPEED * FIRST_BEAT_TIME; }

	public float mistakeMovementPenalty = 2.5f;

	public PlayerScript player;
	public AudioSource aSource;

	public AudioClip music;

	private new Camera camera;

	public GameObject background;

	public Vector2 shakeBounds = new Vector2(4f, 4f);
	public float screenShakeLength = .25f;
	private Vector3 currPos;
	private float screenShakeTimer = 0;

	public bool IsPhased { get; private set; } = false;
	public PhaseType CurrentPhaseType { get => IsPhased ? PhaseType.Phased : PhaseType.Unphased; }
	public event EventHandler PhaseChange;
	public Material phasedMaterial;
	public Material unphasedMaterial;
	private void Reset()
	{
		camera = GetComponent<Camera>();
		camera.name = "Main Camera";
		camera.tag = "MainCamera";

		aSource = GetComponent<AudioSource>();
		aSource.clip = music ?? aSource.clip;

		player = FindObjectOfType<PlayerScript>();
	}
	// Start is called before the first frame update
	void Start()
    {
		aSource ??= GetComponent<AudioSource>();
		if (aSource.clip == null && music != null)
			aSource.clip = music;
		aSource.Play();

		currPos = transform.position;
    }

    // Update is called once per frame
    void Update()
	{
		if (Input.GetButtonDown("Debug Reset"))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//if (Input.GetButtonDown("Fire3") || 
		//	Input.GetButtonUp("Fire3") ||
		//	Input.GetButtonDown("Phase") ||
		//	Input.GetButtonUp("Phase"))
		//{
		//	IsPhased = !IsPhased;
		//	PhaseChange?.Invoke(this, new EventArgs());
		//}
		if ((Input.GetButton("Fire3") || 
		Input.GetButton("Phase")) && !IsPhased)
		{
			IsPhased = true;
			PhaseChange?.Invoke(this, new EventArgs());
		}
		if ((!Input.GetButton("Fire3") &&
		!Input.GetButton("Phase")) && IsPhased)
		{
			IsPhased = false;
			PhaseChange?.Invoke(this, new EventArgs());
		}

		var delta = Vector2.right * SCROLL_SPEED * Time.deltaTime;
		currPos += (Vector3)delta;
		player.transform.Translate(delta);
		background.transform.Translate(delta);

		// Track player above a certain height
		if (player.transform.position.y > 0)
			currPos.y += player.transform.position.y - currPos.y;
		else
			currPos = new Vector3(currPos.x, 0, currPos.z);

		if (screenShakeTimer > 0)
			ScreenShake();
		else
			transform.position = currPos;
	}

	private void ScreenShake()
	{
		screenShakeTimer -= Time.deltaTime;
		var ss = new Vector3(Random.value * shakeBounds.x - shakeBounds.x / 2, Random.value * shakeBounds.y - shakeBounds.y / 2, 0);
		transform.position = currPos + ss;
	}

	public void RegisterMistake()
	{
		screenShakeTimer = screenShakeLength;
		currPos.x += mistakeMovementPenalty;
		background.transform.Translate(mistakeMovementPenalty, 0, 0);
		Debug.Log("Mistake");
	}
}
