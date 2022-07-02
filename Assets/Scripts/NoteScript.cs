using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NoteScript : MonoBehaviour
{
	public float jumpSpeed = 6.924706f * 2;
	public float failJumpSpeed = 6.924706f;

	public NoteScript nextNote;

	public new Collider2D collider;

	public PhaseType phase;

	public bool isWall = false;

	private void Reset()
	{
		collider = GetComponent<Collider2D>();
	}

	public NoteScript FindNextNote()
	{
		if (nextNote != null)
			return nextNote;
		var qSIC = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		//nextNote = Physics2D.Raycast(transform.position, Vector2.right).transform?.GetComponent<NoteScript>();
		//Debug.Assert(nextNote != this);
		//if (nextNote != null)
		//	return nextNote;
		var nextNotes = Physics2D.RaycastAll(transform.position, Vector2.right);
		for (int i = 0; i < nextNotes.Length; i++)
		{
			nextNote = nextNotes[i].transform?.GetComponent<NoteScript>();
			if (nextNote != this && nextNote != null && !nextNote.isWall)
				return nextNote;
			nextNote = null;
		}
		//nextNote = Physics2D.BoxCast(transform.position, new Vector2(100, collider.bounds.size.y), 0, Vector2.down).transform?.GetComponent<NoteScript>();
		//Debug.Assert(nextNote != this);
		////Debug.Assert(nextNote != null);
		nextNotes = Physics2D.BoxCastAll(transform.position, new Vector2(100, collider.bounds.size.y), 0, Vector2.down);
		for (int i = 0; i < nextNotes.Length; i++)
		{
			nextNote = nextNotes[i].transform?.GetComponent<NoteScript>();
			if (nextNote != this && nextNote != null && !nextNote.isWall)
				return nextNote;
			nextNote = null;
		}
		Physics2D.queriesStartInColliders = qSIC;
		return nextNote;
	}

	public NoteScript FindNextHigherNote()
	{
		var qSIC = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		//var nextNote = Physics2D.BoxCast(collider.bounds.max, new Vector2(collider.bounds.size.x * 10, collider.bounds.size.y), 0, Vector2.up).transform?.GetComponent<NoteScript>();
		//Debug.Assert(nextNote != this);
		var nextNotes = Physics2D.BoxCastAll(collider.bounds.max, new Vector2(collider.bounds.size.x * 10, collider.bounds.size.y), 0, Vector2.up);
		NoteScript nextNote = null;
		for (int i = 0; i < nextNotes.Length; i++)
		{
			nextNote = nextNotes[i].transform?.GetComponent<NoteScript>();
			if (nextNote != this && nextNote != null && !nextNote.isWall)
				return nextNote;
			nextNote = null;
		}
		Physics2D.queriesStartInColliders = qSIC;
		return nextNote;
	}

	public NoteScript FindNextLowerNote()
	{
		var qSIC = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders = false;
		//var nextNote = Physics2D.BoxCast(collider.bounds.min, new Vector2(collider.bounds.size.x * 10, collider.bounds.size.y), 0, Vector2.down).transform?.GetComponent<NoteScript>();
		//Debug.Assert(nextNote != this);
		var nextNotes = Physics2D.BoxCastAll(collider.bounds.min, new Vector2(collider.bounds.size.x * 10, collider.bounds.size.y), 0, Vector2.down);
		NoteScript nextNote = null;
		for (int i = 0; i < nextNotes.Length; i++)
		{
			nextNote = nextNotes[i].transform?.GetComponent<NoteScript>();
			if (nextNote != this && nextNote != null && !nextNote.isWall)
				return nextNote;
			nextNote = null;
		}
		Physics2D.queriesStartInColliders = qSIC;
		return nextNote;
	}

	//private void OnDrawGizmosSelected()
	//{
	//// vFinal = 0
	//// deltaY = (-vInitial ^ 2) / (2 * a)
	//// var deltaY = -(jumpSpeed * jumpSpeed) / (2 * Physics2D.gravity.y);
	//var deltaY = MyPhysicsHelper.GetDisplacementComponent(jumpSpeed, Physics2D.gravity.y);
	//Gizmos.DrawLine(
	//	new Vector3(collider.bounds.min.x, deltaY + /*collider.bounds.max*/transform.position.y, 0),
	//	new Vector3(collider.bounds.max.x, deltaY + /*collider.bounds.max*/transform.position.y, 0));
	//Gizmos.color = Color.red;
	////deltaY = -(failJumpSpeed * failJumpSpeed) / (2 * Physics2D.gravity.y);
	//deltaY = MyPhysicsHelper.GetDisplacementComponent(failJumpSpeed, Physics2D.gravity.y);
	//Gizmos.DrawLine(
	//	new Vector3(collider.bounds.min.x, deltaY + /*collider.bounds.min*/transform.position.y, 0),
	//	new Vector3(collider.bounds.max.x, deltaY + /*collider.bounds.min*/transform.position.y, 0));

	//Gizmos.color = new Color(1, 1, 1, .25f);
	//Gizmos.DrawCube(transform.position, new Vector2(collider.bounds.size.x * 6, collider.bounds.size.y));
	//Gizmos.color = new Color(0, 0, 0, .25f);
	//Gizmos.DrawCube(transform.position, new Vector2(100, collider.bounds.size.y));
	//}
}
