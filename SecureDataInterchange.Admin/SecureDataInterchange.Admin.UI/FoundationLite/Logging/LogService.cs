namespace Snsc.Logging
{
    // TODO: Replace stub with full implementation.
    // LogService previously wrote to a central logging database via the removed Snsc.Base package.
    public class LogService
    {
        public void Log(string application, string subject, string detail,
            string user = "", string p1 = "", string p2 = "", string p3 = "",
            string p4 = "", string p5 = "", string p6 = "", string p7 = "",
            string p8 = "", string p9 = "")
        {
            // TODO: Wire up to a real logging mechanism.
            System.Diagnostics.Trace.WriteLine(
                string.Format("[{0}] {1}: {2}", application, subject, detail));
        }
    }
}
