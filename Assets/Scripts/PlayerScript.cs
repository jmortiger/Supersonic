using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// TODO: Dynamic tiedye texture (RenderTexture/Texture2D?).
[RequireComponent(/*typeof(SpriteRenderer), */typeof(Collider2D), typeof(Rigidbody2D), typeof(Phaser))]
public class PlayerScript : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private new Collider2D collider;
	public Collider2D Collider { get => collider; }
	private new Rigidbody2D rigidbody;

	public Scroller scroller;

	public Vector2 jumpForce = new Vector2(0, 70);

	public GameObject backgroundTop;

	Phaser phaser;

	public TMP_Text multiplierDisplay;
	private void Reset()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		collider = GetComponent<Collider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
		scroller = FindObjectOfType<Scroller>();

		phaser = GetComponent<Phaser>();
	}
	// Start is called before the first frame update
	void Start()
	{
		rigidbody ??= GetComponent<Rigidbody2D>();
		scroller ??= FindObjectOfType<Scroller>();
		multiplierDisplay ??= FindObjectOfType<TMP_Text>();
		multiplierDisplay.text = $"x{multiplier}";
    }

	public bool phasing { get => scroller.IsPhased; }
    void Update()
    {
		if (grounded && Input.GetButtonDown("Jump"))
		{
			Debug.Log($"Jump: {Input.GetButtonDown("Jump")}");
			ForceJump();
			UpdateMultiplier(1);
		}
		// Keep a background behind the player
		backgroundTop.transform.position = new Vector3(transform.position.x, transform.position.y, backgroundTop.transform.position.z);
    }

	private int multiplier = 0;
	private int threshold = 2;
	private NoteScript currNote;
	/// <summary>
	/// 
	/// </summary>
	public void ForceJump()
	{
		NoteScript nextNote = null;
		if (Input.GetButton("JumpHigher")/* && multiplier >= threshold*/)
		{
			Debug.Log("FindNextHigherNote");
			nextNote = currNote.FindNextHigherNote();
			Debug.Assert(nextNote != currNote, $"nextNote == currNote");
		}

		if (nextNote == null)
		{
			Debug.Log("FindNextNote");
			nextNote = currNote.FindNextNote();
			Debug.Assert(nextNote != currNote, $"nextNote == currNote");
		}

		if (nextNote == null)
		{
			Debug.Log("FindNextLowerNote");
			nextNote = currNote.FindNextLowerNote();
			Debug.Assert(nextNote != currNote, $"nextNote == currNote");
		}

		var rbv = rigidbody.velocity;
		// Dynamically determine jump speed
		if (nextNote == null)
		{
			Debug.LogWarning("No New Note Found");
			rbv.y = 20;
		}
		else
		{
			Debug.Log($"Target: {nextNote?.name}");
			var desiredPosition = new Vector2(nextNote.transform.position/*collider.bounds.min*/.x, nextNote.collider.bounds.max.y);
			var dT = MyPhysicsHelper.GetTravelTime(desiredPosition.x - transform.position.x, Scroller.SCROLL_SPEED);
			var desiredVelocity = MyPhysicsHelper.GetDesiredVelocityFromDistance(desiredPosition.y - transform.position.y, dT, Physics2D.gravity.y);
			//if (MyPhysicsHelper.GetVelocityFinalComponent(vIComp: desiredVelocity, accelComp: Physics2D.gravity.y, deltaDComp: desiredPosition.y - transform.position.y) > float.Epsilon)
			if (MyPhysicsHelper.GetVelocityFinalComponentByTime(vIComp: desiredVelocity, accelComp: Physics2D.gravity.y, deltaT: dT) > float.Epsilon)
			{
				Debug.Log($"vI of {desiredVelocity} will overshoot");
				desiredVelocity = MyPhysicsHelper.GetInitialVelocityTrial(deltaDComp: desiredPosition.y - transform.position.y, deltaT: dT);
			}
			//var desiredVelocity = MyPhysicsHelper.GetDesiredVelocity(accelComp: Physics2D.gravity.y, deltaT: dT);
			//var desiredVelocity = MyPhysicsHelper.GetInitialVelocityComponentFromDistance(accelComp: Physics2D.gravity.y, deltaDComp: desiredPosition.y - transform.position.y);
			//var desiredVelocity = MyPhysicsHelper.GetInitialVelocityTrial(deltaDComp: desiredPosition.y - transform.position.y, deltaT: dT);
			Debug.Log($"Desired Velocity = {desiredVelocity}");

			rbv.y = desiredVelocity;
		}
		rigidbody.velocity = rbv;
		grounded = false;
	}

	private bool grounded = false;
	private void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log($"Entered {collider.name} at time {Time.timeSinceLevelLoad} & position {transform.position}");
		if (collider.name == "BottomDeath")
		{
			ForceJump();
			return;
		}
		if (collider.tag.Contains("Death") && collider.name != "BottomDeath")
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		currNote = collider.GetComponent<NoteScript>();
		OnTriggerStay2D(collider);
	}
	private void OnTriggerStay2D(Collider2D collider)
	{
		if ((!phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased")) ||
			(phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")))
			grounded = true;
		else
			grounded = false;
		//currNote = collider.GetComponent<NoteScript>();
	}
	private void OnTriggerExit2D(Collider2D collider)
	{
		grounded = false;
		if ((collider.gameObject.layer == LayerMask.NameToLayer("Beatmap") || collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased") || collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")) && rigidbody.velocity.y < 0)
		{
			// TODO: Change when there's Walls vs Notes
			if ((!phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapUnphased")) ||
				 (phasing && collider.gameObject.layer == LayerMask.NameToLayer("BeatmapPhased")))
			{
				Debug.Log($"phasing: {phasing}; layer: {LayerMask.LayerToName(collider.gameObject.layer)}");
				scroller.RegisterMistake();
				UpdateMultiplier(-multiplier);
			}
			//if (Vector3.Distance(GameObject.Find("Bottom Death").transform.position, collider.transform.position) < 12.5)
			if (collider.GetComponent<NoteScript>().FindNextLowerNote() == null)
				ForceJump();
		}
	}

	private void UpdateMultiplier(int delta)
	{
		multiplier += delta;
		if (multiplierDisplay != null)
			multiplierDisplay.text = $"x{multiplier}";
		else
			Debug.LogWarning("No multiplierDisplay");
	}
}
