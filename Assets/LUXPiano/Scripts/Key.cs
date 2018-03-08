using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
	public class Key : MonoBehaviour
	{
		[Tooltip("Used for spacing against other keys")]
		public float thisWidth;
		
		[Tooltip("How many degrees should the key move when pressed?")]
		public Vector3 pressTraversalDegrees = new Vector3(-3.5f, 0, 0);

		[Tooltip("Is the key currently being held?")] [ReadOnly]
		public bool isPressed = false;

		// Store the rotation vectors locally:
		protected Vector3 staticRotation;
		protected Playback playback;

		public int note { get; protected set; }

		public void SetupKey(int noteNumber, Playback playbackReference)
		{
			note = noteNumber;
			playback = playbackReference;
			gameObject.name = "LUX.Key (" + ((Note) Notes.GetNoteFromOctive(note)).ToString() + ")";
			
			// Set the rotation here because this is after instantiation/setup. Keypress could still happen this tick.
			staticRotation = transform.localEulerAngles;
		}

		public void OnPressed(float velocity)
		{
			//Debug.Log(gameObject.name + " Pressed, velocity: "+velocity);
			if (isPressed)
			{
				// Debug.LogWarning("Key can't be double pressed.", gameObject);
				// nb: This can happen if you have more than one keyboard plugged in so COULD happen. let's just ignore.
				return;
			}

			isPressed = true;
			transform.localEulerAngles = staticRotation + pressTraversalDegrees;
			
			playback.PlayNote(note, velocity);
		}

		public void OnReleased()
		{
			//Debug.Log(gameObject.name + " Released");
			if (!isPressed)
			{
				//Debug.LogWarning("Key can't be double unpressed.");
				// nb: This can happen if you have more than one keyboard plugged in so COULD happen. let's just ignore.
				return;
			}

			isPressed = false;
			transform.localEulerAngles = staticRotation;
		}
	}
}
