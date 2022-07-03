using System;
using UnityEngine;

[Flags]
public enum PhaseType
{
	Unphased = 0b0000_0000,
	Phased = 0b1111_1111,
}

[RequireComponent(
	typeof(SpriteRenderer)
	)]
public class Phaser : MonoBehaviour
{
	private SpriteRenderer sr;

	public PhaseType phaseType;

	Scroller scroller;

	private void Reset()
	{
		sr = GetComponent<SpriteRenderer>();
		scroller = FindObjectOfType<Scroller>();
	}

	// Start is called before the first frame update
	void Start()
    {
		sr ??= GetComponent<SpriteRenderer>();
		scroller ??= FindObjectOfType<Scroller>();

		scroller.PhaseChange += OnPhaseChange;
	}

	private void OnPhaseChange(object sender, System.EventArgs e)
	{
		//sr.material = (scroller.CurrentPhaseType == phaseType) ?
		//	scroller.unphasedMaterial :
		//	scroller.phasedMaterial;
		sr.sprite = (scroller.CurrentPhaseType == phaseType) ?
			scroller.unphasedNoteSprite :
			scroller.phasedNoteSprite;
		//Debug.Log($"CurrPhaseType {scroller.CurrentPhaseType} & PhaseType {phaseType}");
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
