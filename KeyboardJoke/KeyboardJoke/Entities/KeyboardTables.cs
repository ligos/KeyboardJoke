using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBHost;

namespace MurrayGrant.KeyboardJoke.Entities
{
    /// <summary>
    /// Various static tables used to translate keystrokes to and from strings.
    /// </summary>
    public static class KeyboardTables
    {
        public const ushort ShiftModifier = 0x0100;
        public const ushort AltModifier = 0x0200;
        public const ushort ControlModifier = 0x0400;
        public const ushort InvalidFlag = 0x8000;

        public static readonly char[] EndOfSentenceCharacters = new char[] { '.', '?', '!' };
        public static readonly ushort[] CharToKeyStrokeTable;
        public static readonly char[] KeyToCharTable;
        public static readonly char[] ShiftedKeyToCharTable;

        static KeyboardTables()
        {
            // Fill the char -> keystroke table.
            CharToKeyStrokeTable = new ushort[256];
            for (int i = 0; i < CharToKeyStrokeTable.Length; i++)
                CharToKeyStrokeTable[i] = InvalidFlag;
            for (int i = 4; i < 29; i++)
            {
                // Letters in upper and lower case.
                CharToKeyStrokeTable[i + 0x3d] = (ushort)(i | ShiftModifier);     // Upper case letters
                CharToKeyStrokeTable[i + 0x5d] = (ushort)i;     // Lower case letters.
            }
            for (int i = 30; i < 38; i++)
                // Numbers 1..9.
                CharToKeyStrokeTable[i + 0x13] = (ushort)i;
            CharToKeyStrokeTable[0x30] = (ushort)KeyboardKey.D0;

            // Everything else.
            CharToKeyStrokeTable[0x20] = (ushort)KeyboardKey.Space;
            CharToKeyStrokeTable[0x21] = (ushort)((int)KeyboardKey.D1 | ShiftModifier);
            CharToKeyStrokeTable[0x22] = (ushort)((int)KeyboardKey.Quotes | ShiftModifier);
            CharToKeyStrokeTable[0x23] = (ushort)((int)KeyboardKey.D3 | ShiftModifier);
            CharToKeyStrokeTable[0x24] = (ushort)((int)KeyboardKey.D4 | ShiftModifier);
            CharToKeyStrokeTable[0x25] = (ushort)((int)KeyboardKey.D5 | ShiftModifier);
            CharToKeyStrokeTable[0x26] = (ushort)((int)KeyboardKey.D7 | ShiftModifier);
            CharToKeyStrokeTable[0x27] = (ushort)KeyboardKey.Quotes;
            CharToKeyStrokeTable[0x28] = (ushort)((int)KeyboardKey.D9 | ShiftModifier);
            CharToKeyStrokeTable[0x29] = (ushort)((int)KeyboardKey.D0 | ShiftModifier);
            CharToKeyStrokeTable[0x2a] = (ushort)((int)KeyboardKey.D8 | ShiftModifier);
            CharToKeyStrokeTable[0x2b] = (ushort)((int)KeyboardKey.Equal | ShiftModifier);
            CharToKeyStrokeTable[0x2c] = (ushort)KeyboardKey.Comma;
            CharToKeyStrokeTable[0x2d] = (ushort)KeyboardKey.Substract;
            CharToKeyStrokeTable[0x2e] = (ushort)KeyboardKey.Period;
            CharToKeyStrokeTable[0x2f] = (ushort)KeyboardKey.Divide;

            CharToKeyStrokeTable[0x3a] = (ushort)((int)KeyboardKey.Semicolon | ShiftModifier);
            CharToKeyStrokeTable[0x3b] = (ushort)KeyboardKey.Semicolon;
            CharToKeyStrokeTable[0x3c] = (ushort)((int)KeyboardKey.Comma | ShiftModifier);
            CharToKeyStrokeTable[0x3d] = (ushort)KeyboardKey.Equal;
            CharToKeyStrokeTable[0x3e] = (ushort)((int)KeyboardKey.Period | ShiftModifier);
            CharToKeyStrokeTable[0x3f] = (ushort)((int)KeyboardKey.Divide | ShiftModifier);
            CharToKeyStrokeTable[0x40] = (ushort)((int)KeyboardKey.D2 | ShiftModifier);

            CharToKeyStrokeTable[0x5b] = (ushort)KeyboardKey.OpenBrackets;
            CharToKeyStrokeTable[0x5c] = (ushort)KeyboardKey.Backslash;
            CharToKeyStrokeTable[0x5d] = (ushort)KeyboardKey.CloseBrackets;
            CharToKeyStrokeTable[0x5e] = (ushort)((int)KeyboardKey.D7 | ShiftModifier);
            CharToKeyStrokeTable[0x5f] = (ushort)((int)KeyboardKey.Substract | ShiftModifier);
            CharToKeyStrokeTable[0x60] = (ushort)KeyboardKey.GraveAccent;

            CharToKeyStrokeTable[0x5b] = (ushort)((int)KeyboardKey.OpenBrackets | ShiftModifier);
            CharToKeyStrokeTable[0x5c] = (ushort)((int)KeyboardKey.Backslash | ShiftModifier);
            CharToKeyStrokeTable[0x5d] = (ushort)((int)KeyboardKey.CloseBrackets | ShiftModifier);
            CharToKeyStrokeTable[0x5e] = (ushort)((int)KeyboardKey.GraveAccent | ShiftModifier);


            // Fill the unshifted (lower case) keystroke -> char table.
            KeyToCharTable = new char[256];
            for (int i = 0x61; i < 0x7a; i++)
                // Letters in lower case.
                KeyToCharTable[i - 0x5d] = (char)i;
            for (int i = 0x31; i < 0x39; i++)
                // Numbers 1..9.
                KeyToCharTable[i - 0x13] = (char)i;
            KeyToCharTable[(int)KeyboardKey.D0] = (char)0x30;

            KeyToCharTable[(int)KeyboardKey.Substract] = (char)0x2d;
            KeyToCharTable[(int)KeyboardKey.Equal] = (char)0x3d;
            KeyToCharTable[(int)KeyboardKey.OpenBrackets] = (char)0x5b;
            KeyToCharTable[(int)KeyboardKey.CloseBrackets] = (char)0x5d;
            KeyToCharTable[(int)KeyboardKey.Backslash] = (char)0x5c;
            KeyToCharTable[(int)KeyboardKey.Semicolon] = (char)0x3b;
            KeyToCharTable[(int)KeyboardKey.Quotes] = (char)0x27;
            KeyToCharTable[(int)KeyboardKey.GraveAccent] = (char)0x60;
            KeyToCharTable[(int)KeyboardKey.Comma] = (char)0x2c;
            KeyToCharTable[(int)KeyboardKey.Period] = (char)0x2e;
            KeyToCharTable[(int)KeyboardKey.Divide] = (char)0x2f;

            // Keypad keys.
            KeyToCharTable[(int)KeyboardKey.Keypad_Divide] = (char)0x2f;
            KeyToCharTable[(int)KeyboardKey.Keypad_Multiply] = (char)0x2a;
            KeyToCharTable[(int)KeyboardKey.Keypad_Substract] = (char)0x2d;
            KeyToCharTable[(int)KeyboardKey.Keypad_Add] = (char)0x2b;
            // TODO: handle numlock keys better??
            KeyToCharTable[(int)KeyboardKey.Keypad_D0] = (char)0x30;
            KeyToCharTable[(int)KeyboardKey.Keypad_D1] = (char)0x31;
            KeyToCharTable[(int)KeyboardKey.Keypad_D2] = (char)0x32;
            KeyToCharTable[(int)KeyboardKey.Keypad_D3] = (char)0x33;
            KeyToCharTable[(int)KeyboardKey.Keypad_D4] = (char)0x34;
            KeyToCharTable[(int)KeyboardKey.Keypad_D5] = (char)0x35;
            KeyToCharTable[(int)KeyboardKey.Keypad_D6] = (char)0x36;
            KeyToCharTable[(int)KeyboardKey.Keypad_D7] = (char)0x37;
            KeyToCharTable[(int)KeyboardKey.Keypad_D8] = (char)0x38;
            KeyToCharTable[(int)KeyboardKey.Keypad_D9] = (char)0x39;



            // Fill the shifted (upper case) keystroke -> char table.
            ShiftedKeyToCharTable = new char[256];
            for (int i = 0x41; i < 0x5a; i++)
                // Letters in upper case.
                ShiftedKeyToCharTable[i - 0x3d] = (char)i;

            ShiftedKeyToCharTable[(int)KeyboardKey.Substract] = (char)0x5f;
            ShiftedKeyToCharTable[(int)KeyboardKey.Equal] = (char)0x2b;
            ShiftedKeyToCharTable[(int)KeyboardKey.OpenBrackets] = (char)0x7b;
            ShiftedKeyToCharTable[(int)KeyboardKey.CloseBrackets] = (char)0x7d;
            ShiftedKeyToCharTable[(int)KeyboardKey.Backslash] = (char)0x7c;
            ShiftedKeyToCharTable[(int)KeyboardKey.Semicolon] = (char)0x3a;
            ShiftedKeyToCharTable[(int)KeyboardKey.Quotes] = (char)0x22;
            ShiftedKeyToCharTable[(int)KeyboardKey.GraveAccent] = (char)0x7e;
            ShiftedKeyToCharTable[(int)KeyboardKey.Comma] = (char)0x3c;
            ShiftedKeyToCharTable[(int)KeyboardKey.Period] = (char)0x3e;
            ShiftedKeyToCharTable[(int)KeyboardKey.Divide] = (char)0x3f;

            ShiftedKeyToCharTable[(int)KeyboardKey.D1] = (char)0x21;
            ShiftedKeyToCharTable[(int)KeyboardKey.D2] = (char)0x40;
            ShiftedKeyToCharTable[(int)KeyboardKey.D3] = (char)0x23;
            ShiftedKeyToCharTable[(int)KeyboardKey.D4] = (char)0x24;
            ShiftedKeyToCharTable[(int)KeyboardKey.D5] = (char)0x25;
            ShiftedKeyToCharTable[(int)KeyboardKey.D6] = (char)0x5e;
            ShiftedKeyToCharTable[(int)KeyboardKey.D7] = (char)0x26;
            ShiftedKeyToCharTable[(int)KeyboardKey.D8] = (char)0x2a;
            ShiftedKeyToCharTable[(int)KeyboardKey.D9] = (char)0x28;
            ShiftedKeyToCharTable[(int)KeyboardKey.D0] = (char)0x29;
        }


