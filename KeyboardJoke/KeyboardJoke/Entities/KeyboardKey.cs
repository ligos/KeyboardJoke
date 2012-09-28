using System;
using Microsoft.SPOT;

namespace MurrayGrant.KeyboardJoke.Entities
{
    /// <summary>
    /// Taken from GHIElectronics.NETMF.USBClient.USBC_Key.
    /// </summary>
    public enum KeyboardKey : byte
    {
        // Summary:
        //     No Key is associated.
        None = 0,
        //
        // Summary:
        //     Key: A.
        A = 4,
        //
        // Summary:
        //     Key: B.
        B = 5,
        //
        // Summary:
        //     Key: C.
        C = 6,
        //
        // Summary:
        //     Key: D.
        D = 7,
        //
        // Summary:
        //     Key: E.
        E = 8,
        //
        // Summary:
        //     Key: F.
        F = 9,
        //
        // Summary:
        //     Key: G.
        G = 10,
        //
        // Summary:
        //     Key: H.
        H = 11,
        //
        // Summary:
        //     Key: I.
        I = 12,
        //
        // Summary:
        //     Key: J.
        J = 13,
        //
        // Summary:
        //     Key: K.
        K = 14,
        //
        // Summary:
        //     Key: L.
        L = 15,
        //
        // Summary:
        //     Key: M.
        M = 16,
        //
        // Summary:
        //     Key: N.
        N = 17,
        //
        // Summary:
        //     Key: O.
        O = 18,
        //
        // Summary:
        //     Key: P.
        P = 19,
        //
        // Summary:
        //     Key: Q.
        Q = 20,
        //
        // Summary:
        //     Key: R.
        R = 21,
        //
        // Summary:
        //     Key: S.
        S = 22,
        //
        // Summary:
        //     Key: T.
        T = 23,
        //
        // Summary:
        //     Key: U.
        U = 24,
        //
        // Summary:
        //     Key: V.
        V = 25,
        //
        // Summary:
        //     Key: W.
        W = 26,
        //
        // Summary:
        //     Key: X.
        X = 27,
        //
        // Summary:
        //     Key: Y.
        Y = 28,
        //
        // Summary:
        //     Key: Z.
        Z = 29,
        //
        // Summary:
        //     Key: 1 or !.
        D1 = 30,
        //
        // Summary:
        //     Key: 2 or @.
        D2 = 31,
        //
        // Summary:
        //     Key: 3 or #.
        D3 = 32,
        //
        // Summary:
        //     Key: 4 or $.
        D4 = 33,
        //
        // Summary:
        //     Key: 5 or %.
        D5 = 34,
        //
        // Summary:
        //     Key: 6 or ^.
        D6 = 35,
        //
        // Summary:
        //     Key: 7 or &.
        D7 = 36,
        //
        // Summary:
        //     Key: 8 or *.
        D8 = 37,
        //
        // Summary:
        //     Key: 9 or (.
        D9 = 38,
        //
        // Summary:
        //     Key: 0 or ).
        D0 = 39,
        //
        // Summary:
        //     Key: Enter.
        Enter = 40,
        //
        // Summary:
        //     Key: Esc.
        Escape = 41,
        //
        // Summary:
        //     Key: Backspace.
        BackSpace = 42,
        //
        // Summary:
        //     Key: Tab.
        Tab = 43,
        //
        // Summary:
        //     Key: Space.
        Space = 44,
        //
        // Summary:
        //     Key: - or _.
        Substract = 45,
        //
        // Summary:
        //     Key: = or +.
        Equal = 46,
        //
        // Summary:
        //     Key: [ or {.
        OpenBrackets = 47,
        //
        // Summary:
        //     Key: ] or }.
        CloseBrackets = 48,
        //
        // Summary:
        //     Key: \ or |.
        Backslash = 49,
        //
        // Summary:
        //     Key: Not supported. Non-US keyboard character.
        NON_US = 50,
        //
        // Summary:
        //     Key: ; or :.
        Semicolon = 51,
        //
        // Summary:
        //     Key: ' or ".
        Quotes = 52,
        //
        // Summary:
        //     Key: ` or ~.
        GraveAccent = 53,
        //
        // Summary:
        //     Key: , or <.
        Comma = 54,
        //
        // Summary:
        //     Key: . or >.
        Period = 55,
        //
        // Summary:
        //     Key: / or ?.
        Divide = 56,
        //
        // Summary:
        //     Key: Caps Lock.
        CapsLock = 57,
        //
        // Summary:
        //     Key: F1.
        F1 = 58,
        //
        // Summary:
        //     Key: F2.
        F2 = 59,
        //
        // Summary:
        //     Key: F3.
        F3 = 60,
        //
        // Summary:
        //     Key: F4.
        F4 = 61,
        //
        // Summary:
        //     Key: F5.
        F5 = 62,
        //
        // Summary:
        //     Key: F6.
        F6 = 63,
        //
        // Summary:
        //     Key: F7.
        F7 = 64,
        //
        // Summary:
        //     Key: F8.
        F8 = 65,
        //
        // Summary:
        //     Key: F9.
        F9 = 66,
        //
        // Summary:
        //     Key: F10.
        F10 = 67,
        //
        // Summary:
        //     Key: F11.
        F11 = 68,
        //
        // Summary:
        //     Key: F12.
        F12 = 69,
        //
        // Summary:
        //     Key: Print Screen.
        PrintScreen = 70,
        //
        // Summary:
        //     Key: Scroll Lock.
        ScrollLock = 71,
        //
        // Summary:
        //     Key: Pause.
        Pause = 72,
        //
        // Summary:
        //     Key: Insert.
        Insert = 73,
        //
        // Summary:
        //     Key: Home.
        Home = 74,
        //
        // Summary:
        //     Key: Page Up.
        PageUp = 75,
        //
        // Summary:
        //     Key: Delete.
        Delete = 76,
        //
        // Summary:
        //     Key: End.
        End = 77,
        //
        // Summary:
        //     Key: Page Down.
        PageDown = 78,
        //
        // Summary:
        //     Key: Right Arrow.
        RightArrow = 79,
        //
        // Summary:
        //     Key: Left Arrow.
        LeftArrow = 80,
        //
        // Summary:
        //     Key: Down Arrow.
        DownArrow = 81,
        //
        // Summary:
        //     Key: Up Arrow.
        UpArrow = 82,
        //
        // Summary:
        //     Key: Num Lock.
        NumLock = 83,
        //
        // Summary:
        //     Key: Keypad /.
        Keypad_Divide = 84,
        //
        // Summary:
        //     Key: Keypad *.
        Keypad_Multiply = 85,
        //
        // Summary:
        //     Key: Keypad -.
        Keypad_Substract = 86,
        //
        // Summary:
        //     Key: Keypad +.
        Keypad_Add = 87,
        //
        // Summary:
        //     Key: Keypad Enter.
        Keypad_Enter = 88,
        //
        // Summary:
        //     Key: Keypad 1 or End.
        Keypad_D1 = 89,
        //
        // Summary:
        //     Key: Keypad 2 or Down Arrow.
        Keypad_D2 = 90,
        //
        // Summary:
        //     Key: Keypad 3 or Page Down.
        Keypad_D3 = 91,
        //
        // Summary:
        //     Key: Keypad 4 or Left Arrow.
        Keypad_D4 = 92,
        //
        // Summary:
        //     Key: Keypad 5.
        Keypad_D5 = 93,
        //
        // Summary:
        //     Key: Keypad 6 or Right Arrow.
        Keypad_D6 = 94,
        //
        // Summary:
        //     Key: Keypad 7 or Home.
        Keypad_D7 = 95,
        //
        // Summary:
        //     Key: Keypad 8 or Up Arrow.
        Keypad_D8 = 96,
        //
        // Summary:
        //     Key: Keypad 9 or Page Up.
        Keypad_D9 = 97,
        //
        // Summary:
        //     Key: Keypad 0 or Insert.
        Keypad_D0 = 98,
        //
        // Summary:
        //     Key: Keypad . or Delete.
        Keypad_Delete = 99,
        //
        // Summary:
        //     Key: Not supported. Non-US keyboard character.
        NON_US_2 = 100,
        //
        // Summary:
        //     Key: Application.
        Application = 101,
        //
        // Summary:
        //     Key: Left Ctrl.
        LeftCtrl = 224,
        //
        // Summary:
        //     Key: Left Shift.
        LeftShift = 225,
        //
        // Summary:
        //     Key: Left Alt.
        LeftAlt = 226,
        //
        // Summary:
        //     Key: Left GUI.
        LeftGUI = 227,
        //
        // Summary:
        //     Key: Right Ctrl.
        RightCtrl = 228,
        //
        // Summary:
        //     Key: Right Shift.
        RightShift = 229,
        //
        // Summary:
        //     Key: Right Alt.
        RightAlt = 230,
        //
        // Summary:
        //     Key: Right GUI.
        RightGUI = 231,

    }
}
