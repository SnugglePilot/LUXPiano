using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace LUX
{
	public class SFZParserContext
	{
		[MenuItem("Assets/Parse SFZ File")]
		static void EditMyTypes()
		{
			float startTime = Time.realtimeSinceStartup;
			string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			Debug.Log("■ SFZ Parsing starting...");
			List<MidiAsset> assets = SFZParser.Parse(assetPath);
			Debug.Log("■ SFZ Parsing completed successfully. Total regions: " + assets.Count);

			if (assets != null && assets.Count > 0)
			{
				Debug.Log("■ Starting file creation...");
				string originalDirectory = Path.GetDirectoryName(assetPath);
				AssetDatabase.CreateFolder(originalDirectory, "ScriptableObjects");
				string soDirName = originalDirectory + "/ScriptableObjects";

				SoundBank bank = ScriptableObject.CreateInstance<SoundBank>();
				bank.clips = new List<MidiAsset>();
				
				int index = 0;
				foreach (var asset in assets)
				{
					string audioClipPath = Path.Combine(originalDirectory, asset.sample);
					asset.audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);
					
					string assetFilename = soDirName + "/MidiAsset_" + index.ToString() + ".asset";
					AssetDatabase.CreateAsset(asset, assetFilename);
					bank.clips.Add(asset);
					index++;
				}

				string bankFilename = originalDirectory + "/SoundBank.asset";
				AssetDatabase.CreateAsset(bank, bankFilename);
				AssetDatabase.SaveAssets();
				Debug.Log("■ File creation complete.");				
			}

			float endTime = Time.realtimeSinceStartup;
			Debug.Log("■ Elapsed time: "+Mathf.RoundToInt(endTime - startTime)+"s");
		}

		// This method makes sure the menu item above is only active for objects of appropriate type:
		[MenuItem("Assets/Parse SFZ File", true)]
		static bool EditMyType()
		{
			return AssetDatabase.GetAssetPath(Selection.activeObject).Contains(".sfz");
		}

	}
}
