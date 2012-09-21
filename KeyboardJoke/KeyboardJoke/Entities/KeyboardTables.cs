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
        public static char[] EndOfSentenceCharacters = new char[] { '.', '?', '!'};



        public static byte[] StringToKeyStrokes(string s)
        {
            // TODO: proper lookup table.
            // TOOD: handle capitalisation / shift key up and down.
            if (s == "Phrase")
                return new byte[] { (byte)USBH_Key.P, (byte)USBH_Key.H, (byte)USBH_Key.R, (byte)USBH_Key.A, (byte)USBH_Key.S, (byte)USBH_Key.E };
            else if (s == "Another")
                return new byte[] { (byte)USBH_Key.A, (byte)USBH_Key.N, (byte)USBH_Key.O, (byte)USBH_Key.T, (byte)USBH_Key.H, (byte)USBH_Key.E, (byte)USBH_Key.R };
            else if (s == "Project")
                return new byte[] { (byte)USBH_Key.P, (byte)USBH_Key.R, (byte)USBH_Key.O, (byte)USBH_Key.J, (byte)USBH_Key.E, (byte)USBH_Key.C, (byte)USBH_Key.T };
            else if (s == "Refactor")
                return new byte[] { (byte)USBH_Key.R, (byte)USBH_Key.E, (byte)USBH_Key.F, (byte)USBH_Key.A, (byte)USBH_Key.C, (byte)USBH_Key.T, (byte)USBH_Key.O, (byte)USBH_Key.R };
            else
                return new byte[] { };
        }
    }
}
