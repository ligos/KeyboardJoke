using System;
using Microsoft.SPOT;

namespace StructTests
{
    public class Program
    {
        //public readonly static QueuedEventSimple[] _SimpleEvents = new QueuedEventSimple[128];

        public static void Main()
        {
            Debug.EnableGCMessages(true);
            
            int size = 128;
            Debug.Print("Size = " + size.ToString());
            var world = new World(size);
            //var noArray = new WithoutArray();

            Debug.GC(true);
            Debug.GC(true);
            Debug.GC(true);
            Debug.Print("Length = " + world.GetLength().ToString());
            //Debug.Print("Length = " + noArray.GetLength().ToString());

            Debug.Print("Main thread sleeping forever.");
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

    }
}
