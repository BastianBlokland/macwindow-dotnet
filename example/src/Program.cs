using System;
using System.Threading;
using MacOS;

namespace Example
{
    class Program
    {
		static void Main(string[] args)
        {
			NativeApp app = new NativeApp();

			bool running = true;
			NativeWindow window = new NativeWindow(app,  new Size(1280f, 720f));
			window.CloseRequested += () => running = false;
			while(running)
			{
				app.ProcessEvents();
				Thread.Sleep(30); //Lets not use all the cpu :)
			}
			window.Dispose();

			app.Dispose();
        }
    }
}
