using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
    [CreateAssetMenu(fileName = "MidiAsset", menuName = "LUX/SoundBank", order = 1)]
    // http://www.sfzformat.com/legacy/
    public class MidiAsset : ScriptableObject
    {
        public int hivel = -1;
        public int lovel = -1; // for note to play, audio must be between lovel and hivel velocity. // 0..127

        public int lokey = -1;
        public int hikey = -1; // for note to play, note played must be between lokey and hikey. // 0..127
        public int pitch_keycenter = -1; // root key. why is this one thing going to -127?? // -127..127

        public int key = -1; // key autoreplaces lokey, hikey, and pitch_keycenter with this value. // 0..127

        public string sample; // File to play
        public float ampeg_release = -1; // Amplifier EG release time (after note release), in seconds. // 0..100

        public float amp_veltrack = -1; // Amplifier velocity tracking, represents how much the amplitude changes with incoming note velocity. // 0..100 [%]

        public AudioClip audioClip;

        public bool IsInNormalizedVelocityRange(float velocity)
        {
            float upperLimit = 1;
            float lowerLimit = 0;
            if (hivel > -1)
            {
                upperLimit = hivel / 127.0f;
            }

            if (lovel > -1)
            {
                lowerLimit = lovel / 127.0f;
            }

            return velocity >= lowerLimit && velocity <= upperLimit;
        }

        public bool IsCandidate(int note, float velocity)
        {
            if (!IsInNormalizedVelocityRange(velocity))
            {
                return false;
            }

            return key == note || pitch_keycenter == note || (note > lokey && note < hikey);
        }
    }
}