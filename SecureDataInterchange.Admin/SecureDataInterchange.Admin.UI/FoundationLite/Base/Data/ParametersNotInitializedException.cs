/********************************************
Copyright  2015, SN Servicing, Inc.
********************************************/

using System;
using System.Runtime.Serialization;

namespace Snsc.Base.Data
{
    /// <summary>
    /// Exception thrown when parameters have not been initialized.
    /// </summary>
    [Serializable]
    public class ParametersNotInitializedException : Exception
    {
        public ParametersNotInitializedException(StoredProcedureExecutorBase executor)
            : base(string.Format("Parameters have not been initialized for {0}", executor.GetType().Name))
        {
        }

        protected ParametersNotInitializedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}