        public static char KeyToChar(KeyboardKey key)
        {
            return KeyToChar(key, false);
        }
        public static char KeyToChar(KeyboardKey key, bool isShifted)
        {
            var asInt = (int)key;
            if (isShifted)
                return ShiftedKeyToCharTable[asInt];
            else
                return KeyToCharTable[asInt];
        }


        public static ushort CreateCapitalisedKeyStroke(KeyboardKey key)
        {
            return (ushort)((int)key | ShiftModifier);
        }

        public static ushort CharToKeyStroke(char c)
        {
            var asInt = (int)c;
            return CharToKeyStrokeTable[asInt];
        }
        public static ushort[] StringToKeyStrokes(string s)
        {
            var result = new ushort[s.Length];
            for (int i = 0; i < s.Length; i++)
                result[i] = CharToKeyStroke(s[i]);
            return result;
        }

        public static bool IsModifierKey(KeyboardKey key)
        {
            return key == KeyboardKey.LeftShift || key == KeyboardKey.RightShift
                || key == KeyboardKey.LeftCtrl || key == KeyboardKey.RightCtrl
                || key == KeyboardKey.LeftAlt || key == KeyboardKey.RightAlt;
        }
        public static bool IsShiftKey(KeyboardKey key)
        {
            return key == KeyboardKey.LeftShift || key == KeyboardKey.RightShift;
        }
        public static bool IsControlKey(KeyboardKey key)
        {
            return key == KeyboardKey.LeftCtrl || key == KeyboardKey.RightCtrl;
        }
        public static bool IsAltKey(KeyboardKey key)
        {
            return key == KeyboardKey.LeftAlt || key == KeyboardKey.RightAlt;
        }
    }
}

