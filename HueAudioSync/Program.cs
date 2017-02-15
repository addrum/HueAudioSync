using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using SharpHue;

namespace HueAudioSync
{
    internal class Program
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

            var formController = new FormController();

            var bridgeConnector = new BridgeConnector();

            _recorder = new LoopbackRecorder();
            _recorder.StartRecording();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            _recorder?.StopRecording();
        }
    }
}
