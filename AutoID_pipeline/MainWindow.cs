using System;
using System.Collections.Generic;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

public partial class MainWindow : Gtk.Window
{
    public static MainWindow ins;
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


    static private int rssiThreshold = 100;
    static private int readTimeout = 500;
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
        ins = this;
        LoadObjectList();
        LoadBinaryState();
        BindButton();
        JArray items = tagInfo["drawer"].Value<JArray>();
        Console.WriteLine(items);
        ThreadStart threadStart = new ThreadStart(ReadSyncFunc);
        Thread readerThread = new Thread(threadStart);
        readerThread.Start();
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

    public void UpdateRFIDList(List<string> rfidList)
    {
        int cnt = 0;
        foreach (var rf in rfidList)
        {
            rfidComboBox.InsertText(cnt, rf);
            cnt++;
        }
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

    protected static void SetupReader(string uri)
    {
        string readeruri = uri;
        // reader = Reader.Create(readeruri);
    }

    private static void ReadSyncFunc()
    {
        int testCnt = 0;
        while (true)
        {
            Console.WriteLine(testCnt);
            /*
            TagReadData[] reads = reader.Read(readTimeout);
            List<TagReadData> tagReads = new List<TagReadData>(reads);
            tagReads.Sort(
                delegate (TagReadData t1, TagReadData t2)
                {
                    return t2.Rssi.CompareTo(t1.Rssi);
                }
            );
            List<string> list = new List<string>();
            foreach(var tagRead in tagReads)
            {
                if(tagRead.Rssi > rssiThreshold)
                {
                    // TODO: Add to list
                    list.Add(tagRead.EpcString);
                }
            }
            */
            List<string> list = new List<string>()
                {
                    testCnt.ToString()
                };
            testCnt++;
            MainWindow.ins.UpdateRFIDList(list);
            Thread.Sleep(readTimeout);
        }
    }
}
