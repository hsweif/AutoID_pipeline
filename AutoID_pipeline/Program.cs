using System;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string jsonfile = "./TagInfo.json";
            System.Console.WriteLine("test");
            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject o = (JObject)JToken.ReadFrom(reader);
                    string json = o.ToString();
                    System.Console.WriteLine(json);
                }
            }
        }
    }
}
