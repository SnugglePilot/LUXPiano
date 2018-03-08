using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
	[CreateAssetMenu(fileName = "SoundBank", menuName = "LUX/SoundBank", order = 1)]
	public class SoundBank : ScriptableObject
	{
		public List<MidiAsset> clips;
		
		public AudioClip FetchClip(int note, float velocity)
		{
			foreach (var clip in clips)
			{
				if (clip.IsCandidate(note, velocity))
				{
					return clip.audioClip;
				}
			}

			return null;
		}

	}
}
