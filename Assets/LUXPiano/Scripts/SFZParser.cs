using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LUX
{
    public class SFZParser
    {
        // http://www.sfzformat.com/legacy/
        public static List<MidiAsset> Parse(string assetPath)
        {
            StreamReader reader = new StreamReader(assetPath);

            MidiAsset currentGroup = null;
            MidiAsset currentMidiAsset = null;
            bool isEditingGroup = false;

            List<MidiAsset> regions = new List<MidiAsset>();

            while (!reader.EndOfStream)
            {
                string nextLine = reader.ReadLine();
                nextLine.Trim();

                if (string.IsNullOrEmpty(nextLine))
                {
                    // blank.
                    continue;
                }

                if (nextLine.Substring(0, 2) == "//")
                {
                    // comment.
                    continue;
                }

                if (nextLine == "<group>")
                {
                    // starting a new group definition. Groups are "defaults" for all following regions.
                    currentGroup = ScriptableObject.CreateInstance<MidiAsset>();
                    isEditingGroup = true;
                    continue;
                }

                if (nextLine == "<region>")
                {
                    // region is a new "note" or play thing
                    if (currentMidiAsset != null)
                    {
                        regions.Add(currentMidiAsset);
                    }

                    currentMidiAsset = ScriptableObject.CreateInstance<MidiAsset>();
                    MergeGroupAndRegion(currentGroup, currentMidiAsset);
                    isEditingGroup = false;
                    continue;
                }

                MidiAsset currentTarget = currentMidiAsset;
                if (isEditingGroup)
                {
                    currentTarget = currentGroup;
                }

                if (currentTarget == null)
                {
                    Debug.LogError("Can't continue without target being defined; " + nextLine);
                    return null;
                }

                if (!nextLine.Contains("="))
                {
                    Debug.LogError("Can't parse value without =; " + nextLine);
                    return null;
                }

                string[] split = nextLine.Split('=');
                string key = split[0];
                string value = split[1];

                var field = currentTarget.GetType().GetField(key);
                if (field == null)
                {
                    Debug.LogError("Can't find field: `" + key + "`");
                    return null;
                }
                else
                {
                    if (field.FieldType == typeof(float))
                    {
                        float newValue = float.Parse(value);
                        field.SetValue(currentTarget, newValue);
                    }
                    else if (field.FieldType == typeof(int))
                    {
                        field.SetValue(currentTarget, ConvertStringToInt(value));
                    }
                    else if (field.FieldType == typeof(string))
                    {
                        field.SetValue(currentTarget, value);
                    }
                    else
                    {
                        Debug.LogError("UNHANDLED BIT: " + nextLine);
                        return null;
                    }

                    continue;
                }
            }

            if (currentMidiAsset != null)
            {
                regions.Add(currentMidiAsset);
            }

            return regions;
        }

        public static int ConvertStringToInt(string input)
        {
            // SFZ can list notes as -127..127, OR a string from C-1..G9 with sharps (eg: a0, a#-1, etc)
            int newValue;
            if (int.TryParse(input, out newValue))
            {
                return newValue;
            }
            // sigh

            string note = input[0].ToString().ToUpper();
            if (input[1] == '#')
            {
                note += "s";
            }

            int noteNumber = (int) ((Note) Enum.Parse(typeof(Note), note, true));

            int octave = 0;
            if (input[input.Length - 1] != '0')
            {
                octave = int.Parse(input.Substring(input.Length - 1));
                if (input.Contains("-"))
                {
                    octave *= -1;
                }
            }

            return noteNumber + ((octave + 1) * 12);
        }

        public static void MergeGroupAndRegion(MidiAsset group, MidiAsset midiAsset)
        {
            if (group == null || midiAsset == null)
            {
                return;
            }

            var type = group.GetType();
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(float))
                {
                    float newValue = (float) field.GetValue(group);
                    if (newValue == -1)
                    {
                        continue;
                    }
                    
                    field.SetValue(midiAsset, newValue);
                }
                else if (field.FieldType == typeof(int))
                {
                    int newValue = (int) field.GetValue(group);
                    if (newValue == -1)
                    {
                        continue;
                    }
                    
                    field.SetValue(midiAsset, newValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    string newValue = (string) field.GetValue(group);
                    if (string.IsNullOrEmpty(newValue))
                    {
                        continue;
                    }
                    
                    field.SetValue(midiAsset, newValue);
                }
                else if (field.FieldType == typeof(AudioClip))
                {
                    // we don't parse these here, handled later.
                    continue;
                } else {
                    Debug.LogError("UNHANDLED");
                    return;
                }
            }
        }
    }
}