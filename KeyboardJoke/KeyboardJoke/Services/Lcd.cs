/*
Copyright 2010 GHI Electronics LLC
Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. 
*/


using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class LcdAndKeypad
    {
        public enum Keys
        {
            Up,
            Down,
            Right,
            Left,
            Select,
            None,
        }

        private OutputPort LCD_RS;      // 4 bytes
        private OutputPort LCD_E;       // 4 bytes

        private OutputPort LCD_D4;      // 4 bytes
        private OutputPort LCD_D5;      // 4 bytes
        private OutputPort LCD_D6;      // 4 bytes
        private OutputPort LCD_D7;      // 4 bytes

        private AnalogIn AnKey;         // 4 bytes

        private OutputPort BackLight;   // 4 bytes
        // 4 bytes for object reference
        // Total: 36 bytes

        const byte DISP_ON = 0xC;    //Turn visible LCD on
        const byte CLR_DISP = 1;      //Clear display
        const byte CUR_HOME = 2;      //Move cursor home and clear screen memory
        const byte SET_CURSOR = 0x80;   //SET_CURSOR + X : Sets cursor position to X

        const char Empty = ' ';

        #region Initialisation
        public LcdAndKeypad()
        {
            LCD_RS = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di8, false);
            LCD_E = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di9, false);

            LCD_D4 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di4, false);
            LCD_D5 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, false);
            LCD_D6 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di6, false);
            LCD_D7 = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, false);

            AnKey = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An0);

            BackLight = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di10, true);
        }

        public void Init()
        {
            LCD_RS.Write(false);

            // 4 bit data communication
            Thread.Sleep(50);

            LCD_D7.Write(false);
            LCD_D6.Write(false);
            LCD_D5.Write(true);
            LCD_D4.Write(true);

            LCD_E.Write(true);
            LCD_E.Write(false);

            Thread.Sleep(50);
            LCD_D7.Write(false);
            LCD_D6.Write(false);
            LCD_D5.Write(true);
            LCD_D4.Write(true);

            LCD_E.Write(true);
            LCD_E.Write(false);

            Thread.Sleep(50);
            LCD_D7.Write(false);
            LCD_D6.Write(false);
            LCD_D5.Write(true);
            LCD_D4.Write(true);

            LCD_E.Write(true);
            LCD_E.Write(false);

            Thread.Sleep(50);
            LCD_D7.Write(false);
            LCD_D6.Write(false);
            LCD_D5.Write(true);
            LCD_D4.Write(false);

            LCD_E.Write(true);
            LCD_E.Write(false);

            SendCmd(DISP_ON);
            SendCmd(CLR_DISP);
        }
        #endregion

        #region Low Level Private Methods
        //Sends an ASCII character to the LCD
        private void Putc(byte c)
        {
            LCD_D7.Write((c & 0x80) != 0);
            LCD_D6.Write((c & 0x40) != 0);
            LCD_D5.Write((c & 0x20) != 0);
            LCD_D4.Write((c & 0x10) != 0);
            LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin

            LCD_D7.Write((c & 0x08) != 0);
            LCD_D6.Write((c & 0x04) != 0);
            LCD_D5.Write((c & 0x02) != 0);
            LCD_D4.Write((c & 0x01) != 0);
            LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin
            //Thread.Sleep(1);
        }

        //Sends an LCD command
        private void SendCmd(byte c)
        {
            LCD_RS.Write(false); //set LCD to data mode

            LCD_D7.Write((c & 0x80) != 0);
            LCD_D6.Write((c & 0x40) != 0);
            LCD_D5.Write((c & 0x20) != 0);
            LCD_D4.Write((c & 0x10) != 0);
            LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin

            LCD_D7.Write((c & 0x08) != 0);
            LCD_D6.Write((c & 0x04) != 0);
            LCD_D5.Write((c & 0x02) != 0);
            LCD_D4.Write((c & 0x01) != 0);
            LCD_E.Write(true); LCD_E.Write(false); //Toggle the Enable Pin
            Thread.Sleep(1);
            LCD_RS.Write(true); //set LCD to data mode
        }
        #endregion

        #region Print
        public void Print(string str)
        {
            for (int i = 0; i < str.Length; i++)
                Putc((byte)str[i]);
        }
        public void Print(char c)
        {
            Putc((byte)c);
        }
        public void Print(char c, byte count)
        {
            for (int i = 0; i < count; i++)
                Putc((byte)c);
        }
        #endregion

        #region Clear
        public void Clear()
        {
            SendCmd(CLR_DISP);
        }
        #endregion

        #region Cursor
        public void CursorHome()
        {
            SendCmd(CUR_HOME);
        }

        public void SetCursorPosition(byte row, byte col)
        {
            SendCmd((byte)(SET_CURSOR | row << 6 | col));
        }
        #endregion

        #region GetKey
        public Keys GetKey()
        {
            int i = AnKey.Read();       // 10 bit AtoD, range = 0..1023
            // use this to read values to calibrate
            /*while (true)
            {
                i = AnKey.Read();
                Debug.Print(i.ToString());
                Thread.Sleep(300);
            }*/
            const int ERROR = 50;

            if (i >= 1021)
                return Keys.None;

            if (i < 0 + ERROR)
                return Keys.Right;

            if (i < 195 + ERROR && i > 195 - ERROR)
                return Keys.Up;

            if (i < 450 + ERROR && i > 450 - ERROR)
                return Keys.Down;

            if (i < 690 + ERROR && i > 690 - ERROR)
                return Keys.Left;

            if (i < 1000 + ERROR && i > 1000 - ERROR)
                return Keys.Select;

            return Keys.None;
        }
        #endregion

        #region Backlight
        public void SetBacklightState(bool onOrOff)
        {
            BackLight.Write(onOrOff);
        }

        public void TurnBacklightOn()
        {
            BackLight.Write(true);
        }

        public void ShutBacklightOff()
        {
            BackLight.Write(false);
        }
        #endregion

    }
}
