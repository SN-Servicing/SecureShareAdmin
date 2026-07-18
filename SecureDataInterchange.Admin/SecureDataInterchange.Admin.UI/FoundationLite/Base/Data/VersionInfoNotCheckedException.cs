/********************************************
Copyright  2015, SN Servicing, Inc.
********************************************/

using System;
using System.Runtime.Serialization;

namespace Snsc.Base.Data
{
    /// <summary>
    /// Exception thrown when version info has not been checked before execution.
    /// </summary>
    [Serializable]
    public class VersionInfoNotCheckedException : Exception
    {
        public VersionInfoNotCheckedException(StoredProcedureExecutorBase executor)
            : base(string.Format("Version info must be checked before executing {0}. Call VersionInfo method before Execute.", executor.GetType().Name))
        {
        }

        protected VersionInfoNotCheckedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}