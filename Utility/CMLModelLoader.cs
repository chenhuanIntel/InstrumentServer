using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility
{
    /// <summary>
    /// Class to load Machine Learning Model to get predicted value.
    /// </summary>
    public static class CMLModelLoader
    {
        /// <summary>
        /// Trigger to load Onnx file to run Machine Learning Model to get predicted value.
        /// </summary>
        /// <param name="FilePath">Onnx file Path</param>
        /// <param name="Data">Input data for Onnx</param>
        /// <param name="DutIndex">Index of DUT Slot</param>
        /// <returns>Feedback Predicted value</returns>
        public static float[] RunMLModel(string FilePath, float[] Data, int DutIndex)
        {
            string sOutput = "";
            string sInput = String.Join(" ", Data);
            ProcessStartInfo procStartInfo = new ProcessStartInfo();
            procStartInfo.WorkingDirectory = $"{Environment.CurrentDirectory}\\net6.0";
            procStartInfo.Arguments = $"\"{FilePath}\" {sInput}";
            procStartInfo.FileName = $"{Environment.CurrentDirectory}\\net6.0\\MachineLearningApp.exe";
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            string Lines = proc.StandardOutput.ReadToEnd();
            foreach (string sline in Lines.Split('\n'))
            {
                clog.Log(sline, DutIndex);
                if (sline.StartsWith("Result:")) sOutput = sline.Replace("Result: ", "");
            }

            if (sOutput != "")
            {
                string[] arOutput = sOutput.Trim().Split(',');
                float[] OutputData = new float[arOutput.Length];
                for (int i = 0; i < arOutput.Length; i++)
                {
                    OutputData[i] = float.Parse(arOutput[i]);
                }
                return OutputData;
            }
            else
            {
                if (Lines == "") clog.Log($"Warning: Please ensure you have installed .Net6 Runtime.", DutIndex);
                return null;
            }
        }
    }
}
