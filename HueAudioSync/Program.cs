using System;
using System.Windows.Forms;

namespace HueAudioSync
{
    internal class Program
    {
        private readonly LoopbackRecorder _recorder;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            var program = new Program();
        }

        public Program()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            var formController = new FormController();

            _recorder = new LoopbackRecorder();
            _recorder.StartRecording();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            _recorder?.StopRecording();
        }
    }
}
