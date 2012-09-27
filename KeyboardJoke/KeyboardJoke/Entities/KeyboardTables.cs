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
            CharToKeyStrokeTable[0x30] = (ushort)USBH_Key.D0;

            // Everything else.
            CharToKeyStrokeTable[0x20] = (ushort)USBH_Key.Space;
            CharToKeyStrokeTable[0x21] = (ushort)((int)USBH_Key.D1 | ShiftModifier);
            CharToKeyStrokeTable[0x22] = (ushort)((int)USBH_Key.Quotes | ShiftModifier);
            CharToKeyStrokeTable[0x23] = (ushort)((int)USBH_Key.D3 | ShiftModifier);
            CharToKeyStrokeTable[0x24] = (ushort)((int)USBH_Key.D4 | ShiftModifier);
            CharToKeyStrokeTable[0x25] = (ushort)((int)USBH_Key.D5 | ShiftModifier);
            CharToKeyStrokeTable[0x26] = (ushort)((int)USBH_Key.D7 | ShiftModifier);
            CharToKeyStrokeTable[0x27] = (ushort)USBH_Key.Quotes;
            CharToKeyStrokeTable[0x28] = (ushort)((int)USBH_Key.D9 | ShiftModifier);
            CharToKeyStrokeTable[0x29] = (ushort)((int)USBH_Key.D0 | ShiftModifier);
            CharToKeyStrokeTable[0x2a] = (ushort)((int)USBH_Key.D8 | ShiftModifier);
            CharToKeyStrokeTable[0x2b] = (ushort)((int)USBH_Key.Equal | ShiftModifier);
            CharToKeyStrokeTable[0x2c] = (ushort)USBH_Key.Comma;
            CharToKeyStrokeTable[0x2d] = (ushort)USBH_Key.Substract;
            CharToKeyStrokeTable[0x2e] = (ushort)USBH_Key.Period;
            CharToKeyStrokeTable[0x2f] = (ushort)USBH_Key.Divide;

            CharToKeyStrokeTable[0x3a] = (ushort)((int)USBH_Key.Semicolon | ShiftModifier);
            CharToKeyStrokeTable[0x3b] = (ushort)USBH_Key.Semicolon;
            CharToKeyStrokeTable[0x3c] = (ushort)((int)USBH_Key.Comma | ShiftModifier);
            CharToKeyStrokeTable[0x3d] = (ushort)USBH_Key.Equal;
            CharToKeyStrokeTable[0x3e] = (ushort)((int)USBH_Key.Period | ShiftModifier);
            CharToKeyStrokeTable[0x3f] = (ushort)((int)USBH_Key.Divide | ShiftModifier);
            CharToKeyStrokeTable[0x40] = (ushort)((int)USBH_Key.D2 | ShiftModifier);

            CharToKeyStrokeTable[0x5b] = (ushort)USBH_Key.OpenBrackets;
            CharToKeyStrokeTable[0x5c] = (ushort)USBH_Key.Backslash;
            CharToKeyStrokeTable[0x5d] = (ushort)USBH_Key.CloseBrackets;
            CharToKeyStrokeTable[0x5e] = (ushort)((int)USBH_Key.D7 | ShiftModifier);
            CharToKeyStrokeTable[0x5f] = (ushort)((int)USBH_Key.Substract | ShiftModifier);
            CharToKeyStrokeTable[0x60] = (ushort)USBH_Key.GraveAccent;

            CharToKeyStrokeTable[0x5b] = (ushort)((int)USBH_Key.OpenBrackets | ShiftModifier);
            CharToKeyStrokeTable[0x5c] = (ushort)((int)USBH_Key.Backslash | ShiftModifier);
            CharToKeyStrokeTable[0x5d] = (ushort)((int)USBH_Key.CloseBrackets | ShiftModifier);
            CharToKeyStrokeTable[0x5e] = (ushort)((int)USBH_Key.GraveAccent | ShiftModifier);


            // Fill the unshifted (lower case) keystroke -> char table.
            KeyToCharTable = new char[256];
            for (int i = 0x61; i < 0x7a; i++)
                // Letters in lower case.
                KeyToCharTable[i - 0x5d] = (char)i;
            for (int i = 0x31; i < 0x39; i++)
                // Numbers 1..9.
                KeyToCharTable[i - 0x13] = (char)i;
            KeyToCharTable[(int)USBH_Key.D0] = (char)0x30;

            KeyToCharTable[(int)USBH_Key.Substract] = (char)0x2d;
            KeyToCharTable[(int)USBH_Key.Equal] = (char)0x3d;
            KeyToCharTable[(int)USBH_Key.OpenBrackets] = (char)0x5b;
            KeyToCharTable[(int)USBH_Key.CloseBrackets] = (char)0x5d;
            KeyToCharTable[(int)USBH_Key.Backslash] = (char)0x5c;
            KeyToCharTable[(int)USBH_Key.Semicolon] = (char)0x3b;
            KeyToCharTable[(int)USBH_Key.Quotes] = (char)0x27;
            KeyToCharTable[(int)USBH_Key.GraveAccent] = (char)0x60;
            KeyToCharTable[(int)USBH_Key.Comma] = (char)0x2c;
            KeyToCharTable[(int)USBH_Key.Period] = (char)0x2e;
            KeyToCharTable[(int)USBH_Key.Divide] = (char)0x2f;

            // Keypad keys.
            KeyToCharTable[(int)USBH_Key.Keypad_Divide] = (char)0x2f;
            KeyToCharTable[(int)USBH_Key.Keypad_Multiply] = (char)0x2a;
            KeyToCharTable[(int)USBH_Key.Keypad_Substract] = (char)0x2d;
            KeyToCharTable[(int)USBH_Key.Keypad_Add] = (char)0x2b;
            // TODO: handle numlock keys better??
            KeyToCharTable[(int)USBH_Key.Keypad_D0] = (char)0x30;
            KeyToCharTable[(int)USBH_Key.Keypad_D1] = (char)0x31;
            KeyToCharTable[(int)USBH_Key.Keypad_D2] = (char)0x32;
            KeyToCharTable[(int)USBH_Key.Keypad_D3] = (char)0x33;
            KeyToCharTable[(int)USBH_Key.Keypad_D4] = (char)0x34;
            KeyToCharTable[(int)USBH_Key.Keypad_D5] = (char)0x35;
            KeyToCharTable[(int)USBH_Key.Keypad_D6] = (char)0x36;
            KeyToCharTable[(int)USBH_Key.Keypad_D7] = (char)0x37;
            KeyToCharTable[(int)USBH_Key.Keypad_D8] = (char)0x38;
            KeyToCharTable[(int)USBH_Key.Keypad_D9] = (char)0x39;



            // Fill the shifted (upper case) keystroke -> char table.
            ShiftedKeyToCharTable = new char[256];
            for (int i = 0x41; i < 0x5a; i++)
                // Letters in upper case.
                ShiftedKeyToCharTable[i - 0x3d] = (char)i;

            ShiftedKeyToCharTable[(int)USBH_Key.Substract] = (char)0x5f;
            ShiftedKeyToCharTable[(int)USBH_Key.Equal] = (char)0x2b;
            ShiftedKeyToCharTable[(int)USBH_Key.OpenBrackets] = (char)0x7b;
            ShiftedKeyToCharTable[(int)USBH_Key.CloseBrackets] = (char)0x7d;
            ShiftedKeyToCharTable[(int)USBH_Key.Backslash] = (char)0x7c;
            ShiftedKeyToCharTable[(int)USBH_Key.Semicolon] = (char)0x3a;
            ShiftedKeyToCharTable[(int)USBH_Key.Quotes] = (char)0x22;
            ShiftedKeyToCharTable[(int)USBH_Key.GraveAccent] = (char)0x7e;
            ShiftedKeyToCharTable[(int)USBH_Key.Comma] = (char)0x3c;
            ShiftedKeyToCharTable[(int)USBH_Key.Period] = (char)0x3e;
            ShiftedKeyToCharTable[(int)USBH_Key.Divide] = (char)0x3f;

            ShiftedKeyToCharTable[(int)USBH_Key.D1] = (char)0x21;
            ShiftedKeyToCharTable[(int)USBH_Key.D2] = (char)0x40;
            ShiftedKeyToCharTable[(int)USBH_Key.D3] = (char)0x23;
            ShiftedKeyToCharTable[(int)USBH_Key.D4] = (char)0x24;
            ShiftedKeyToCharTable[(int)USBH_Key.D5] = (char)0x25;
            ShiftedKeyToCharTable[(int)USBH_Key.D6] = (char)0x5e;
            ShiftedKeyToCharTable[(int)USBH_Key.D7] = (char)0x26;
            ShiftedKeyToCharTable[(int)USBH_Key.D8] = (char)0x2a;
            ShiftedKeyToCharTable[(int)USBH_Key.D9] = (char)0x28;
            ShiftedKeyToCharTable[(int)USBH_Key.D0] = (char)0x29;
        }


        public static char KeyToChar(byte key)
        {
            return KeyToChar(key, false);
        }
        public static char KeyToChar(byte key, bool isShifted)
        {
            var asInt = (int)key;
            if (isShifted)
                return ShiftedKeyToCharTable[asInt];
            else
                return KeyToCharTable[asInt];
        }
        

        public static ushort CreateCapitalisedKeyStroke(byte key)
        {
            return (ushort)(key | ShiftModifier);
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

        public static bool IsModifierKey(byte key)
        {
            return key == (byte)USBH_Key.LeftShift || key == (byte)USBH_Key.RightShift
                || key == (byte)USBH_Key.LeftCtrl  || key == (byte)USBH_Key.RightCtrl
                || key == (byte)USBH_Key.LeftAlt   || key == (byte)USBH_Key.RightAlt;
        }
        public static bool IsShiftKey(byte key)
        {
            return key == (byte)USBH_Key.LeftShift || key == (byte)USBH_Key.RightShift;
        }
        public static bool IsControlKey(byte key)
        {
            return key == (byte)USBH_Key.LeftCtrl || key == (byte)USBH_Key.RightCtrl;
        }
        public static bool IsAltKey(byte key)
        {
            return key == (byte)USBH_Key.LeftAlt || key == (byte)USBH_Key.RightAlt;
        }
    }
}

