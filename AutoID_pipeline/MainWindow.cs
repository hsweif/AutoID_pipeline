using System;
using System.Collections.Generic;
using Gtk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using ThingMagic;
using SampleConsoleApplication;


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

    // private string readerUri = "tmr://101.6.114.16";
    private string readerUri = "";
    static private bool readerConnected = false;
    static private int rssiThreshold = 100;
    static private int readTimeout = 1000;
    private string tagInfoPath = "../../TagInfo.json";
    private string rfid;
    private string objName;
    private Gtk.ListStore rfidListStore;
    private List<string> origRfidList;
    private string onState = "ON", offState = "OFF";
    private string onSemantic, offSemantic;
    private JObject tagInfo;
    int objNum = 0;
    int rfidNum = 0;
    static private Reader mercuryReader;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        ins = this;
        LoadObjectList();
        LoadBinaryState();
        BindButton();
        JArray items = tagInfo["drawer"].Value<JArray>();
        Console.WriteLine(items);
        rfidListStore = new ListStore(typeof(string));
        origRfidList = new List<string>();
        rfidComboBox.Model = rfidListStore;
        SetMercuryReader();
        ThreadStart threadStart = new ThreadStart(ReadSyncFunc);
        Thread readerThread = new Thread(threadStart);
        readerThread.Start();
    }

    protected void SetMercuryReader()
    {
        try
        {
            mercuryReader = Reader.Create(readerUri);
            mercuryReader.Connect();
            readerConnected = true;
        }
        catch(Exception e)
        {
            // Console.WriteLine(e.StackTrace);
        }
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
        if(rfidList == origRfidList)
        {
            return;
        }
        // rfidListStore.Clear();
        /*
        foreach(var rfid in rfidListStore)
        {
            var v = rfid.ToString();
            Console.WriteLine(v.ToString());
            if (!rfidList.Contains(v))
            {
                TreeIter iter;
                rfidListStore.GetIterFromString(out iter, v);
                if(!rfidListStore.Remove(ref iter)) {
                    Console.WriteLine("Fail to remove");
                }
            }
            else
            {
                rfidList.Remove(v);
            }
        }
        */
        origRfidList = rfidList;
        foreach(var rfid in rfidList)
        {
            Console.WriteLine(rfid);
            rfidListStore.AppendValues(rfid);
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

    private static void ReadSyncFunc()
    {
        int testCnt = 0;
        while (true)
        {
            if(readerConnected)
            {
                List<string> rfidDataList = Program.GetRfidData(mercuryReader, readTimeout, rssiThreshold);
                MainWindow.ins.UpdateRFIDList(rfidDataList);
            }
            else
            {
                Console.WriteLine("Try to connect to the reader...");
                // MainWindow.ins.SetMercuryReader();
                List<string> list = new List<string>()
                {
                    testCnt.ToString()
                };
                testCnt++;
                MainWindow.ins.UpdateRFIDList(list);
            }
            Thread.Sleep(readTimeout);
        }
    }
}
