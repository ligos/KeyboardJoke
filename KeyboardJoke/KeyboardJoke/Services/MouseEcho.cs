using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.USBHost;
using GHIElectronics.NETMF.USBClient;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class MouseEcho
    {
        private USBH_Mouse _MouseHost;
        private USBC_Mouse _MouseClient;
        private readonly bool _DebugMode;

        public MouseEcho(bool inDebugMode)
        {
            _DebugMode = inDebugMode;
        }

        public void BeginMouseEmulation(USBH_Device device)
        {
            _MouseHost = new USBH_Mouse(device);
            _MouseHost.Disconnected += new USBH_MouseEventHandler(_MouseHost_Disconnected);
            _MouseHost.MouseDown += new USBH_MouseEventHandler(_MouseHost_Activity);
            _MouseHost.MouseUp += new USBH_MouseEventHandler(_MouseHost_Activity);
            _MouseHost.MouseMove += new USBH_MouseEventHandler(_MouseHost_Activity);
            if (!_DebugMode)
                _MouseClient = USBClientController.StandardDevices.StartMouse();
        }
        private void _MouseHost_Activity(USBH_Mouse sender, USBH_MouseEventArgs args)
        {
            if (!_DebugMode && _MouseClient != null)
                _MouseClient.SendData(args.DeltaPosition.X, args.DeltaPosition.Y, args.DeltaPosition.ScrollWheelValue, (USBC_Mouse.Buttons)args.ButtonState);
        }

        private void _MouseHost_Disconnected(USBH_Mouse sender, USBH_MouseEventArgs args)
        {
            if (USBClientController.GetState() != USBClientController.State.Stopped)
            {
                USBClientController.Stop();
                _MouseClient = null;
                _MouseHost = null;
            }
        }
    }
}
