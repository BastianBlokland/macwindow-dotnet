using System;
using System.Threading;
using NativeMacOS;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeApp app = new NativeApp();

            bool running = true;
            NativeWindow window = new NativeWindow(app,  new Int2(1280, 720), "MacWindow");
            window.CloseRequested += () => running = false;
            window.Resized += size => Console.WriteLine("Resized: " + size);
            window.Moved += pos => Console.WriteLine("Moved: " + pos);

            int counter = 0;
            while(running)
            {
                window.Title = (counter++).ToString();

                app.ProcessEvents();
                Thread.Sleep(30); //Lets not use all the cpu :)
            }
            window.Dispose();

            app.Dispose();
        }
    }
}
