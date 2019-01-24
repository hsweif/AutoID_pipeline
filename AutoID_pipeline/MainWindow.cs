using System;
using System.Collections.Generic;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        LoadList();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void LoadList()
    {
        CellRendererText cell = new CellRendererText();
        this.rfidComboBox.Activate();
        this.rfidComboBox.InsertText(0, "test1");
        this.rfidButton.Clicked += new EventHandler(RfidButton_clicked);
    }

    private void RfidButton_clicked(object sender, System.EventArgs eventArgs)
    {
        string ctx = this.rfidEntry.Text;
        this.rfidComboBox.InsertText(0, ctx);
    }
}
