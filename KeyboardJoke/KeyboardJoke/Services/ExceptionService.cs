using System;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.IO;
using GHIElectronics.NETMF.IO;

namespace MurrayGrant.KeyboardJoke.Services
{
    public class ExceptionService
    {
        private LcdAndKeypad _Lcd;
        public int ExceptionHappened = 0;
        public bool HasExceptionHappened { get { return ExceptionHappened != 0; } }

        public readonly VolumeInfo Volume;
        private readonly PersistentStorage Device;
        public const string ExceptionFileName = "exceptions.txt";

        public static readonly ExceptionService Singleton = new ExceptionService();

        private ExceptionService()
        {
            this.Device = new PersistentStorage("SD");
            this.Device.MountFileSystem();
            this.Volume = VolumeInfo.GetVolumes()[0];
            if (this.Volume == null)
                throw new ApplicationException("Unable to get SD card VolumeInfo object.");

        }
        public void Init(LcdAndKeypad lcd)
        {
            this._Lcd = lcd;
        }

        public void HandleException(Exception ex)
        {
            System.Threading.Interlocked.CompareExchange(ref this.ExceptionHappened, 1, 0);
            var now = WallClock.GetDateTime();
            var now2 = Utility.GetMachineTime();

            // Log detail to file.
            this.LogException(ex, now, now2);

            System.Threading.Thread.Sleep(100);

            // Dump to LCD.
            if (this._Lcd != null)
            {
                this._Lcd.Clear();
                this._Lcd.SetCursorPosition(0, 0);
                this._Lcd.Print(ex.GetType().Name);
                this._Lcd.SetCursorPosition(1, 0);
                if (ex is System.IO.IOException)
                    this._Lcd.Print((ex as System.IO.IOException).ErrorCode.ToString());
                else
                    this._Lcd.Print(ex.Message);
            }
        }

        public void LogException(Exception ex, DateTime timestamp, TimeSpan machineTime)
        {
            // Silly hresult isn't public.
            int hresult;
            try
            {
                hresult = (int)ex.GetType().GetField("m_HResult", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(ex);
            }
            catch (Exception)
            {
                hresult = 0;
            }

            using (var stream = new FileStream(Path.Combine(this.Volume.RootDirectory, ExceptionFileName), FileMode.Append, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("WallTime: " + timestamp.ToString("yyyy-MM-dd HH:mm:ss") + "Z");
                writer.WriteLine("MachineTime: " + machineTime);
                writer.WriteLine(ex.ToString());
                writer.WriteLine("HResult: " + hresult.ToString());
                writer.WriteLine(ex.StackTrace);

                for (int i = 0; i < 15; i++)
                    writer.Write('-');
                writer.WriteLine();
            }
            Volume.FlushAll();
        }

    }
}
