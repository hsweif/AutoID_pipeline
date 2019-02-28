using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AutoID_pipeline
{
    public partial class InfoWindow : Gtk.Window
    {
        public InfoWindow() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }

        public void SetWindows(string name, JToken token)
        {
            objName.Text = name;
            string content = "RFID / ON STATE / OFF STATE\r\n";
            JArray jArray = (JArray)token;
            JObject jObject = (JObject)jArray.First;
            Console.WriteLine(jObject);
            if (jObject != null)
            {
                foreach(var pair in jObject)
                {
                    content += pair.Key;
                    content += " / ";
                    JArray array = (JArray)pair.Value;
                    foreach(var item in array)
                    {
                        JObject o = (JObject)item;
                        foreach(var p in o)
                        {
                            content += p.Key;
                            content += ": ";
                            content += p.Value;
                        }
                        content += " / ";
                    }
                    content += "\r\n";
                }
            }
            semInfo.Text = content;
        }
    }
}
