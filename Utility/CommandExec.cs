using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utility
{
    /// <summary>
    /// Utility to execute command lines from within .NET
    /// </summary>
    public static class CommandExec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteCommandSync));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                // Log the exception
                Debug.WriteLine(objException.ToString());
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
                Debug.WriteLine(objException.ToString());
            }
            catch (Exception ex)
            {
                // Log the exception
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
