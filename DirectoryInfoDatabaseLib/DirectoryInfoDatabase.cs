using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace DirectoryInfoDatabaseLib
{
  public class DirectoryInfoDatabase
	{
		protected const string XML_COMMENT = "Directory Info Database";
		protected const string XML_PROCESSING_INSTRUCTION = "version=\"1.0\"";
		//public const string XML_NAMESPACE_DEFAULT = "urn:directoryinfodatabase:schemas:databasedocument"
		public const string XML_ELEMENT_ROOT = "DirectoryInfoDatabase";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME = "StartTime";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_ENDTIME = "EndTime";
		public const string XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT = "TimeFormat";
    public const string XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE = "LowerCase";
    public const string XML_ELEMENT_ROOT_ATTRIBUTE_BADSIG = "BadSignature";
    public const string XML_ELEMENT_ROOT_ATTRIBUTE_EXCLUDED_COMPANY = "ExcludedCompany";
    public const string XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT = "Comment";
    public const string XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK = "FileMask";
		protected const string XML_ELEMENT_FILE = "File";
		protected const string XML_ELEMENT_DIRECTORY = "Directory";
		protected const string XML_ELEMENT_ATTRIBUTE_NAME = "Name";
		protected const string XML_ELEMENT_ATTRIBUTE_CREATIONTIME = "CreationTime";
		protected const string XML_ELEMENT_ATTRIBUTE_LASTWRITETIME = "LastWriteTime";
		protected const string XML_ELEMENT_ATTRIBUTE_ATTRIBUTES = "Attributes";
		protected const string XML_ELEMENT_ATTRIBUTE_FILELENGTH = "Length";
    protected const string XML_ELEMENT_ATTRIBUTE_FILESIG = "Signature";
    protected const string XML_ELEMENT_ATTRIBUTE_OPRESULT = "OperationResult";
    protected const string XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED = "SignatureVerified";
    protected const string XML_ELEMENT_ATTRIBUTE_FILES = "Files";
		protected const string XML_ELEMENT_ATTRIBUTE_DIRECTORIES = "Directories";
    protected const string XML_ELEMENT_ATTRIBUTE_ERROR = "Error";
		private string m_sSearchPattern = "";
		private string m_sComment = "";
    public bool m_bExcludeEmptyDirs = true;
		public bool m_bLowerCaseNames;
		public bool m_bTimeInTicks = true;
    public bool m_bOnlyCorrupted;
    public bool m_bTryToFixCorruptedFiles;
    private string m_sExcludedCompany = "";
    public string m_sDriverFolder;

    WorkControlHelper workControl;

    protected XmlDocument m_xDocument;
		public string FileMask
		{
			get
			{
				return m_sSearchPattern;
			}
			set
			{
        m_sSearchPattern = value ?? string.Empty;
			}
		}
		public string Comment
		{
			get
			{
				return m_sComment;
			}
			set
			{
				m_sComment = value ?? string.Empty;
			}
		}
    public string ExcludedCompany
    {
      get
      {
        return m_sExcludedCompany;
      }
      set
      {
        m_sExcludedCompany = value ?? string.Empty;
      }
    }
    public XmlDocument Document
		{
			get
			{
				return m_xDocument;
			}
		}
		private XmlElement ProcessDirectory(DirectoryInfo sDirectoryInfo, int depth)
		{
      string sName = sDirectoryInfo.FullName;// (bTopLevel) ? sDirectoryInfo.FullName : sDirectoryInfo.Name;
			if (m_bLowerCaseNames)
			  sName = sName.ToLower();
			XmlElement xDir = m_xDocument.CreateElement(XML_ELEMENT_DIRECTORY);
			xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_NAME, sName);
      if (!workControl.NeedStop)
      {
        string sBrowseErr = null;
        FileInfo[] fl;
        try
        {
          fl = (string.IsNullOrEmpty(m_sSearchPattern)) ? sDirectoryInfo.GetFiles() : sDirectoryInfo.GetFiles(m_sSearchPattern);
        }
        catch (Exception e)
        {
          fl = null;
          sBrowseErr = e.Message;
        }
        if (fl != null)
        {
          xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_CREATIONTIME, (m_bTimeInTicks) ? sDirectoryInfo.CreationTimeUtc.Ticks.ToString() : sDirectoryInfo.CreationTimeUtc.ToString());
          xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_LASTWRITETIME, (m_bTimeInTicks) ? sDirectoryInfo.LastWriteTimeUtc.Ticks.ToString() : sDirectoryInfo.LastWriteTimeUtc.ToString());
          xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_ATTRIBUTES, sDirectoryInfo.Attributes.ToString());
          xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_FILES, fl.Length.ToString());
          foreach (FileInfo f in fl)
          {
            if (workControl.NeedStop)
              break;
            sName = f.Name;
            if (m_bLowerCaseNames)
              sName = sName.ToLower();
            XmlElement xFile = m_xDocument.CreateElement(XML_ELEMENT_FILE);
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_NAME, sName);
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_CREATIONTIME, (m_bTimeInTicks) ? f.CreationTimeUtc.Ticks.ToString() : f.CreationTimeUtc.ToString());
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_LASTWRITETIME, (m_bTimeInTicks) ? f.LastWriteTimeUtc.Ticks.ToString() : f.LastWriteTimeUtc.ToString());
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_ATTRIBUTES, f.Attributes.ToString());
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_FILELENGTH, f.Length.ToString());

            bool bCompany = string.IsNullOrWhiteSpace(m_sExcludedCompany);
            if (!bCompany)
              try
              {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(f.FullName);
                bCompany = !m_sExcludedCompany.Equals(myFileVersionInfo.CompanyName, StringComparison.CurrentCultureIgnoreCase);
              }
              catch
              {
                bCompany = true;
              }
            string sfcert = "0";
            DigitalSignatureVerifier.WinVerifyTrustResult wvtr = DigitalSignatureVerifier.WinVerifyTrustResult.FileError;
            if (f.Length > 0)
            {
              X509Certificate sig = null;
              try
              {
                sig = X509Certificate.CreateFromSignedFile(f.FullName);
              }
              catch { }
              if (sig != null)
                sfcert = sig.GetCertHashString();

              wvtr = DigitalSignatureVerifier.WinVerifyTrust(f.FullName);
            }
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_FILESIG, sfcert);
            CompressedFile.OperationResult opres = CompressedFile.OperationResult.NoOperation;
            sfcert = opres.ToString();
            if (m_bTryToFixCorruptedFiles)
            {
              var compressedFile = new CompressedFile(f.FullName, m_sDriverFolder);
              opres = compressedFile.Check(ref wvtr);
              sfcert = opres.ToString();
              string oerr = compressedFile.OperationError;
              if (!string.IsNullOrWhiteSpace(oerr))
                sfcert += CompressedFile.ERROR_MESSAGE_DELIMITER + oerr;
            }
            xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_FILESIGVERIFIED, wvtr.ToString());
            if (opres != CompressedFile.OperationResult.NoOperation)
              xFile.SetAttribute(XML_ELEMENT_ATTRIBUTE_OPRESULT, sfcert);
            if ((opres != CompressedFile.OperationResult.NoOperation) || ((!m_bOnlyCorrupted || (wvtr != DigitalSignatureVerifier.WinVerifyTrustResult.Success) && (wvtr != DigitalSignatureVerifier.WinVerifyTrustResult.FileError) && (wvtr != DigitalSignatureVerifier.WinVerifyTrustResult.SubjectFormUnknown) && (wvtr != DigitalSignatureVerifier.WinVerifyTrustResult.NoSignature)) && bCompany))
              xDir.AppendChild(xFile);
          }
          if (!workControl.NeedStop)
          {
            DirectoryInfo[] ds = sDirectoryInfo.GetDirectories();
            int c = ds.Length;
            xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_DIRECTORIES, c.ToString());
            if ((depth != 0) && (c > 0))
            {
              depth--;
              foreach (DirectoryInfo d in ds)
              {
                XmlElement d1 = ProcessDirectory(d, depth);
                if (!m_bExcludeEmptyDirs || d1.HasChildNodes)
                  xDir.AppendChild(d1);
              }
            }
          }
        }
        else if (!string.IsNullOrEmpty(sBrowseErr))
          xDir.SetAttribute(XML_ELEMENT_ATTRIBUTE_ERROR, sBrowseErr);
      }
      return xDir;
		}
		public XmlDocument CollectInfo(DirectoryInfo sDirectoryInfo, int depth)
		{
      workControl.NeedStop = false;
      workControl.IsRunning = true;
      m_xDocument = null;
      if (sDirectoryInfo != null)
			{
				m_xDocument = new XmlDocument();
				//XmlProcessingInstruction xPI = m_xDocument.CreateProcessingInstruction("xml", XML_PROCESSING_INSTRUCTION + " encoding=\"" + System.Text.Encoding.UTF8.WebName + "\"");
				//m_xDocument.AppendChild(xPI);
				XmlComment xComment = m_xDocument.CreateComment(XML_COMMENT);
				m_xDocument.AppendChild(xComment);
				XmlElement xElmntRoot = m_xDocument.CreateElement(XML_ELEMENT_ROOT);
				xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_TIMEFORMAT, m_bTimeInTicks.ToString());
				xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_LOWERCASE, m_bLowerCaseNames.ToString());
        xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_STARTTIME, (m_bTimeInTicks) ? DateTime.UtcNow.Ticks.ToString() : DateTime.UtcNow.ToString());
				xElmntRoot.AppendChild(ProcessDirectory(sDirectoryInfo, depth));
				xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_ENDTIME, (m_bTimeInTicks) ? DateTime.UtcNow.Ticks.ToString() : DateTime.UtcNow.ToString());
				if (!string.IsNullOrWhiteSpace(m_sComment))
					xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_COMMENT, m_sComment);
        if (!string.IsNullOrWhiteSpace(m_sSearchPattern))
          xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_FILEMASK, m_sSearchPattern);
        xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_BADSIG, m_bOnlyCorrupted.ToString());
        if (!string.IsNullOrWhiteSpace(m_sExcludedCompany))
          xElmntRoot.SetAttribute(XML_ELEMENT_ROOT_ATTRIBUTE_EXCLUDED_COMPANY, m_sExcludedCompany);
        m_xDocument.AppendChild(xElmntRoot);
			}
      workControl.NeedStop = false;
      workControl.IsRunning = false;
      return m_xDocument;
		}
		public XmlDocument CollectInfo(string sDirectoryPath, int depth)
		{
			if (!string.IsNullOrEmpty(sDirectoryPath) && Directory.Exists(sDirectoryPath))
				return CollectInfo(new DirectoryInfo(sDirectoryPath), depth);
			return null;
		}
    public XmlDocument CollectInfo(DirectoryInfo sDirectoryInfo)
		{
      return CollectInfo(sDirectoryInfo, -1);
    }
    public XmlDocument CollectInfo(string sDirectoryPath)
		{
      return CollectInfo(sDirectoryPath, -1);
    }
    public DirectoryInfoDatabase(WorkControlHelper wc = null)
    {
      workControl = wc;
    }
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, WorkControlHelper wc = null)
    {
      workControl = wc;
      CollectInfo(sDirectoryInfo);
    }
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, WorkControlHelper wc = null)
    {
      workControl = wc;
      Comment = sComment;
      CollectInfo(sDirectoryInfo);
    }
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			CollectInfo(sDirectoryInfo);
		}
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			m_bTimeInTicks = bTimeInTicks;
			CollectInfo(sDirectoryInfo);
		}
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			m_bTimeInTicks = bTimeInTicks;
			m_bLowerCaseNames = bLowerCaseNames;
			CollectInfo(sDirectoryInfo);
		}
		public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, bool onlyCorrupted = false, string excludedCompany = null, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			m_bTimeInTicks = bTimeInTicks;
			m_bLowerCaseNames = bLowerCaseNames;
      m_bOnlyCorrupted = onlyCorrupted;
      ExcludedCompany = excludedCompany;
      CollectInfo(sDirectoryInfo, depth);
		}
    public DirectoryInfoDatabase(DirectoryInfo sDirectoryInfo, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, string excludedCompany, WorkControlHelper wc = null)
    {
      workControl = wc;
      Comment = sComment;
      FileMask = sFileMask;
      m_bTimeInTicks = bTimeInTicks;
      m_bLowerCaseNames = bLowerCaseNames;
      ExcludedCompany = excludedCompany;
      CollectInfo(sDirectoryInfo, depth);
    }
    public DirectoryInfoDatabase(string sDirectoryPath, WorkControlHelper wc = null)
    {
      workControl = wc;
      CollectInfo(sDirectoryPath);
    }
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, WorkControlHelper wc = null)
    {
      workControl = wc;
      Comment = sComment;
      CollectInfo(sDirectoryPath);
    }
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, string sFileMask, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			CollectInfo(sDirectoryPath);
		}
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			m_bTimeInTicks = bTimeInTicks;
			CollectInfo(sDirectoryPath);
		}
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, WorkControlHelper wc = null)
		{
      workControl = wc;
      Comment = sComment;
			FileMask = sFileMask;
			m_bTimeInTicks = bTimeInTicks;
			m_bLowerCaseNames = bLowerCaseNames;
			CollectInfo(sDirectoryPath);
		}
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, bool onlyCorrupted = false, string excludedCompany = null, WorkControlHelper wc = null)
    {
      workControl = wc;
      Comment = sComment;
      FileMask = sFileMask;
      m_bTimeInTicks = bTimeInTicks;
      m_bLowerCaseNames = bLowerCaseNames;
      m_bOnlyCorrupted = onlyCorrupted;
      ExcludedCompany = excludedCompany;
      CollectInfo(sDirectoryPath, depth);
    }
    public DirectoryInfoDatabase(string sDirectoryPath, string sComment, string sFileMask, bool bTimeInTicks, bool bLowerCaseNames, int depth, string excludedCompany, WorkControlHelper wc = null)
    {
      workControl = wc;
      Comment = sComment;
      FileMask = sFileMask;
      m_bTimeInTicks = bTimeInTicks;
      m_bLowerCaseNames = bLowerCaseNames;
      ExcludedCompany = excludedCompany;
      CollectInfo(sDirectoryPath, depth);
    }
  }
}
