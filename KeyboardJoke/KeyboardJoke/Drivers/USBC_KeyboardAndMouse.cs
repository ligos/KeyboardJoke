using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware.UsbClient;
using GHIElectronics.NETMF.USBClient;
using UsbConf = Microsoft.SPOT.Hardware.UsbClient.Configuration;

namespace MurrayGrant.KeyboardJoke.Drivers
{
    /// <summary>
    /// Implements a USB client Keyboard and Mouse combo.
    /// Based on reverse engineering the GHI USBC_Keyboard and USBC_Mouse classes.
    /// </summary>
    public class USBC_KeyboardAndMouse
    {
        private readonly static byte[] _KbReportDescriptorPayload = new byte[] {         
            0x5, 0x1, 0x9, 0x6, 0xa1, 0x1, 0x5, 0x7, 0x19, 0xe0, 0x29, 0xe7, 0x15, 0x0, 0x25, 0x1, 
            0x75, 0x1, 0x95, 0x8, 0x81, 0x2, 0x95, 0x7, 0x75, 0x8, 0x15, 0x0, 0x26, 0xff, 0x0, 0x5, 
            0x7, 0x19, 0x0, 0x2a, 0xff, 0x0, 0x81, 0x0, 0xc0 };
        private readonly static byte[] _KbClassDescriptorPayload;

        private const byte KB_DESC_TYPE_HID = 0x21;
        private const byte KB_INTERFACE_CLASS_HID = 0x3;
        private const byte KB_MAX_KEYS = 0x7;
        private readonly byte[] _KbReport = new byte[8];
        private readonly USBC_Stream _KbStream;

        private readonly static byte[] _MouseClassDescriptorPayload = new byte[] { 0x1, 0x1, 0x0, 0x1, 0x22, 0x34, 0x0 };
        private readonly static byte[] _MouseReportDescriptorPayload = new byte[] { 
            0x5, 0x1, 0x9, 0x2, 0xa1, 0x1, 0x9, 0x1, 0xa1, 0x0, 0x5, 0x9, 0x19, 0x1, 0x29, 0x5, 
            0x15, 0x0, 0x25, 0x1, 0x95, 0x5, 0x75, 0x1, 0x81, 0x2, 0x95, 0x1, 0x75, 0x3, 0x81, 0x1, 
            0x5, 0x1, 0x9, 0x30, 0x9, 0x31, 0x9, 0x38, 0x15, 0x81, 0x25, 0x7f, 0x75, 0x8, 0x95, 0x3, 
            0x81, 0x6, 0xc0, 0xc0 };

        private const byte M_DESC_TYPE_HID = 0x21;
        private const byte M_INTERFACE_CLASS_HID = 0x3;
        private readonly byte[] _MouseReport = new byte[4];
        private const int REPORT_FIELD_MOUSE_BT = 0x0;
        private const int REPORT_FIELD_MOUSE_W = 0x3;
        private const int REPORT_FIELD_MOUSE_X = 0x1;
        private const int REPORT_FIELD_MOUSE_Y = 0x2;
        private readonly USBC_Stream _MouseStream;

        static USBC_KeyboardAndMouse()
        {
            _KbClassDescriptorPayload = new byte[] { 0x1, 0x1, 0x0, 0x1, 0x22, 0x0, 0x0 };
            _KbClassDescriptorPayload[5] = (byte)_KbReportDescriptorPayload.Length;
        }
        public static USBC_KeyboardAndMouse StartDefault()
        {
            // This is a bit naughty: taking GHI's vendor ID.
            var device = new USBC_Device(0x1b9f, 0xf010, 0x100, 250, null, "KeyboardAndMouse", null);
            var result = new USBC_KeyboardAndMouse(device);
            USBClientController.Start(device);
            return result;
        }

