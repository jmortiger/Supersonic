using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateNotesOnBeat : ScriptableWizard
{
	[MenuItem("Tools/Create/Notes On Beat")]
	static void CreateWizard()
	{
		var wiz = DisplayWizard<CreateNotesOnBeat>("Create Notes On Beat", "Create New"/*, "Update Selected"*/);
		//if (Selection.activeTransform?.GetComponent<PatrolRoute>() != null)
		//	wiz.objectName = Selection.activeTransform.name;
		//else
		//	wiz.objectName = "Route";
		wiz.playerTransform = FindObjectOfType<PlayerScript>().transform;
		wiz.firstNotePlacement = wiz.playerTransform.position + new Vector3(Scroller.BEAT_1_DELTA_X, Scroller.BEAT_1_DELTA_Y);
		//new Vector2(wiz.playerTransform.position.x + Scroller.BEAT_1_DELTA_X, Scroller.BEAT_1_DELTA_Y);
	}

	public Transform playerTransform/* = FindObjectOfType<PlayerScript>().transform*/;

	public string parentObjectName;
	public Vector3 firstNotePlacement;// = new Vector2(pt.position.x + Scroller.BEAT_1_DELTA_X, Scroller.BEAT_1_DELTA_Y);
	public int numberOfNotesToGenerate = Scroller.NUM_BEATS;
	public float spaceBetweenNotes = Scroller.SECONDS_PER_BEAT * Scroller.SCROLL_SPEED * 4;
	public Vector3 size = Vector3.one;
	public Vector2 colliderOffset = new Vector2(0, .25f);
	public Vector2 colliderSize = new Vector2(1, 1.5f);
	public Sprite sprite;
	void OnWizardUpdate()
	{
		helpString = "Enter info.";

		isValid = true;
		errorString = "";
		if (sprite == null)
		{
			errorString = "Please select a sprite.";
			isValid = false;
		}
		if (parentObjectName == "")
		{
			errorString = "Please provide a name.";
			isValid = false;
		}
		if (parentObjectName == Selection.activeTransform?.name)
		{
			errorString = "There is already an object with this name.";
			isValid = false;
		}
	}
	void OnWizardCreate()
	{
		if (parentObjectName == Selection.activeTransform?.name)
			parentObjectName = "New " + parentObjectName;
		GameObject parentGO = new GameObject(parentObjectName);
		parentGO.layer = LayerMask.NameToLayer("Beatmap");
		parentGO.tag = "Beatmap";
		parentGO.transform.position = firstNotePlacement;
		parentGO.transform.localScale = size;
		parentGO.layer = LayerMask.NameToLayer("Beatmap");
		parentGO.tag = "Beatmap";
		var notes = new NoteScript[numberOfNotesToGenerate];
		for (int i = 0; i < numberOfNotesToGenerate; i++)
		{
			var curr = new GameObject($"{i} - t={Scroller.FIRST_BEAT_TIME + i * Scroller.SECONDS_PER_BEAT}s");
			curr.layer = LayerMask.NameToLayer("Beatmap");
			//Debug.Log($"Beatmap Layer Index: {LayerMask.NameToLayer("Beatmap")}");
			curr.tag = "Beatmap";
			curr.transform.parent = parentGO.transform;
			curr.transform.localPosition = new Vector3(i * spaceBetweenNotes, 0 , 0);

			var box = curr.AddComponent<BoxCollider2D>();
			box.size = colliderSize;
			box.offset = colliderOffset;
			box.isTrigger = true;

			var note = curr.AddComponent<NoteScript>();
			notes[i] = note;
			note.collider = box;
			if (i > 0)
				notes[i - 1].nextNote = note;

			var sRend = curr.AddComponent<SpriteRenderer>();
			sRend.sprite = sprite;
		}
	}

	//void OnWizardOtherButton()
	//{

	//}
}
