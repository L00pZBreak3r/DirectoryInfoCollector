using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DirectoryInfoDatabaseLib
{
  internal static class NativeMethods
  {
    internal enum UnionChoice { File = 1, Catalog, Blob, Signer, Cert };
    internal enum UiChoice { All = 1, NoUI, NoBad, NoGood };
    internal enum RevocationCheckFlags { None, WholeChain };
    internal enum StateAction { Ignore, Verify, Close, AutoCache, AutoCacheFlush };
    internal enum TrustProviderFlags
    {
      UseIE4Trust = 1,
      NoIE4Chain = 2,
      NoPolicyUsage = 4,
      RevocationCheckNone = 16,
      RevocationCheckEndCert = 32,
      RevocationCheckChain = 64,
      RecovationCheckChainExcludeRoot = 128,
      Safer = 256,
      HashOnly = 512,
      UseDefaultOSVerCheck = 1024,
      LifetimeSigning = 2048,
      CacheOnlyUrlRetrieval = 4096,
      DisableMD2MD4 = 8192
    };
    internal enum UIContext { Execute = 0, Install };
    internal enum WssFlags { VerifySpecific = 1, GetSecondarySigCount };

    internal enum TrustE : uint { Success = 0, FileError = 0x80092003, BadMsg = 0x8009200d, SecuritySettings = 0x80092026, ASN1BadTag = 0x8009310b, CounterSigner = 0x80096003, BadDigest = 0x80096010, Unknown = 0x800b0000, ProviderUnknown, ActionUnknown, SubjectFormUnknown, SubjectNotTrusted, NoSignature = 0x800b0100, CertExpired = 0x800b0101, CertUntrustedRoot = 0x800b0109, CertChaining = 0x800b010a, CertRevoked = 0x800b010c, CertWrongUsage = 0x800b0110, ExplicitDistrust = 0x800b0111 };

#if USE_WINTRUST_SIGNATURE_SETTINGS
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINTRUST_SIGNATURE_SETTINGS
    {
      int cbStruct;
      int dwIndex;
      WssFlags dwFlags;
      int cSecondarySigs;
      int dwVerifiedSigIndex;
      IntPtr pCryptoPolicy;
    }
#endif

    internal enum SE_OBJECT_TYPE
    {
      SE_UNKNOWN_OBJECT_TYPE,
      SE_FILE_OBJECT,
      SE_SERVICE,
      SE_PRINTER,
      SE_REGISTRY_KEY,
      SE_LMSHARE,
      SE_KERNEL_OBJECT,
      SE_WINDOW_OBJECT,
      SE_DS_OBJECT,
      SE_DS_OBJECT_ALL,
      SE_PROVIDER_DEFINED_OBJECT,
      SE_WMIGUID_OBJECT,
      SE_REGISTRY_WOW64_32KEY
    };

    internal const int ERROR_SUCCESS = 0x0;

    internal const int OWNER_SECURITY_INFORMATION = 0x1;

    internal const int SID_SIZE = 64;

    internal const int TOKEN_QUERY = 0x8;
    internal const int TOKEN_ADJUST_PRIVILEGES = 0x20;
    internal const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
    internal const string SE_RESTORE_NAME = "SeRestorePrivilege";
    internal const int SE_PRIVILEGE_ENABLED = 0x2;

    internal enum FSCTL_GET_COMPRESSION
    {
      FSCTL_GET_COMPRESSION = 0x9003C
    };

    internal enum FSCTL_SET_COMPRESSION
    {
      FSCTL_SET_COMPRESSION = 0x9C040
    };

    internal enum COMPRESSION_FORMAT : short
    {
      COMPRESSION_FORMAT_NONE,
      COMPRESSION_FORMAT_DEFAULT,
      COMPRESSION_FORMAT_LZNT1
    };

    internal enum SID_NAME_USE
    {
      SidTypeUndefined,
      SidTypeUser,
      SidTypeGroup,
      SidTypeDomain,
      SidTypeAlias,
      SidTypeWellKnownGroup,
      SidTypeDeletedAccount,
      SidTypeInvalid,
      SidTypeUnknown,
      SidTypeComputer,
      SidTypeLabel
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SID_IDENTIFIER_AUTHORITY
    {
      public byte Value0;
      public byte Value1;
      public byte Value2;
      public byte Value3;
      public byte Value4;
      public byte Value5;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct SID
    {
      public byte Revision;
      public byte SubAuthorityCount;
      public SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
      public short SubAuthority;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct LUID_AND_ATTRIBUTES
    {
      public long Luid;
      public int Attributes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TOKEN_PRIVILEGES
    {
      private int PrivilegeCount;
      private LUID_AND_ATTRIBUTES Privilege1;
      private LUID_AND_ATTRIBUTES Privilege2;

      internal TOKEN_PRIVILEGES(long luidPrivilege1, long luidPrivilege2)
      {
        PrivilegeCount = 2;
        Privilege1.Luid = luidPrivilege1;
        Privilege1.Attributes = SE_PRIVILEGE_ENABLED;
        Privilege2.Luid = luidPrivilege2;
        Privilege2.Attributes = SE_PRIVILEGE_ENABLED;
      }
    }

    #region Win32 API

    [DllImport("Wintrust.dll")]
    internal static extern uint WinVerifyTrust(IntPtr hWnd, IntPtr pgActionID, IntPtr pWinTrustData);

    //DWORD SetNamedSecurityInfo(
    // LPTSTR pObjectName,
    // SE_OBJECT_TYPE ObjectType,
    // SECURITY_INFORMATION SecurityInfo,
    // PSID psidOwner,
    // PSID psidGroup,
    // PACL pDacl,
    // PACL pSacl
    //);
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetNamedSecurityInfo(string pObjectName, SE_OBJECT_TYPE ObjectType, int SecurityInfo, IntPtr psidOwner, IntPtr psidGroup, IntPtr pDacl, IntPtr pSacl);

    //BOOL LookupAccountName(
    // LPCTSTR lpSystemName,
    // LPCTSTR lpAccountName,
    // PSID Sid,
    // LPDWORD cbSid,
    // LPTSTR ReferencedDomainName,
    // LPDWORD cchReferencedDomainName,
    // PSID_NAME_USE peUse
    //);
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int LookupAccountName(string lpSystemName, string lpAccountName, IntPtr Sid, ref int cbSid, string lpReferencedDomainName, ref int cchReferencedDomainName, ref SID_NAME_USE peUse);

    //[DllImport("kernel32", SetLastError = true)]
    //internal static extern IntPtr LocalFree(IntPtr hMem);

    //BOOL WINAPI CloseHandle(
    //_In_ HANDLE hObject
    //);
    [DllImport("kernel32", SetLastError = true)]
    internal static extern int CloseHandle(IntPtr hObject);

    //BOOL WINAPI OpenProcessToken(
    //_In_   HANDLE ProcessHandle,
    //_In_   DWORD DesiredAccess,
    //_Out_  PHANDLE TokenHandle
    //);
    [DllImport("advapi32.dll", SetLastError = true)]
    internal static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

    //BOOL WINAPI LookupPrivilegeValue(
    //_In_opt_  LPCTSTR lpSystemName,
    //_In_      LPCTSTR lpName,
    //_Out_     PLUID lpLuid
    //);
    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

    //BOOL WINAPI AdjustTokenPrivileges(
    //_In_       HANDLE TokenHandle,
    //_In_       BOOL DisableAllPrivileges,
    //_In_opt_   PTOKEN_PRIVILEGES NewState,
    //_In_       DWORD BufferLength,
    //_Out_opt_  PTOKEN_PRIVILEGES PreviousState,
    //_Out_opt_  PDWORD ReturnLength
    //);
    [DllImport("advapi32.dll", SetLastError = true)]
    internal static extern int AdjustTokenPrivileges(IntPtr TokenHandle, int DisableAllPrivileges, ref TOKEN_PRIVILEGES NewState, int BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

    //BOOL WINAPI DeviceIoControl(
    //_In_ HANDLE hDevice,
    //_In_ DWORD dwIoControlCode,
    //_In_opt_ LPVOID lpInBuffer,
    //_In_ DWORD nInBufferSize,
    //_Out_opt_ LPVOID lpOutBuffer,
    //_In_ DWORD nOutBufferSize,
    //_Out_opt_ LPDWORD lpBytesReturned,
    //_Inout_opt_ LPOVERLAPPED lpOverlapped
    //);
    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
    internal static extern int DeviceIoControlGetCompress(SafeFileHandle hDevice, FSCTL_GET_COMPRESSION dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, ref short lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
    internal static extern int DeviceIoControlSetCompress(SafeFileHandle hDevice, FSCTL_SET_COMPRESSION dwIoControlCode, ref short lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);


    #endregion Win32 API

  }

}
