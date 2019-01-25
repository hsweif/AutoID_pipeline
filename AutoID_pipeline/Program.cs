using System;
using System.Collections.Generic;
using Gtk;
// using ThingMagic;
using System.Threading;

namespace AutoID_pipeline
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow win = new MainWindow();
            win.Show();
            Application.Run();
        }
    }
}
