using System;
//using System.Collections.Generic;
using System.Text;
//using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DirectoryInfoDatabaseLib
{
  public class CompressedFile
  {
    public enum OperationResult
    {
      NoOperation,
      AccessChanged,
      AccessFailed,
      UncompressFailed,
      ReplaceFailed,
      AccessChangedReplaceFailed,
      UncompressSuccess,
      ReplaceSuccess
    };

    public const string ERROR_MESSAGE_DELIMITER = ";";

    //private const string TrustedInstallerAccountName = @"NT SERVICE\TrustedInstaller";
    private const string TrustedInstallerAccountSid = @"S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464";
    private const int DOMAIN_NAME_SIZE = 128;

    protected readonly string path;
    protected readonly string driverFolder;
    protected static readonly string driversPath;
    protected static readonly SecurityIdentifier sidAdminGroup;
    protected static readonly NTAccount ntAdminGroup;
    protected static readonly SecurityIdentifier sidTrustedInstaller;
    protected static readonly NTAccount ntTrustedInstaller;
    protected static readonly SecurityIdentifier sidUsersGroup;

    protected readonly StringBuilder opError = new StringBuilder();

    static CompressedFile()
    {
      sidAdminGroup = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
      ntAdminGroup = (NTAccount)sidAdminGroup.Translate(typeof(NTAccount));
      sidTrustedInstaller = new SecurityIdentifier(TrustedInstallerAccountSid);
      ntTrustedInstaller = (NTAccount)sidTrustedInstaller.Translate(typeof(NTAccount));
      sidUsersGroup = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
      driversPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers");
    }

    public CompressedFile(string fullpath, string driverfolder = null)
    {
      path = (File.Exists(fullpath)) ? fullpath : null;
      driverFolder = driverfolder;
    }

    public string FullPath
    {
      get
      {
        return path;
      }
    }

    public bool FileExists
    {
      get
      {
        return File.Exists(path);
      }
    }

    public string OperationError
    {
      get
      {
        return opError.ToString();
      }
    }

    private static string GetLastErrorString(int error = 0)
    {
      Win32Exception Win32Error = new Win32Exception((error == 0) ? Marshal.GetLastWin32Error() : error);
      return Win32Error.Message;
    }

    private bool SetPrivileges()
    {
      // get the current process's token
      IntPtr hProc = Process.GetCurrentProcess().Handle;
      IntPtr hToken = IntPtr.Zero;
      if (NativeMethods.OpenProcessToken(hProc, NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, ref hToken) == 0)
      {
        opError.Append(ERROR_MESSAGE_DELIMITER + "OpenProcessToken:" + GetLastErrorString());
        return false;
      }

      // get the LUIDs for the privileges (provided they already exists)
      long luid_TakeOwnership = 0;
      long luid_Restore = 0;
      if ((NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_TAKE_OWNERSHIP_NAME, ref luid_TakeOwnership) == 0) ||
          (NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_RESTORE_NAME, ref luid_Restore) == 0))
      {
        opError.Append(ERROR_MESSAGE_DELIMITER + "LookupPrivilegeValue:" + GetLastErrorString());
        NativeMethods.CloseHandle(hToken);
        return false;
      }

      NativeMethods.TOKEN_PRIVILEGES tp = new NativeMethods.TOKEN_PRIVILEGES(luid_TakeOwnership, luid_Restore);

      // enable the privileges
      bool res = NativeMethods.AdjustTokenPrivileges(hToken, 0, ref tp, 0, IntPtr.Zero, IntPtr.Zero) != 0;
      if (!res)
        opError.Append(ERROR_MESSAGE_DELIMITER + "AdjustTokenPrivileges:" + GetLastErrorString());

      if (NativeMethods.CloseHandle(hToken) == 0)
      {
        res = false;
        opError.Append(ERROR_MESSAGE_DELIMITER + "CloseHandle:" + GetLastErrorString());
      }
      return res;
    }

    private bool SetOwner(string strUserName)
    {
      bool res = false;

      // Do we have the required Privilege?
      if (SetPrivileges())
      {
        int sid_len = NativeMethods.SID_SIZE;
        IntPtr pNewOwner = Marshal.AllocHGlobal(sid_len);
        int domain_len = DOMAIN_NAME_SIZE;
        string domain_name = new string(' ', domain_len + 1);
        NativeMethods.SID_NAME_USE deUse = NativeMethods.SID_NAME_USE.SidTypeUndefined;

        res = (NativeMethods.LookupAccountName(null, strUserName, pNewOwner, ref sid_len, domain_name, ref domain_len, ref deUse) != 0);
        if (res)
        {
#if DEBUG
          domain_name = domain_name.Substring(0, domain_len);
#endif
          int r = NativeMethods.SetNamedSecurityInfo(path, NativeMethods.SE_OBJECT_TYPE.SE_FILE_OBJECT, NativeMethods.OWNER_SECURITY_INFORMATION, pNewOwner, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
          if (r != NativeMethods.ERROR_SUCCESS)
          {
            res = false;
            opError.Append(ERROR_MESSAGE_DELIMITER + "SetNamedSecurityInfo:" + GetLastErrorString(r));
          }
        }
        else
          opError.Append(ERROR_MESSAGE_DELIMITER + "LookupAccountName:" + GetLastErrorString());
        Marshal.FreeHGlobal(pNewOwner);
      }

      return res;
    }

    // Adds an ACL entry on the specified file for the specified account. 
    private bool AddFileSecurity(bool addRights = true, SecurityIdentifier sid = null)
    {
      bool res = true;
      // Get a FileSecurity object that represents the 
      // current security settings.
      FileSecurity fSecurity = File.GetAccessControl(path, AccessControlSections.Access);
      if (sid == null)
        sid = sidUsersGroup;
      FileSystemAccessRule rule = null;
      try
      {
        rule = new FileSystemAccessRule(sid, (addRights) ? FileSystemRights.Modify : (FileSystemRights.Write | FileSystemRights.Delete), AccessControlType.Allow);
      }
      catch (Exception ex)
      {
        opError.Append(ERROR_MESSAGE_DELIMITER + "FileSystemAccessRule:" + ex.Message);
        res = false;
      }
      if (res && (rule != null))
      {
        // Add or remove the FileSystemAccessRule to the security settings.
        if (addRights)
          fSecurity.AddAccessRule(rule);
        else
          res = fSecurity.RemoveAccessRule(rule);
        if (res)
        {
          try
          {
            // Set the new access settings.
            File.SetAccessControl(path, fSecurity);
          }
          catch (Exception ex)
          {
            opError.Append(ERROR_MESSAGE_DELIMITER + "SetAccessControl:" + ex.Message);
            res = false;
          }
        }
        else
          opError.Append(ERROR_MESSAGE_DELIMITER + "RemoveAccessRule:Failed");
      }
      return res;
    }


    private static bool CompareFiles(string path1, string path2)
    {
      if (string.Compare(path1, path2, true) == 0)
        return false;
      try
      {
        FileVersionInfo ver1 = FileVersionInfo.GetVersionInfo(path1);
        FileVersionInfo ver2 = FileVersionInfo.GetVersionInfo(path2);
        return !string.IsNullOrWhiteSpace(ver1.CompanyName) && !string.IsNullOrWhiteSpace(ver2.CompanyName) &&
          !string.IsNullOrWhiteSpace(ver1.ProductName) && !string.IsNullOrWhiteSpace(ver2.ProductName) &&
          !string.IsNullOrWhiteSpace(ver1.FileVersion) && !string.IsNullOrWhiteSpace(ver2.FileVersion) &&
          (string.CompareOrdinal(ver1.CompanyName, ver2.CompanyName) == 0) && (string.CompareOrdinal(ver1.ProductName, ver2.ProductName) == 0) && (string.CompareOrdinal(ver1.FileVersion, ver2.FileVersion) == 0);
      }
      catch
      {
        return false;
      }
    }

    private bool SetAccess(bool access)
    {
      return (access) ? (SetOwner(ntAdminGroup.Value) && AddFileSecurity(true)) : (AddFileSecurity(false) && SetOwner(ntTrustedInstaller.Value));
    }

    public bool IsCompressed
    {
      get
      {
        bool res = false;
        int result;
        short lpOutBuffer = (short)NativeMethods.COMPRESSION_FORMAT.COMPRESSION_FORMAT_NONE;
        using (FileStream f = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
          int lpBytesReturned = 0;
          result = NativeMethods.DeviceIoControlGetCompress(f.SafeFileHandle, NativeMethods.FSCTL_GET_COMPRESSION.FSCTL_GET_COMPRESSION,
          IntPtr.Zero, 0, ref lpOutBuffer, Marshal.SizeOf(lpOutBuffer),
          ref lpBytesReturned, IntPtr.Zero);
        }
        if (result != 0)
          res = lpOutBuffer == (short)NativeMethods.COMPRESSION_FORMAT.COMPRESSION_FORMAT_LZNT1;
        return res;
      }
    }

    public bool Compress(bool compress = true)
    {
      int result;
      using (FileStream f = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
      {
        short lpInBuffer = (compress) ? (short)NativeMethods.COMPRESSION_FORMAT.COMPRESSION_FORMAT_DEFAULT : (short)NativeMethods.COMPRESSION_FORMAT.COMPRESSION_FORMAT_NONE;
        int lpBytesReturned = 0;
        result = NativeMethods.DeviceIoControlSetCompress(f.SafeFileHandle, NativeMethods.FSCTL_SET_COMPRESSION.FSCTL_SET_COMPRESSION,
        ref lpInBuffer, Marshal.SizeOf(lpInBuffer), IntPtr.Zero, 0,
        ref lpBytesReturned, IntPtr.Zero);
      }
      return result != 0;
    }

    private bool Replace(string path2)
    {
      bool res = false;
      if (File.Exists(path2) && CompareFiles(path, path2))
      {
        string errmsg1 = ERROR_MESSAGE_DELIMITER + "Replace(" + Path.GetDirectoryName(path2) + "):";
        DigitalSignatureVerifier.WinVerifyTrustResult sigcheck = DigitalSignatureVerifier.WinVerifyTrust(path2);
        if (sigcheck == DigitalSignatureVerifier.WinVerifyTrustResult.Success)
        {
          try
          {
            File.Copy(path2, path, true);
            res = true;
          }
          catch (Exception ex)
          {
            opError.Append(errmsg1 + ex.Message);
          }
        }
        else
        {
          opError.Append(errmsg1 + sigcheck.ToString());
        }
      }
      return res;
    }

    private bool Replace()
    {
      string fn = Path.GetFileName(path);
      if (!string.IsNullOrWhiteSpace(driverFolder) && Directory.Exists(driverFolder) && Replace(Path.Combine(driverFolder, fn)))
        return true;
      return Replace(Path.Combine(driversPath, fn));
    }

    public OperationResult Check(ref DigitalSignatureVerifier.WinVerifyTrustResult sigcheck)
    {
      OperationResult res = OperationResult.NoOperation;
      opError.Clear();
      if (!string.IsNullOrEmpty(path))
      {
        if (sigcheck == DigitalSignatureVerifier.WinVerifyTrustResult.FunctionCallFailed)
          sigcheck = DigitalSignatureVerifier.WinVerifyTrust(path);

        bool b1 = sigcheck == DigitalSignatureVerifier.WinVerifyTrustResult.ASN1BadTag;
        bool b2 = (File.GetAttributes(path) & FileAttributes.Compressed) == FileAttributes.Compressed;
        if (b1 || b2)
        {
          var fs = File.GetAccessControl(path, AccessControlSections.Owner);
          var sid = fs.GetOwner(typeof(SecurityIdentifier));
          if (TrustedInstallerAccountSid.Equals(sid.Value, StringComparison.OrdinalIgnoreCase))
            res = (SetAccess(true)) ? OperationResult.AccessChanged : OperationResult.AccessFailed;
          if (res != OperationResult.AccessFailed)
          {
            OperationResult res2 = OperationResult.NoOperation;
            if (b2)
              res2 = (Compress(false)) ? OperationResult.UncompressSuccess : OperationResult.UncompressFailed;
            if (b1)
              res2 = (Replace()) ? OperationResult.ReplaceSuccess : OperationResult.ReplaceFailed;
            if ((res == OperationResult.AccessChanged) && SetAccess(false))
              res = OperationResult.NoOperation;
            if (res == OperationResult.NoOperation)
            {
              res = res2;
              if (res != OperationResult.ReplaceFailed)
              {
                sigcheck = DigitalSignatureVerifier.WinVerifyTrust(path);
                if ((sigcheck != DigitalSignatureVerifier.WinVerifyTrustResult.Success) && (sigcheck != DigitalSignatureVerifier.WinVerifyTrustResult.SubjectFormUnknown))
                  res = OperationResult.ReplaceFailed;
              }
            }
            else if ((res == OperationResult.AccessChanged) && (res2 == OperationResult.ReplaceFailed))
              res = OperationResult.AccessChangedReplaceFailed;
          }
        }
      }
      return res;
    }
  }
}
