using System;
using System.Windows.Forms;

namespace HueAudioSync
{
    internal static class Program
    {
        private static LoopbackRecorder _recorder;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            _recorder = new LoopbackRecorder();
            _recorder.StartRecording();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _recorder.StopRecording();
        }
    }
}
