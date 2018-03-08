using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LUX
{
    public class Keys : MonoBehaviour
    {
        [Tooltip("Prefab for spawning piano keys")]
        public Key keyPrefab;
        public Key keyPrefabSharp;

        public bool spawnKeysOnEnable = true;

        [Tooltip("MIDI specified note numbers for keyboard layout. eg: Middle C is 60")]
        // http://computermusicresource.com/midikeys.html
        public int startNote = 21;
        public int endNote = 108;

        public int numKeys
        {
            get
            {
                if (startNote > endNote)
                {
                    return 0;
                }
                return endNote - startNote + 1;
            }
        }

        protected Dictionary<int, Key> keys = new Dictionary<int, Key>();
        protected Playback playback;

        protected void OnEnable()
        {
            playback = GetComponentInParent<Playback>();
            
            if (spawnKeysOnEnable)
            {
                CreateKeys();
            }
        }

        /// <summary>
        /// Destroys all keys that we've previously created.
        /// </summary>
        public void DestroyKeys()
        {
            foreach (KeyValuePair<int, Key> entry in keys)
            {
                Destroy(entry.Value.gameObject);
            }

            keys.Clear();
        }

        /// <summary>
        /// Creates a whole new set of keys.
        /// </summary>
        public void CreateKeys()
        {
            // clear out the old:
            DestroyKeys();

            if (numKeys <= 0)
            {
                Debug.LogError("Can't make <= 0 keys.");
                return;
            }

            int note = startNote;
            Vector3 spawnPosition = transform.position;
            Key lastKey = null;
            while (note <= endNote)
            {
                if (lastKey != null)
                {
                    // black keys have a thisWidth of zero.
                    spawnPosition += transform.right * lastKey.thisWidth;
                }
                lastKey = CreateKey(note, spawnPosition, transform.rotation);
                note++;
            }
        }

        private Key CreateKey(int note, Vector3 position, Quaternion rotation)
        {
            Key prefab;
            if (Notes.IsSharp(note))
            {
                prefab = keyPrefabSharp;
            }
            else
            {
                prefab = keyPrefab;
            }
            
            Key key = Instantiate<Key>(prefab, position, rotation);
            key.transform.SetParent(transform, true);
            key.SetupKey(note, playback);
            keys.Add(note, key);
            return key;
        }

        public void OnKeyPressed(int note, float velocity)
        {
            if (keys.ContainsKey(note))
            {
                OnKeyPressed(keys[note], velocity);
            }
        }

        public void OnKeyPressed(Key key, float velocity)
        {
            key.OnPressed(velocity);
        }

        public void OnKeyReleased(int note)
        {
            if (keys.ContainsKey(note))
            {
                OnKeyReleased(keys[note]);
            }
        }

        public void OnKeyReleased(Key key)
        {
            key.OnReleased();
        }

        public void OnClumsyKeyPressed(Key key, float velocity)
        {
            // This function tries to make a "nice" chord for you
            int keyNote = key.note;
            OnKeyPressed(key, velocity);
            OnKeyPressed(keyNote + 4, velocity);
            OnKeyPressed(keyNote + 4 + 3, velocity);
        }

        public void ReleaseAll()
        {
            foreach (KeyValuePair<int, Key> entry in keys)
            {
                entry.Value.OnReleased();
            }
        }
    }
}
