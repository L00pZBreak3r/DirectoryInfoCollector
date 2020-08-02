using System;
using System.Runtime.InteropServices;

namespace DirectoryInfoDatabaseLib
{
  public static class DigitalSignatureVerifier
  {
    private sealed class UnmanagedPointer : IDisposable
    {
      internal enum AllocMethod { HGlobal, CoTaskMem };

      private IntPtr m_ptr;
      private AllocMethod m_meth;
      private Type m_type;
      private bool m_struct;

      internal UnmanagedPointer(Type objtype, bool allocstruct = false, AllocMethod method = AllocMethod.HGlobal)
      {
        m_type = objtype;
        m_meth = method;
        m_struct = allocstruct;
        m_ptr = (m_meth == AllocMethod.CoTaskMem) ? Marshal.AllocCoTaskMem(Marshal.SizeOf(objtype)) : Marshal.AllocHGlobal(Marshal.SizeOf(objtype));
      }

      ~UnmanagedPointer()
      {
        Dispose(false);
      }

      #region IDisposable Members
      private void Dispose(bool disposing)
      {
        if (m_ptr != IntPtr.Zero)
        {
          if (m_struct)
            Marshal.DestroyStructure(m_ptr, m_type);
          if (m_meth == AllocMethod.HGlobal)
          {
            Marshal.FreeHGlobal(m_ptr);
          }
          else if (m_meth == AllocMethod.CoTaskMem)
          {
            Marshal.FreeCoTaskMem(m_ptr);
          }
          m_ptr = IntPtr.Zero;
        }
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      #endregion

      public static implicit operator IntPtr(UnmanagedPointer ptr)
      {
        return ptr.m_ptr;
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WINTRUST_FILE_INFO : IDisposable
    {
      public int cbStruct;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pcwszFilePath;
      public IntPtr hFile;
      public IntPtr pgKnownSubject;

      public WINTRUST_FILE_INFO(string fileName, Guid subject)
      {
        cbStruct = Marshal.SizeOf(typeof(WINTRUST_FILE_INFO));
        pcwszFilePath = fileName;
        hFile = IntPtr.Zero;
        pgKnownSubject = IntPtr.Zero;

        if (!Guid.Empty.Equals(subject))
        {
          pgKnownSubject = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Guid)));
          Marshal.StructureToPtr(subject, pgKnownSubject, false);
        }
      }
      #region IDisposable Members

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
        if (pgKnownSubject != IntPtr.Zero)
        {
          Marshal.DestroyStructure<Guid>(pgKnownSubject);
          Marshal.FreeHGlobal(pgKnownSubject);
        }
      }

      #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WINTRUST_DATA : IDisposable
    {
      public int cbStruct;
      public IntPtr pPolicyCallbackData;
      public IntPtr pSIPCallbackData;
      private NativeMethods.UiChoice dwUIChoice;
      public NativeMethods.RevocationCheckFlags fdwRevocationChecks;
      public readonly NativeMethods.UnionChoice dwUnionChoice;
      public IntPtr pInfoStruct;
      public NativeMethods.StateAction dwStateAction;
      public IntPtr hWVTStateData;
      private IntPtr pwszURLReference;
      public NativeMethods.TrustProviderFlags dwProvFlags;
      public NativeMethods.UIContext dwUIContext;
      private IntPtr pSignatureSettings;

      public WINTRUST_DATA(WINTRUST_FILE_INFO fileInfo)
      {
        cbStruct = Marshal.SizeOf(typeof(WINTRUST_DATA));
        dwUnionChoice = NativeMethods.UnionChoice.File;

        pInfoStruct = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WINTRUST_FILE_INFO)));
        Marshal.StructureToPtr(fileInfo, pInfoStruct, false);

        pPolicyCallbackData = IntPtr.Zero;
        pSIPCallbackData = IntPtr.Zero;

        dwUIChoice = NativeMethods.UiChoice.NoUI;
        fdwRevocationChecks = NativeMethods.RevocationCheckFlags.None;
        dwStateAction = NativeMethods.StateAction.Ignore;
        hWVTStateData = IntPtr.Zero;
        pwszURLReference = IntPtr.Zero;
        dwProvFlags = NativeMethods.TrustProviderFlags.DisableMD2MD4;

        dwUIContext = NativeMethods.UIContext.Execute;

