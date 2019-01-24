using System;
using System.Collections.Generic;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class MainWindow : Gtk.Window
{
    protected class TagInfo
    {
        string rfid;
        string on;
        string off;
        public TagInfo(string _rfid, string _on, string _off)
        {
            rfid = _rfid;
            on = _on;
            off = _off;
        }
    }

    private string tagInfoPath = "../../TagInfo.json";
    private string rfid;
    private string objName;
    private string onState = "ON", offState = "OFF";
    private string onSemantic, offSemantic;
    private JObject tagInfo;
    int objNum = 0;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        LoadObjectList();
        LoadBinaryState();
        LoadRFIDList();
        BindButton();
        JArray items = tagInfo["drawer"].Value<JArray>();
        Console.WriteLine(items);
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void BindButton()
    {
        rfidButton.Clicked += new EventHandler(RfidButton_clicked);
        objButton.Clicked += new EventHandler(ObjButton_clicked);
        saveButton.Clicked += new EventHandler(SaveButton_clicked);
        binButton.Clicked += new EventHandler(BinButton_clicked);
    }

    protected void LoadRFIDList()
    {
    }

    protected void LoadObjectList()
    {
        using (System.IO.StreamReader file = System.IO.File.OpenText(tagInfoPath))
        {
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject o = (JObject)JToken.ReadFrom(reader);
                string json = o.ToString();
                foreach(var pair in o)
                {
                    Console.WriteLine(pair.Key);
                    objectCombobox.InsertText(objNum, pair.Key);
                    objNum ++;
                }
                tagInfo = o;
            }
        }
    }

    protected void LoadBinaryState()
    {
        binCombobox.InsertText(0, onState);
        binCombobox.InsertText(1, offState);
    }

    protected void UpdateObjectInfo()
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
        if(rfidEntry.Text.Length == 0)
        {

        }
        else
        {
            rfid = rfidEntry.Text;
            rfidEntry.Text = "";
        }
    }

    private void ObjButton_clicked(object sender, System.EventArgs eventArgs)
    {
        if(objectEntry.Text.Length == 0)
        {
            objName = objectCombobox.ActiveText;
        }
        else
        {
            objName = objectEntry.Text;
            JArray jArray = new JArray();
            JProperty jObject = new JProperty(objName, jArray);
            tagInfo.Add(jObject);
            objectCombobox.InsertText(objNum, objName);
            objNum++;
            Console.WriteLine(tagInfo);
        }
        UpdateObjectInfo();
    }

    private void BinButton_clicked(object sender, System.EventArgs eventArgs)
    {
        if(binCombobox.ActiveText == onState)
        {
            onSemantic = semEntry.Text;            
        }
        else if(binCombobox.ActiveText == offState)
        {
            offSemantic = semEntry.Text;            
        }
        semEntry.Text = "";
    }

    private void SaveButton_clicked(object sender, System.EventArgs eventArgs)
    {
        TagInfo tag = new TagInfo(rfid, onSemantic, offSemantic);
        var stateArray = new JArray();
        var on = new JObject { { "on", onSemantic } };
        var off = new JObject { { "off", offSemantic } };
        stateArray.Add(on);
        stateArray.Add(off);
        JObject newItem = new JObject { { rfid, stateArray } };
        if (tagInfo.ContainsKey(objName))
        {
            JArray items = tagInfo[objName].Value<JArray>();
            items.Insert(0, newItem);
        }
        Console.WriteLine(tagInfo.ToString());
        UpdateObjectInfo();
    }

}
