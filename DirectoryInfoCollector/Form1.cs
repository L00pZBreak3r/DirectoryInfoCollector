using System;
using System.IO;
using System.Xml;
//using System.Threading;
using System.Windows.Forms;
using DirectoryInfoDatabaseLib;

namespace DirectoryInfoCollector
{
  public partial class Form1 : Form
  {
    private class DoCollectInfoParam
    {
      public WorkControlHelper workControl;
      //public Form1 form1;
      public string path;
      public string mask;
      public string comment;
      public string db1;
      public string db2;
      public int depth;
      public bool useTicks;
      public bool useLowerCase;
      public bool excludeEmptyFolders;
      public bool onlyCorrupted;
      public bool tryToFixCorruptedFiles;
      public string excludedCompany;
      public string driverFolder;
      public TimeSpan timeScan;
      public TimeSpan timeCompare;

      public DoCollectInfoParam(Form1 form1)
      {
        //form1 = f1;
        workControl = form1.workControl;

        path = form1.textBox1.Text.Trim();
        mask = form1.textBox2.Text.Trim();
        depth = (form1.checkBox1.Checked) ? (int)form1.numericUpDown1.Value : 0;
        comment = form1.textBox3.Text.Trim();
        db1 = form1.textBox4.Text.Trim();
        db2 = form1.textBox5.Text.Trim();
        useLowerCase = form1.checkBox2.Checked;
        useTicks = form1.comboBox1.SelectedIndex <= 0;
        excludeEmptyFolders = form1.checkBox3.Checked;
        onlyCorrupted = form1.checkBox5.Checked;
        tryToFixCorruptedFiles = form1.checkBox6.Checked;
        if (form1.checkBox4.Checked)
          excludedCompany = MICROSOFT_COMPANY;
        driverFolder = Path.Combine(Application.StartupPath, DRIVER_FOLDER);
      }
    }

    private const string REPORT_FILE_EXT = ".xml";
    private const string REPORT_FILE_SUFFIX = ".report" + REPORT_FILE_EXT;

    private const string MICROSOFT_COMPANY = "Microsoft Corporation";
    private const string BUTTON_NAME_START = "Start";
    private const string BUTTON_NAME_STOP = "Stop";

    private const string DRIVER_FOLDER = "drivers";

    private WorkControlHelper workControl;
    private bool isWorking;

    DoCollectInfoParam workerThreadParam;
    //Thread workerThread;

    //delegate void StopWorkingCallback(TimeSpan timeScan, TimeSpan timeCompare);

    private bool IsWorking
    {
      get
      {
        return isWorking;
      }
      set
      {
        isWorking = value;
        if (isWorking)
        {
          textBox1.Enabled = false;
          textBox2.Enabled = false;
          checkBox1.Enabled = false;
          groupBox2.Enabled = false;
          label2.Enabled = false;
          label3.Enabled = false;
          numericUpDown1.Enabled = false;
          textBox4.Enabled = false;
          textBox5.Enabled = false;
          label8.Text = "Working...";
          button1.Text = BUTTON_NAME_STOP;
        }
        else
        {
          textBox1.Enabled = true;
          textBox4.Enabled = true;
          textBox5.Enabled = true;
          label8.Text = "Ready";
          button1.Text = BUTTON_NAME_START;
          textBox1_TextChanged(this, null);
        }
      }
    }

    public Form1()
    {
      workControl = new WorkControlHelper();
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      comboBox1.SelectedIndex = 0;
      textBox1_TextChanged(sender, e);
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      label2.Enabled = numericUpDown1.Enabled = checkBox1.Enabled && checkBox1.Checked;
      if (sender is CheckBox)
        numericUpDown1.Value = (checkBox1.Checked) ? -1 : 0;
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
      bool bT = !string.IsNullOrWhiteSpace(textBox1.Text);
      textBox2.Enabled = bT;
      checkBox1.Enabled = bT;
      groupBox2.Enabled = bT;
      label2.Enabled = bT;
      label3.Enabled = bT;
      numericUpDown1.Enabled = bT;
      checkBox1_CheckedChanged(sender, e);
      textBox4_TextChanged(sender, e);
    }

    private void textBox4_TextChanged(object sender, EventArgs e)
    {
      bool b0 = !string.IsNullOrWhiteSpace(textBox4.Text);
      bool b1 = !string.IsNullOrWhiteSpace(textBox1.Text);
      bool b2 = !string.IsNullOrWhiteSpace(textBox5.Text);
      button1.Enabled = b0 && (b1 || b2);
    }

