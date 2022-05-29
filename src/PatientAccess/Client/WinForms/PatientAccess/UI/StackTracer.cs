using System;
using System.Diagnostics;
using System.Text;

namespace PatientAccess.UI
{

    public static class StackTracer
    {

        /// <summary>
        /// Logs the stack trace so that we can debug the cross-thread issues. 
        /// 
        /// Instead of getting the full stack dump, which in PatientAccess could be fairly deep, 
        /// this method allows you to set the amount of the stack you wish to dump to the 
        /// breadcrumb file. The variable is called, amountOfStackRequired, and should be
        /// > 0.0 and less than 1.0.
        /// 
        /// Also this method will only dump the amount of the stack you specify if the UI control it is called on
        /// actually requires an InvokeRequired (must be true), else it does nothing.
        /// 
        /// </summary>
        /// 
        /// <param name="st"></param>
        /// <param name="invokeRequired"></param>
        /// <returns>A string with the formatted contents of the stack trace.</returns>
        public static string LogTraceLog(StackTrace st, bool invokeRequired)
        {
            StringBuilder sb1 = new StringBuilder();
            if (invokeRequired)
            {
                double amountOfStackRequired = 0.05 ;
                // get size of stack
                int stackSize = st.FrameCount-1;
                // reduce it by a desired real value [0..1] that reflects a percentage
                int softLimit = (int)(amountOfStackRequired * stackSize);

                sb1.Append("Invoke Required: " + invokeRequired + Environment.NewLine) ;
                for (int i = softLimit; i > -1; i--)
                {
                    StackFrame sf = st.GetFrame(i);
                    sb1.Append("Method Name: " + sf.GetMethod() + Environment.NewLine);
                }
            }
            return sb1.ToString();
        }

    }

}
