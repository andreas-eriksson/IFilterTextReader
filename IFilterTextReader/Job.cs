﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IFilterTextReader
{
    /// <summary>
    /// Use this class to sandbox Adobe IFilter 11 or higher when you want to use this code on Windows 2012 or higher
    /// </summary>
    public class Job : IDisposable
    {
        #region Fields
        private IntPtr _jobHandle;
        private bool _disposed;
        #endregion

        #region Constructor
        public Job()
        {
            _jobHandle = NativeMethods.CreateJobObject(IntPtr.Zero, null);

            var info = new NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = 0x2000
            };

            var extendedInfo = new NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = info
            };

            var length = Marshal.SizeOf(typeof(NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

            if (!NativeMethods.SetInformationJobObject(_jobHandle, NativeMethods.JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length))
                throw new Exception(string.Format("Unable to set information.  Error: {0}", Marshal.GetLastWin32Error()));
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Disposes this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this object
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
            }

            NativeMethods.CloseHandle(_jobHandle);
            _jobHandle = IntPtr.Zero;
            _disposed = true;
        }
        #endregion

        #region AddProcess
        /// <summary>
        /// Add a process to the job by it's <see cref="processHandle"/>
        /// </summary>
        /// <param name="processHandle"></param>
        /// <returns></returns>
        public bool AddProcess(IntPtr processHandle)
        {
            return NativeMethods.AssignProcessToJobObject(_jobHandle, processHandle);
        }

        /// <summary>
        /// Add a process to the job by it's <see cref="processId"/>
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public bool AddProcess(int processId)
        {
            return AddProcess(Process.GetProcessById(processId).Handle);
        }
        #endregion
    }
}