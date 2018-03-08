using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
	public class Playback : MonoBehaviour
	{
		public SoundBank soundBank;
		
		public int numOutputs;
		public AudioSource outputPrefab;

		protected AudioSource[] outputs;
		protected int outputIndex = 0;
		
		void Start()
		{
			outputs = new AudioSource[numOutputs];
			for (int i = 0; i < numOutputs; i++)
			{
				AudioSource newSource = Instantiate<AudioSource>(outputPrefab);
				newSource.transform.SetParent(transform);
				newSource.transform.localPosition = Vector3.zero;
				outputs[i] = newSource;
			}
		}

		public void PlayNote(int note, float velocity)
		{
			AudioClip clip = soundBank.FetchClip(note, velocity);
			outputs[outputIndex].clip = clip;
			outputs[outputIndex].Play();
			outputIndex++;
			if (outputIndex >= outputs.Length)
			{
				outputIndex = 0;
			}
		}
	}
}
