using System;
using System.Collections.Generic;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class MainWindow : Gtk.Window
{
    private string tagInfoPath = "../../TagInfo.json";
    private JObject tagInfo;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        LoadObjectList();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void LoadObjectList()
    {
        // rfidComboBox.Activate();
        // rfidComboBox.InsertText(0, "test1");
        // rfidButton.Clicked += new EventHandler(RfidButton_clicked);
        using (System.IO.StreamReader file = System.IO.File.OpenText(tagInfoPath))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                string json = o.ToString();
                int cnt = 0;
                foreach(var pair in o)
                {
                    Console.WriteLine(pair.Key);
                    objectCombobox.InsertText(cnt, pair.Key);
                    cnt++;
                }
                tagInfo = o;
            }
        }
        objButton.Clicked += new EventHandler(ObjButton_clicked);
    }

    protected void updateTagInfo()
    {
        using (System.IO.StreamWriter file = System.IO.File.CreateText(tagInfoPath))
        {
            using(JsonTextWriter writer = new JsonTextWriter(file))
            {
                writer.WriteRaw(tagInfo.ToString());
            }
        }
    }

    private void RfidButton_clicked(object sender, System.EventArgs eventArgs)
    {
        string ctx = rfidEntry.Text;
        rfidComboBox.InsertText(0, ctx);
    }

    private void ObjButton_clicked(object sender, System.EventArgs eventArgs)
    {
        if(objectEntry.Text.Length == 0)
        {
            Console.WriteLine(objectCombobox.ActiveText);
        }
        else
        {

        }
    }

}