    private static void DoCollectInfo(object data)
    {
      DoCollectInfoParam p = (DoCollectInfoParam)data;

      if (!string.IsNullOrEmpty(p.db1))
      {
        //bool bOpS = false;
        string sDb = p.db1;
        string sDbe = sDb;
        if (sDbe.EndsWith(REPORT_FILE_EXT, StringComparison.CurrentCultureIgnoreCase))
          sDb = sDb.Substring(0, sDb.Length - REPORT_FILE_EXT.Length);
        else
          sDbe += REPORT_FILE_EXT;

        //p.timeScan = TimeSpan.Zero;
        XmlDocument xDoc = null;
        DirectoryInfoDatabaseComparable di = new DirectoryInfoDatabaseComparable(p.workControl);
        if (!string.IsNullOrEmpty(p.path))
        {
          di.FileMask = p.mask;
          di.m_bLowerCaseNames = p.useLowerCase;
          di.Comment = p.comment;
          di.m_bTimeInTicks = p.useTicks;
          di.m_bOnlyCorrupted = p.onlyCorrupted;
          di.ExcludedCompany = p.excludedCompany;
          di.m_bExcludeEmptyDirs = p.excludeEmptyFolders;
          di.m_bTryToFixCorruptedFiles = p.tryToFixCorruptedFiles;
          di.m_sDriverFolder = p.driverFolder;

          xDoc = di.CollectInfo(p.path, p.depth);

          if (xDoc != null)
          {
            if (p.excludeEmptyFolders)
              xDoc = DirectoryInfoDatabaseComparable.RemoveEmptyDirectories(xDoc);
            DirectoryInfoDatabaseComparable.Save(xDoc, sDbe);
            string t1s = xDoc.DocumentElement.GetAttribute(DirectoryInfoDatabaseComparable.XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME);
            string t2s = xDoc.DocumentElement.GetAttribute(DirectoryInfoDatabaseComparable.XML_ELEMENT_ROOT_ATTRIBUTE_ENDTIME);
            long t1l, t2l;
            if (long.TryParse(t1s, out t1l) && long.TryParse(t2s, out t2l))
            {
              p.timeScan = new TimeSpan(t2l - t1l);
              //bOpS = true;
            }
            else
            {
              DateTime t1, t2;
              if (DateTime.TryParse(t1s, out t1) && DateTime.TryParse(t2s, out t2))
              {
                p.timeScan = t2 - t1;
                //bOpS = true;
              }
            }
          }
        }
        //bool bOpC = false;
        //p.timeCompare = TimeSpan.Zero;
        if (!string.IsNullOrEmpty(p.db2))
        {
          string sDb2 = p.db2;
          string sDb2e = p.db2;
          if (sDb2e.EndsWith(REPORT_FILE_EXT, StringComparison.CurrentCultureIgnoreCase))
            sDb2 = sDb2.Substring(0, sDb2.Length - REPORT_FILE_EXT.Length);
          else
            sDb2e += REPORT_FILE_EXT;
          if (!string.IsNullOrEmpty(sDb2) && File.Exists(sDb2e))
          {
            XmlDocument xDoc1 = new XmlDocument();
            xDoc1.Load(sDb2e);
            long t1l = DateTime.Now.Ticks;
            if (xDoc == null)
            {
              if (File.Exists(sDbe))
              {
                XmlDocument xDoc2 = new XmlDocument();
                xDoc2.Load(sDbe);
                xDoc = DirectoryInfoDatabaseComparable.Compare(xDoc1, xDoc2, sDb + REPORT_FILE_SUFFIX);
              }
            }
            else
              xDoc = di.Compare(xDoc1, sDb + REPORT_FILE_SUFFIX);
            long t2l = DateTime.Now.Ticks;
            if (xDoc != null)
            {
              p.timeCompare = new TimeSpan(t2l - t1l);
              //bOpC = true;
            }
          }
        }
      }

      //p.form1.StopWorking(p.timeScan, p.timeCompare);
    }

    private void StopWorking(TimeSpan timeScan, TimeSpan timeCompare)
    {
      if (IsWorking)
      {
        IsWorking = false;

        string s = "All done (";
        if (timeScan != TimeSpan.Zero)
          s += "scan:" + timeScan.ToString() + ";";
        if (timeCompare != TimeSpan.Zero)
          s += " compare:" + timeCompare.ToString();
        s += ")";

        /*if (label8.InvokeRequired)
        {
          StopWorkingCallback d = new StopWorkingCallback(StopWorking);
          Invoke(d, new object[] { timeScan, timeCompare });
        }
        else*/
          label8.Text = s;
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (IsWorking)
      {
        if (workControl.IsRunning)
          workControl.NeedStop = true;

        /*workerThread.Join();

        TimeSpan tsS = TimeSpan.Zero;
        TimeSpan tsC = TimeSpan.Zero;

        if (workerThreadParam != null)
        {
          tsS = workerThreadParam.timeScan;
          tsC = workerThreadParam.timeCompare;
        }

        StopWorking(tsS, tsC);*/
      }
      else
      {
        workerThreadParam = new DoCollectInfoParam(this);
        

        if (!string.IsNullOrWhiteSpace(textBox4.Text))
        {
          IsWorking = true;

          /*workerThread = new Thread(DoCollectInfo);

          workerThread.Start(workerThreadParam);*/

          backgroundWorker1.RunWorkerAsync();

        }
        else
          label8.Text = "Database file name not specified";
      }
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      TimeSpan tsS = TimeSpan.Zero;
      TimeSpan tsC = TimeSpan.Zero;

      if (workerThreadParam != null)
      {
        tsS = workerThreadParam.timeScan;
        tsC = workerThreadParam.timeCompare;
      }

      StopWorking(tsS, tsC);
    }

    private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      if (workerThreadParam != null)
        DoCollectInfo(workerThreadParam);
    }
  }
}