        public USBC_KeyboardAndMouse(USBC_Device device)
        {
            // Keyboard.
            byte endpointAddress = device.ReserveNewEndpoint();
            var endpoints = new UsbConf.Endpoint[] { new UsbConf.Endpoint(endpointAddress, 0x83) };
            endpoints[0].bInterval = 10;
            endpoints[0].wMaxPacketSize = 8;
            var usbInterface = new UsbConf.UsbInterface(0, endpoints)
            {
                bInterfaceClass = 3,
                bInterfaceSubClass = 0,
                bInterfaceProtocol = 0,
                classDescriptors = new UsbConf.ClassDescriptor[] { new UsbConf.ClassDescriptor(0x21, _KbClassDescriptorPayload) }
            };
            var desc = new UsbConf.GenericDescriptor(0x81, 0x2200, _KbReportDescriptorPayload);
            byte num2 = device.AddInterface(usbInterface, "Keyboard");
            device.AddDescriptor(desc);
            desc.wIndex = num2;
            this._KbStream = device.CreateUSBStream(endpointAddress, 0);
            this._KbStream.WriteTimeout = 20;



            // Mouse.
            endpointAddress = device.ReserveNewEndpoint();
            endpoints = new UsbConf.Endpoint[] { new UsbConf.Endpoint(endpointAddress, 0x83) };
            endpoints[0].bInterval = 10;
            endpoints[0].wMaxPacketSize = 8;
            usbInterface = new UsbConf.UsbInterface(1, endpoints)
            {
                bInterfaceClass = 3,
                bInterfaceSubClass = 0,
                bInterfaceProtocol = 0,
                classDescriptors = new UsbConf.ClassDescriptor[] { new UsbConf.ClassDescriptor(0x21, _MouseClassDescriptorPayload) }
            };
            desc = new UsbConf.GenericDescriptor(0x81, 0x2200, _MouseReportDescriptorPayload);
            byte num3 = device.AddInterface(usbInterface, "Mouse");
            device.AddDescriptor(desc);
            desc.wIndex = num3;
            this._MouseStream = device.CreateUSBStream(endpointAddress, 0);
            this._MouseStream.WriteTimeout = 20;
        }


        public void KeyDown(USBC_Key key)
        {
            // This was taken from USBC_Keyboard.KeyDown() from reflector, with minor reformatting.
            int num = 0;
            if ((key >= USBC_Key.LeftCtrl) && (key <= USBC_Key.RightGUI))
            {
                byte num3 = (byte) (((int) key) - 0xe0);
                this._KbReport[0] = (byte) (this._KbReport[0] | ((byte) (((int)1) << num3)));
            }
            else
            {
                for (int i = 0; i < this._KbReport.Length-1; i++)
                {
                    if (this._KbReport[i+1] == (byte)key)
                        break;
                    if (this._KbReport[i+1] == 0x0)
                        num = i;
                }
                if (num < this._KbReport.Length-1)
                    this._KbReport[num+1] = (byte) key;
            }
            this._KbStream.Write(this._KbReport, 0, this._KbReport.Length);
        }


        public void KeyUp(USBC_Key key)
        {
            // This was taken from USBC_Keyboard.KeyUp() from reflector, with minor reformatting.
            if ((key >= USBC_Key.LeftCtrl) && (key <= USBC_Key.RightGUI))
            {
                byte num2 = (byte)(((int)key) - 0xe0);
                this._KbReport[0] = (byte)(this._KbReport[0] & ((byte)~(((int)0x1) << num2)));
            }
            else
            {
                for (int i = 0; i < this._KbReport.Length-1; i++)
                {
                    if (this._KbReport[i+1] == (byte)key)
                    {
                        this._KbReport[i+1] = 0;
                        break;
                    }
                }
            }
            this._KbStream.Write(this._KbReport, 0, this._KbReport.Length);
        }

        public void KeyTap(USBC_Key key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        public void SendMouseData(int dx, int dy, int dw, USBC_Mouse.Buttons buttons)
        {
            this._MouseReport[0] = (byte)buttons;
            this._MouseReport[1] = (byte)dx;
            this._MouseReport[2] = (byte)dy;
            this._MouseReport[3] = (byte)dw;
            this._MouseStream.Write(this._MouseReport, 0, this._MouseReport.Length);
        }
    }
}
