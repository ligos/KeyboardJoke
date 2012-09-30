using System;
using Microsoft.SPOT;

namespace StructTests
{
    public class Program
    {
        public static World World;

        public static void Main()
        {
            //Debug.EnableGCMessages(true);

            uint[] types = new uint[32];
            for (int i = 0; i < types.Length; i++)
                types[i] = (uint)(1 << i);
            int[] sizes = new[] { 4, 8, 16, 32, 64, 128, 256, 512, 1024 };
            World = new World();

            for (int typeIdx = 0; typeIdx < types.Length; typeIdx++)
            {
                var type = types[typeIdx];
                var typeName = World.GetTypeName(type);
                Debug.Print("Type = " + typeName);
                for (int sizeIdx = 0; sizeIdx < sizes.Length; sizeIdx++)
                {
                    var size = sizes[sizeIdx];
                    Debug.Print("  Size = " + size.ToString());

                    World.ClearAll();
                    var memoryUsageBefore = World.GetMemoryUsage();
                    World.SetArray(type, size);
                    var memoryUsageAfter = World.GetMemoryUsage();

                    var deltaMemory = memoryUsageBefore - memoryUsageAfter;
                    Debug.Print("  Delta memory usage = " + deltaMemory.ToString("N0"));
                    var estMemoryEach = (double)deltaMemory / (double)size;
                    Debug.Print("  Estimate " + estMemoryEach.ToString("N2") + " bytes per object");
                }
                Debug.Print("");
            }

            Debug.Print("Main thread sleeping forever.");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

    }
}
