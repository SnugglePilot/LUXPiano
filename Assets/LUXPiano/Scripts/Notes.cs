using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LUX
{
    public enum Note : int
    {
        C = 0,
        Cs = 1,
        D = 2,
        Ds = 3,
        E = 4,
        F = 5,
        Fs = 6,
        G = 7,
        Gs = 8,
        A = 9,
        As = 10,
        B = 11
    }

    public class Notes
    {
        public static bool IsSharp(int note)
        {
            if (IsNote(note, Note.Cs) ||
                IsNote(note, Note.Ds) ||
                IsNote(note, Note.Fs) ||
                IsNote(note, Note.Gs) ||
                IsNote(note, Note.As))
            {
                return true;
            }

            return false;
        }

        public static bool IsNotSharp(int note)
        {
            return !IsSharp(note);
        }

        public static bool IsNote(int note, Note comparitive)
        {
            note = GetNoteFromOctive(note);
            int compareValue = (int) comparitive;
            if (compareValue == note)
            {
                return true;
            }
            return false;
        }

        public static int GetNoteFromOctive(int note)
        {
            while (note >= 12)
            {
                note -= 12;
            }

            return note;
        }
    }
}
