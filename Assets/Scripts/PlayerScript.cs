using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Dynamic tiedye texture (RenderTexture/Texture2D?).
[RequireComponent(/*typeof(SpriteRenderer), */typeof(Collider2D), typeof(Rigidbody2D), typeof(Phaser))]
public class PlayerScript : MonoBehaviour
{
	//private Sprite sprite;

	//[SerializeField]
	//private RenderTexture texture;

	//[SerializeField]
	//private Material playerMaterial;

	private SpriteRenderer spriteRenderer;
	private new Collider2D collider;
	public Collider2D Collider { get => collider; }
	private new Rigidbody2D rigidbody;

	public Scroller scroller;

	public Vector2 jumpForce = new Vector2(0, 70);

	public GameObject backgroundTop;

	Phaser phaser;
	private void Reset()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<Collider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
		scroller = FindObjectOfType<Scroller>();

		//texture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGBFloat);
		phaser = GetComponent<Phaser>();
	}
	// Start is called before the first frame update
	void Start()
	{
		rigidbody ??= GetComponent<Rigidbody2D>();
		scroller ??= FindObjectOfType<Scroller>();
    }

	public bool phasing { get => scroller.IsPhased; }
    void Update()
    {
		//if (!Camera.current.pixelRect.Contains(GetComponent<Collider2D>().bounds.max))
		//	SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		if (grounded && Input.GetButtonDown("Jump"))
		{
			ForceJump();
			multiplier++;
		}
		// Keep a background behind the player
		backgroundTop.transform.position = new Vector3(transform.position.x, transform.position.y, backgroundTop.transform.position.z);
    }

	private int multiplier = 0;
	private int threshold = 20;
	private NoteScript currNote;
	/// <summary>
	/// 
	/// </summary>
	public void ForceJump()
	{
		NoteScript nextNote = null;
		if (Input.GetButton("Fire3")/* && multiplier >= threshold*/)
			//Debug.Log("Fire3");
			nextNote = currNote.FindNextHigherNote();

		if (nextNote == null)
			nextNote = currNote.FindNextNote();
		//Debug.Log($"NextNote.pos: {nextNote?.transform?.position}");
		// Dynamically determine jump speed
		if (nextNote == null)
		{
			Debug.LogWarning("No New Note Found");
			var rbv = rigidbody.velocity;
			rbv.y = 20;
			rigidbody.velocity = rbv;
			grounded = false;
		}
		else
		{
			Debug.Log($"Target: {nextNote?.name}");
			var desiredPosition = new Vector2(nextNote.transform.position.x, nextNote.collider.bounds.max.y);
			var dT = MyPhysicsHelper.GetTravelTime(desiredPosition.x - transform.position.x, Scroller.SCROLL_SPEED);
			var desiredVelocity = MyPhysicsHelper.GetDesiredVelocity(desiredPosition.y - transform.position.y, dT, Physics2D.gravity.y);

			var rbv = rigidbody.velocity;
			rbv.y = desiredVelocity;
			rigidbody.velocity = rbv;
			grounded = false;
		}
	}

	private bool grounded = false;
	private void OnTriggerEnter2D(Collider2D collider)
	{
		//if (collider.GetComponent<NoteScript>() != null)
		//	Debug.Log($"Note Entered at time {Time.timeSinceLevelLoad} & position {transform.position}");
		Debug.Log($"Entered {collider.name} at time {Time.timeSinceLevelLoad} & position {transform.position}");
		if (collider.tag.Contains("Death"))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		currNote = collider.GetComponent<NoteScript>();
		OnTriggerStay2D(collider);
		//if (!phasing && collider.GetComponent<SpriteRenderer>()?.material == scroller.unphasedMaterial ||
		//	phasing && collider.GetComponent<SpriteRenderer>()?.material == scroller.phasedMaterial)
		//	grounded = true;
		//else
		//	grounded = false;
	}
	private void OnTriggerStay2D(Collider2D collider)
	{
		//if (collider.GetComponent<NoteScript>() != null)
		//	Debug.Log($"Note Entered at time {Time.timeSinceLevelLoad} & position {transform.position}");
		//Debug.Log($"Entered {collider.name} at time {Time.timeSinceLevelLoad} & position {transform.position}");
		//if (collider.tag.Contains("Death"))
		//	SceneManager.LoadScene(SceneManager.GetActiveScene().name);

		//var sr = collider.GetComponent<SpriteRenderer>();
		//if (!phasing && sr?.material == scroller.unphasedMaterial ||
		//	phasing && sr?.material == scroller.phasedMaterial)
		if ((!phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased")) ||
			(phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")))
			grounded = true;
		else
			grounded = false;
		//Debug.Assert(sr?.material == scroller.unphasedMaterial || sr?.material == scroller.phasedMaterial, "sr?.material is weird");
		//currNote = collider.GetComponent<NoteScript>();
	}
	private void OnTriggerExit2D(Collider2D collider)
	{
		grounded = false;
		if ((collider.gameObject.layer == LayerMask.NameToLayer("Beatmap") || collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased") || collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")) && rigidbody.velocity.y < 0)
		{
			// TODO: Change when there's Walls vs Notes
			//if (!phasing && collider.GetComponent<SpriteRenderer>()?.material == scroller.unphasedMaterial ||
			//	phasing && collider.GetComponent<SpriteRenderer>()?.material == scroller.phasedMaterial)
				if ((!phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased")) ||
					 (phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")))
			{
				Debug.Log($"phasing: {phasing}; layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
				scroller.RegisterMistake();
				multiplier = 0;
			}
			//if (Vector3.Distance(GameObject.Find("Bottom Death").transform.position, collider.transform.position) < 12.5)
			if (collider.GetComponent<NoteScript>().FindNextLowerNote() == null)
				ForceJump();
		}
	}
}