        pSignatureSettings = IntPtr.Zero;
      }

      #region IDisposable Members

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
        if (pInfoStruct != IntPtr.Zero)
        {
          if (dwUnionChoice == NativeMethods.UnionChoice.File)
            Marshal.DestroyStructure<WINTRUST_FILE_INFO>(pInfoStruct);
          Marshal.FreeHGlobal(pInfoStruct);
          pInfoStruct = IntPtr.Zero;
        }
      }

      #endregion
    }

    public enum WinVerifyTrustResult { Success, FileError, BadMsg, SecuritySettings, ASN1BadTag, CounterSigner, BadDigest, FunctionCallFailed, ProviderUnknown, ActionUnknown, SubjectFormUnknown, SubjectNotTrusted, NoSignature, CertExpired, CertUntrustedRoot, CertChaining, CertRevoked, CertWrongUsage, ExplicitDistrust };

    private static readonly Guid wintrust_action_generic_verify_v2 = new Guid("{00AAC56B-CD44-11D0-8CC2-00C04FC295EE}");
    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    public static WinVerifyTrustResult WinVerifyTrust(string fileName)
    {
      WINTRUST_FILE_INFO fileInfo = new WINTRUST_FILE_INFO(fileName, Guid.Empty);
      WINTRUST_DATA data = new WINTRUST_DATA(fileInfo);

      uint result;
#if DEBUG
      uint r2;
#endif

      using (UnmanagedPointer guidPtr = new UnmanagedPointer(typeof(Guid), true))
      using (UnmanagedPointer wvtDataPtr = new UnmanagedPointer(typeof(WINTRUST_DATA), true))
      {
        IntPtr pGuid = guidPtr;
        IntPtr pData = wvtDataPtr;

        Marshal.StructureToPtr(wintrust_action_generic_verify_v2, pGuid, false);

        data.dwStateAction = NativeMethods.StateAction.Verify;
        Marshal.StructureToPtr(data, pData, false);

        result = NativeMethods.WinVerifyTrust(INVALID_HANDLE_VALUE, pGuid, pData);

        data.dwStateAction = NativeMethods.StateAction.Close;
        Marshal.StructureToPtr(data, pData, true);
#if DEBUG
        r2 =
#endif
        NativeMethods.WinVerifyTrust(INVALID_HANDLE_VALUE, pGuid, pData);
      }

      if ((result >= (uint)NativeMethods.TrustE.ProviderUnknown) && (result <= (uint)NativeMethods.TrustE.SubjectNotTrusted))
        return (WinVerifyTrustResult)((int)(result - (uint)NativeMethods.TrustE.Unknown) + (int)WinVerifyTrustResult.FunctionCallFailed);

      switch (result)
      {
        case NativeMethods.ERROR_SUCCESS:
          return WinVerifyTrustResult.Success;
        case (uint)NativeMethods.TrustE.FileError:
          return WinVerifyTrustResult.FileError;
        case (uint)NativeMethods.TrustE.BadMsg:
          return WinVerifyTrustResult.BadMsg;
        case (uint)NativeMethods.TrustE.SecuritySettings:
          return WinVerifyTrustResult.SecuritySettings;
        case (uint)NativeMethods.TrustE.ASN1BadTag:
          return WinVerifyTrustResult.ASN1BadTag;
        case (uint)NativeMethods.TrustE.CounterSigner:
          return WinVerifyTrustResult.CounterSigner;
        case (uint)NativeMethods.TrustE.BadDigest:
          return WinVerifyTrustResult.BadDigest;
        case (uint)NativeMethods.TrustE.NoSignature:
          return WinVerifyTrustResult.NoSignature;
        case (uint)NativeMethods.TrustE.CertExpired:
          return WinVerifyTrustResult.CertExpired;
        case (uint)NativeMethods.TrustE.CertUntrustedRoot:
          return WinVerifyTrustResult.CertUntrustedRoot;
        case (uint)NativeMethods.TrustE.CertChaining:
          return WinVerifyTrustResult.CertChaining;
        case (uint)NativeMethods.TrustE.CertRevoked:
          return WinVerifyTrustResult.CertRevoked;
        case (uint)NativeMethods.TrustE.CertWrongUsage:
          return WinVerifyTrustResult.CertWrongUsage;
        case (uint)NativeMethods.TrustE.ExplicitDistrust:
          return WinVerifyTrustResult.ExplicitDistrust;
        default:
          return WinVerifyTrustResult.FunctionCallFailed;
      }
    }
  }
}
