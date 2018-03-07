using System;
using System.Collections;
using System.Collections.Generic;
using MidiJack;
using UnityEngine;

namespace LUX
{
	public class MidiInput : MonoBehaviour
	{
		public bool logEvents = false;
		public bool resizeKeysToRange = false;
		
		protected Keys luxKeys;
		[ReadOnly] public int minKey = int.MaxValue;
		[ReadOnly] public int maxKey = int.MinValue;

		protected void OnEnable()
		{
			MidiMaster.noteOnDelegate += OnNoteOn;
			MidiMaster.noteOffDelegate += OnNoteOff;
			luxKeys = GetComponentInChildren<Keys>();
		}

		protected void OnDisable()
		{
			MidiMaster.noteOnDelegate -= OnNoteOn;
			MidiMaster.noteOffDelegate -= OnNoteOff;
		}

		private void OnNoteOn(MidiChannel channel, int note, float velocity)
		{
			Log("[NoteOn] channel: " + channel.ToString() + " note: " + note + " velocity: " + velocity);
			CheckNoteRange(note);
			luxKeys.OnKeyPressed(note, velocity);
		}

		private void OnNoteOff(MidiChannel channel, int note)
		{
			Log("[NoteOff] channel: " + channel.ToString() + " note: " + note);
			CheckNoteRange(note);
			luxKeys.OnKeyReleased(note);
		}

		private void CheckNoteRange(int note)
		{
			bool rangeChanged = false;
			if (note < minKey)
			{
				minKey = note;
				rangeChanged = true;
			}

			if (note > maxKey)
			{
				maxKey = note;
				rangeChanged = true;
			}

			if (rangeChanged)
			{
				if (resizeKeysToRange)
				{
					luxKeys.startNote = minKey;
					luxKeys.endNote = maxKey;
					luxKeys.CreateKeys();
				}
			}
		}

		protected void Log(string message)
		{
			if (logEvents)
			{
				Debug.Log(message);
			}
		}

	}
}
