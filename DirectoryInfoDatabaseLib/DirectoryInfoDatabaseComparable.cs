using System.Xml;
using System.Text;
using System.IO;

namespace DirectoryInfoDatabaseLib
{
	public class DirectoryInfoDatabaseComparable : DirectoryInfoDatabase
	{
		protected const string XML_ELEMENT_ALTERED = "Altered";
		protected const string XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION = "Action";
		protected const string XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED = "changed";
		protected const string XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED = "deleted";
		protected const string XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED = "created";
		protected const string XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED = "renamed";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED = "Changed";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN = "Time1";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_TIME_SECONDSCAN = "Time2";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2 = "Comment2";
		private const string XSLT_SHEET_DIRECTORY_NAME = "_DiffReport_Files";
		private const string XSLT_SHEET_FILE_NAME = "DiffReport.xslt";
    private const string XSLT_SHEET_CSS_NAME = "DiffReport.css";
    private const string XSLT_SHEET_IMAGE_PLUS_NAME = "DiffReport_Plus.gif";
    private const string XSLT_SHEET_IMAGE_MINUS_NAME = "DiffReport_Minus.gif";
    private static string[] m_aDirAttributes = new string[] { XML_ELEMENT_ATTRIBUTE_CREATIONTIME, XML_ELEMENT_ATTRIBUTE_LASTWRITETIME, XML_ELEMENT_ATTRIBUTE_ATTRIBUTES, XML_ELEMENT_ATTRIBUTE_FILES, XML_ELEMENT_ATTRIBUTE_DIRECTORIES, XML_ELEMENT_ATTRIBUTE_ERROR };
		private static string[] m_aFileAttributes = new string[] { XML_ELEMENT_ATTRIBUTE_CREATIONTIME, XML_ELEMENT_ATTRIBUTE_LASTWRITETIME, XML_ELEMENT_ATTRIBUTE_ATTRIBUTES, XML_ELEMENT_ATTRIBUTE_FILELENGTH, XML_ELEMENT_ATTRIBUTE_FILESIG, XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED };
    private static string[,] m_aXsltSheetNameReplace = new string[,] { { "XML_ELEMENT_ROOT", XML_ELEMENT_ROOT }, { "XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE", XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE }, { "XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT", XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT }, { "XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2", XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2 }, { "XML_ELEMENT_FILE", XML_ELEMENT_FILE }, { "XML_ELEMENT_DIRECTORY", XML_ELEMENT_DIRECTORY }, { "XML_ELEMENT_ATTRIBUTE_FILES", XML_ELEMENT_ATTRIBUTE_FILES }, { "XML_ELEMENT_ATTRIBUTE_DIRECTORIES", XML_ELEMENT_ATTRIBUTE_DIRECTORIES },
      { "XML_ELEMENT_ATTRIBUTE_NAME", XML_ELEMENT_ATTRIBUTE_NAME }, { "XML_ELEMENT_ALTERED", XML_ELEMENT_ALTERED}, { "XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION", XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION}, { "XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED", XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED }, { "XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN", XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN }, { "XML_ELEMENT_ROOT_ATTRIBUTE_TIME_SECONDSCAN", XML_ELEMENT_ROOT_ATTRIBUTE_TIME_SECONDSCAN }, { "XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT", XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT },
      { "XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK", XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK }, { "XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME", XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME }, { "XML_ELEMENT_ROOT_ATTRIBUTE_BADSIG", XML_ELEMENT_ROOT_ATTRIBUTE_BADSIG }, { "XML_ELEMENT_ROOT_ATTRIBUTE_EXCLUDED_COMPANY", XML_ELEMENT_ROOT_ATTRIBUTE_EXCLUDED_COMPANY },
      { "XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED", XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED }, { "XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED", XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED}, { "XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED", XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED}, { "XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED", XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED },
      { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED", XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_SUCCESS", DigitalSignatureVerifier.WinVerifyTrustResult.Success.ToString() }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FILEERROR", DigitalSignatureVerifier.WinVerifyTrustResult.FileError.ToString() }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_BADDIGEST", DigitalSignatureVerifier.WinVerifyTrustResult.BadDigest.ToString() }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_ASN", DigitalSignatureVerifier.WinVerifyTrustResult.ASN1BadTag.ToString() }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_FORMUNKNOWN", DigitalSignatureVerifier.WinVerifyTrustResult.SubjectFormUnknown.ToString() }, { "XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED_NOSIGNATURE", DigitalSignatureVerifier.WinVerifyTrustResult.NoSignature.ToString() },
      { "DIFF_REPORT_TITLE", "Diff Report" },
      { "DIFF_REPORT_SCAN_COMMENT_TITLE", "Comment" },
      { "DIFF_REPORT_SUMMARY_TITLE", "Summary"},
      { "DIFF_REPORT_SUMMARY_FILE_TITLE", "Files"},
      { "DIFF_REPORT_SUMMARY_DIRECTORY_TITLE", "Directories"},
      { "DIFF_REPORT_ALTERED_STATUS_TITLE", "Status" },
      { "DIFF_REPORT_SETTING_FILEMASK_TITLE", "File mask" },
      { "DIFF_REPORT_SETTING_LOWERCASE_TITLE", "Using lower-case names" },
      { "DIFF_REPORT_SETTING_TIMEFORMAT_TITLE", "Time in ticks" },
      { "DIFF_REPORT_SETTING_BADSIG_TITLE", "Only files with corrupted Digital Signature" },
      { "DIFF_REPORT_SETTING_EXCLUDED_COMPANY_TITLE", "Only files that do not have this Company attribute" },
      { "XSLT_SHEET_DIRECTORY_NAME", XSLT_SHEET_DIRECTORY_NAME },
      { "XSLT_SHEET_IMAGE_PLUS_NAME", XSLT_SHEET_IMAGE_PLUS_NAME },
      { "XSLT_SHEET_IMAGE_MINUS_NAME", XSLT_SHEET_IMAGE_MINUS_NAME },
      { "XSLT_SHEET_CSS_NAME", XSLT_SHEET_CSS_NAME },
      { "DIFF_REPORT_DIFF_FOUND_TITLE", "changed" },
      { "DIFF_REPORT_DIFF_NOT_FOUND_TITLE", "not changed" } };
		private const string m_sDateFormatXsltText = "<script type=\"text/javascript\">tryDate(<xsl:text disable-output-escaping=\"yes\">&quot;</xsl:text>####<xsl:text disable-output-escaping=\"yes\">&quot;</xsl:text>,<xsl:text disable-output-escaping=\"yes\">&quot;</xsl:text><xsl:value-of select=\"$prop-name\"/><xsl:text disable-output-escaping=\"yes\">&quot;</xsl:text>);</script>";
		private static string[,] m_aXsltDateFormatPattern = new string[,] {
      { "DIFF_REPORT_DATEFORMAT_JS_FUNCTION", "function tryDate(dVal,nm){var v=parseInt(dVal.substr(0,dVal.length-4));if(!(isNaN(v)||(nm.indexOf(\"Time\")==-1))){var dt=new Date();dt.setUTCFullYear(1,0,1);dt.setUTCHours(0,0,0,0);dt.setTime(dt.getTime()+v);dVal=dt.toLocaleString();}document.write(dVal);}" },
      { "DIFF_REPORT_DATEFORMAT_XSLTTEXT", "<xsl:value-of select=\".\"/>" } };
		public bool m_bShowContentOfCreatedDirectory = false;
		//private const string m_sNameAttributeLowerCase As String = "translate(@" + XML_ELEMENT_ATTRIBUTE_NAME + ",\"ABCDEFGHIJKLMNOPQRSTUVWXYZ\",\"abcdefghijklmnopqrstuvwxyz\")"
		private static bool ProcessDirectoryScan(XmlElement xDir, XmlElement xElement, XmlDocument xDoc, bool bScanForCreated, bool bLowerCaseNames)
		{
			bool bRes = false;
			string sName = xDir.GetAttribute(XML_ELEMENT_ATTRIBUTE_NAME);
			if (bLowerCaseNames)
				sName = sName.ToLower();
			XmlElement xDir2 = (XmlElement) xElement.SelectSingleNode(xDir.Name + "[@" + XML_ELEMENT_ATTRIBUTE_NAME + "=\"" + sName + "\"]");
			XmlElement alt = xDoc.CreateElement(XML_ELEMENT_ALTERED);
			if (xDir2 != null)
			{
        string val1;
        string val2;
				bool b = false;
				if (!bScanForCreated)
				{
					foreach (string a in m_aDirAttributes)
					{
						val1 = xDir.GetAttribute(a);
						val2 = xDir2.GetAttribute(a);
						if (string.Compare(val1, val2, true) != 0)
						{
							alt.SetAttribute(a, val2);
							b = true;
						}
					}
					if (b)
					{
						alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED);
						xDir.AppendChild(alt);
						bRes = true;
					}
				}
				XmlNodeList fs = xDir.SelectNodes(XML_ELEMENT_FILE);
				if ((fs != null) && (fs.Count > 0))
				{
					foreach (XmlNode f in fs)
					{
						XmlElement f1 = (XmlElement) f;
						sName = f1.GetAttribute(XML_ELEMENT_ATTRIBUTE_NAME);
						if (bLowerCaseNames)
							sName = sName.ToLower();
						XmlElement f2 = (XmlElement) xDir2.SelectSingleNode(f1.Name + "[@" + XML_ELEMENT_ATTRIBUTE_NAME + "=\"" + sName + "\"]");
						alt = xDoc.CreateElement(XML_ELEMENT_ALTERED);
						if (f2 != null)
						{
							if (!bScanForCreated)
							{
								b = false;
								foreach (string a in m_aFileAttributes)
								{
									val1 = f1.GetAttribute(a);
									val2 = f2.GetAttribute(a);
									if (string.Compare(val1, val2, true) != 0)
									{
										alt.SetAttribute(a, val2);
										b = true;
									}
								}
								if (b)
								{
									alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CHANGED);
									f1.AppendChild(alt);
									bRes = true;
								}
							}
						}
						else
						{
							if (bScanForCreated)
							{
								alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED);
								XmlNode xNewFile = xDoc.ImportNode(f1, true);
								xNewFile.AppendChild(alt);
								xDir2.AppendChild(xNewFile);
							}
							else
							{
								alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED);
								f1.AppendChild(alt);
							}
							bRes = true;
						}
					}
				}
				fs = xDir.SelectNodes(XML_ELEMENT_DIRECTORY);
				if ((fs != null) && (fs.Count > 0))
				{
					foreach (XmlNode f in fs)
						bRes = ProcessDirectoryScan(((XmlElement) f), xDir2, xDoc, bScanForCreated, bLowerCaseNames) | bRes;
				}
			}
			else
			{
				if (bScanForCreated)
				{
					alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED);
					XmlNode xNewDir = xDoc.ImportNode(xDir, true);
					xNewDir.AppendChild(alt);
					xElement.AppendChild(xNewDir);
				}
				else
				{
					alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED);
					xDir.AppendChild(alt);
				}
				bRes = true;
			}
			return bRes;
		}
    private static void CleanDocument(XmlNode xNode, bool bShowContentOfCreatedDirectory, bool bDelEmptyDirsOnly)
		{
			XmlElement alt = null;
      if (bShowContentOfCreatedDirectory && !bDelEmptyDirsOnly)
				alt = (XmlElement) xNode.SelectSingleNode(XML_ELEMENT_ALTERED + "[@" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION + "=\"" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED + "\"]");
			if (alt == null)
			{
				XmlNodeList ds = (bDelEmptyDirsOnly) ? null : xNode.SelectNodes(XML_ELEMENT_FILE);
				if ((ds != null) && (ds.Count > 0))
				{
					foreach (XmlNode d in ds)
					{
						if (d.HasChildNodes)
						{
							alt = (XmlElement) d.SelectSingleNode(XML_ELEMENT_ALTERED + "[@" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION + "=\"" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED + "\"]");
							if (alt != null)
							{
								XmlElement f1 = (XmlElement) d;
								string attrs = string.Empty;
								foreach (string a in m_aFileAttributes)
									attrs += " and (@" + a + "=\"" + f1.GetAttribute(a) + "\")";
								XmlElement f2 = (XmlElement) xNode.SelectSingleNode(XML_ELEMENT_FILE + "[(" + XML_ELEMENT_ALTERED + "/@" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION + "=\"" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED + "\")" + attrs + "]");
								if (f2 != null)
								{
									string a = f2.GetAttribute(XML_ELEMENT_ATTRIBUTE_NAME);
									xNode.RemoveChild(f2);
									alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED);
									alt.SetAttribute(XML_ELEMENT_ATTRIBUTE_NAME, a);
								}
							}
						}
						else
							xNode.RemoveChild(d);
					}
				}
				ds = xNode.SelectNodes(XML_ELEMENT_DIRECTORY);
				if ((ds != null) && (ds.Count > 0))
				{
					foreach (XmlNode d in ds)
					{
            CleanDocument(d, bShowContentOfCreatedDirectory, bDelEmptyDirsOnly);
						if (d.HasChildNodes)
						{
							alt = (XmlElement) d.SelectSingleNode(XML_ELEMENT_ALTERED + "[@" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION + "=\"" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_CREATED + "\"]");
							if (alt != null)
							{
								XmlElement f1 = (XmlElement) d;
								string attrs = string.Empty;
								foreach (string a in m_aDirAttributes)
									attrs += " and (@" + a + "=\"" + f1.GetAttribute(a) + "\")";
								XmlElement f2 = (XmlElement) xNode.SelectSingleNode(XML_ELEMENT_DIRECTORY + "[(" + XML_ELEMENT_ALTERED + "/@" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION + "=\"" + XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_DELETED + "\")" + attrs + "]");
								if (f2 != null)
								{
									string a = f2.GetAttribute(XML_ELEMENT_ATTRIBUTE_NAME);
									attrs = f1.GetAttribute(XML_ELEMENT_ATTRIBUTE_NAME);
									xNode.RemoveChild(f2);
									f1.SetAttribute(XML_ELEMENT_ATTRIBUTE_NAME, a);
									alt.SetAttribute(XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION, XML_ELEMENT_ALTERED_ATTRIBUTE_ACTION_VALUE_RENAMED);
									alt.SetAttribute(XML_ELEMENT_ATTRIBUTE_NAME, attrs);
								}
							}
						}
						else
							xNode.RemoveChild(d);
					}
				}
			}
		}
    public static XmlDocument RemoveEmptyDirectories(XmlDocument xDoc)
    {
      XmlDocument xDoc1 = (XmlDocument)xDoc.Clone();
      CleanDocument(xDoc1.DocumentElement, false, true);
      return xDoc1;
    }
		public static XmlDocument Compare(XmlDocument xDoc1, XmlDocument xDoc2, string sFullName, bool bShowContentOfCreatedDirectory)
		{
			XmlDocument xDoc = null;
			if ((xDoc1 != null) && (xDoc2 != null))
			{
				XmlElement xRoot = xDoc1.DocumentElement;
				XmlElement xRoot2 = xDoc2.DocumentElement;
				if (xRoot.Name == xRoot2.Name)
				{
					string lc1 = xRoot.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE);
					string lc2 = xRoot2.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE);
					string tf1 = xRoot.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT);
					string tf2 = xRoot2.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT);
          string fm1 = xRoot.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK);
          string fm2 = xRoot2.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK);
					bool bLowerCaseNames, bLowerCaseNames2;
          bool.TryParse(lc1, out bLowerCaseNames);
          bool.TryParse(lc2, out bLowerCaseNames2);
          bool btf1, btf2;
          bool.TryParse(tf1, out btf1);
          bool.TryParse(tf2, out btf2);
          if (!(bLowerCaseNames ^ bLowerCaseNames2) && !(btf1 ^ btf2) && (string.Compare(fm1, fm2, true) == 0))
					{
						xDoc = (XmlDocument) xDoc1.Clone();
						xRoot = xDoc.DocumentElement;
						bool bRes = false;
						XmlNodeList ds = xRoot.SelectNodes(XML_ELEMENT_DIRECTORY);
						if ((ds != null) && (ds.Count > 0))
						{
							foreach (XmlNode xNode in ds)
								bRes = ProcessDirectoryScan((XmlElement) xNode, xRoot2, xDoc, false, bLowerCaseNames) | bRes;
						}
						ds = xRoot2.SelectNodes(XML_ELEMENT_DIRECTORY);
						if ((ds != null)&& ds.Count > 0)
						{
							foreach (XmlNode xNode in ds)
								bRes = ProcessDirectoryScan((XmlElement) xNode, xRoot, xDoc, true, bLowerCaseNames) | bRes;
						}
						CleanDocument(xRoot, bShowContentOfCreatedDirectory, false);
						xRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_CHANGED, bRes.ToString());
						lc1 = xRoot.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME);
						lc2 = xRoot2.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME);
						xRoot.RemoveAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME);
						xRoot.RemoveAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_ENDTIME);
						xRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIME_FIRSTSCAN, lc1);
						xRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIME_SECONDSCAN, lc2);
						if (xRoot2.HasAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT))
							xRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT2, xRoot2.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT));
						Save(xDoc, sFullName);
					}
				}
			}
			return xDoc;
		}
    public static XmlDocument Compare(XmlDocument xDoc1, XmlDocument xDoc2, string sFullName)
		{
      return Compare(xDoc1, xDoc2, sFullName, false);
    }
    public static XmlDocument Compare(XmlDocument xDoc1, XmlDocument xDoc2)
		{
      return Compare(xDoc1, xDoc2, null, false);
    }
		public XmlDocument Compare(XmlDocument xDoc, string sFullName)
		{
			return Compare(xDoc, m_xDocument, sFullName, m_bShowContentOfCreatedDirectory);
		}
    public XmlDocument Compare(XmlDocument xDoc)
		{
			return Compare(xDoc, m_xDocument, null, m_bShowContentOfCreatedDirectory);
		}
		public static void Save(XmlDocument xReport, string sFullName)
		{
			if ((xReport != null) && !string.IsNullOrEmpty(sFullName))
			{
				string tf1 = xReport.DocumentElement.GetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT);
				bool bUseMillis;
				if ((xReport.DocumentElement.Name == XML_ELEMENT_ROOT) && bool.TryParse(tf1, out bUseMillis))
				{
					string sDir = Path.GetDirectoryName(sFullName);
					if (string.IsNullOrEmpty(sDir))
						sDir = Directory.GetCurrentDirectory();
					if (!string.IsNullOrEmpty(sDir) && Directory.Exists(sDir))
					{
						sDir = Path.Combine(sDir, XSLT_SHEET_DIRECTORY_NAME);
						Directory.CreateDirectory(sDir);
            Properties.Resources.DiffReport_Minus.Save(Path.Combine(sDir, XSLT_SHEET_IMAGE_MINUS_NAME));
            Properties.Resources.DiffReport_Plus.Save(Path.Combine(sDir, XSLT_SHEET_IMAGE_PLUS_NAME));
            File.WriteAllText(Path.Combine(sDir, XSLT_SHEET_CSS_NAME), Properties.Resources.DiffReport, Encoding.Unicode);
            string sXsltText = Properties.Resources.DiffReportXslt;
            sXsltText = sXsltText.Replace("##" + m_aXsltDateFormatPattern[0, 0] + "##", (bUseMillis) ? m_aXsltDateFormatPattern[0, 1] : string.Empty);
						int cnt = m_aXsltDateFormatPattern.GetLength(0);
						for (int i = 1; i < cnt; i++)
              sXsltText = sXsltText.Replace("##" + m_aXsltDateFormatPattern[i, 0] + "##", (bUseMillis) ? m_sDateFormatXsltText.Replace("####", m_aXsltDateFormatPattern[i, 1]) : m_aXsltDateFormatPattern[i, 1]);
						cnt = m_aXsltSheetNameReplace.GetLength(0);
						for (int i = 0; i < cnt; i++)
							sXsltText = sXsltText.Replace("##" + m_aXsltSheetNameReplace[i, 0] + "##", m_aXsltSheetNameReplace[i, 1]);
						File.WriteAllText(Path.Combine(sDir, XSLT_SHEET_FILE_NAME), sXsltText, Encoding.Unicode);
						XmlDocument xDoc = new XmlDocument();
						//XmlProcessingInstruction xPI = xDoc.CreateProcessingInstruction("xml", XML_PROCESSING_INSTRUCTION + " encoding=\"" + Encoding.Unicode.WebName + "\"");
						//xDoc.AppendChild(xPI);
            XmlProcessingInstruction xPI = xDoc.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"" + XSLT_SHEET_DIRECTORY_NAME + "/" + XSLT_SHEET_FILE_NAME + "\"");
						xDoc.AppendChild(xPI);
						XmlNode xRoot = xDoc.ImportNode(xReport.DocumentElement, true);
						xDoc.AppendChild(xRoot);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(sFullName, settings))
              xDoc.Save(writer);
					}
				}
			}
		}
    public DirectoryInfoDatabaseComparable(WorkControlHelper wc = null) : base(wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, WorkControlHelper wc = null) : base(sDirectoryInfo, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, sFileMask, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, sFileMask, bTimeInTicks, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, bool onlyCorrupted = false, string excludedCompany = null, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, depth, onlyCorrupted, excludedCompany, wc) { }
    public DirectoryInfoDatabaseComparable(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, string excludedCompany, WorkControlHelper wc = null) : base(sDirectoryInfo, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, depth, excludedCompany, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, WorkControlHelper wc = null) : base(sDirectoryPath, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, string sFileMask, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, sFileMask, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, sFileMask, bTimeInTicks, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, bool onlyCorrupted = false, string excludedCompany = null, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, depth, onlyCorrupted, excludedCompany, wc) { }
    public DirectoryInfoDatabaseComparable(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, string excludedCompany, WorkControlHelper wc = null) : base(sDirectoryPath, sComment, sFileMask, bTimeInTicks, bLowerCaseNames, depth, excludedCompany, wc) { }
  }
}
