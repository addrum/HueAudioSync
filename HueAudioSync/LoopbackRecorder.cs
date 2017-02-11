// adapted from http://www.blakepell.com/2013-07-26-naudio-loopback-record-what-you-hear-through-the-speaker

using System.Diagnostics;
using NAudio.Wave;

namespace HueAudioSync
{
    /// <summary>
    /// A wrapper for the WasapiLoopbackCapture that will implement basic recording to a file that is overwrite only.
    /// </summary>
    public class LoopbackRecorder
    {
        private IWaveIn _waveIn;
        private bool _isRecording;

        /// <summary>
        /// Starts the recording.
        /// </summary>
        public void StartRecording()
        {
            // If we are currently record then go ahead and exit out.
            if (_isRecording)
            {
                return;
            }
            _waveIn = new WasapiLoopbackCapture();
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
            _waveIn.StartRecording();
            _isRecording = true;
        }

        /// <summary>
        /// Stops the recording
        /// </summary>
        public void StopRecording()
        {
            if (_waveIn == null)
            {
                return;
            }
            _waveIn.StopRecording();
        }

        /// <summary>
        /// Event handled when recording is stopped.  We will clean up open objects here that are required to be 
        /// closed and/or disposed of.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_waveIn != null)
            {
                _waveIn.Dispose();
                _waveIn = null;
            }
            _isRecording = false;
            if (e.Exception != null)
            {
                throw e.Exception;
            }
        } // end void OnRecordingStopped

        /// <summary>
        /// Event handled when data becomes available.  The data will be written out to disk at this point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            Debug.WriteLine(e.BytesRecorded);
        }

        private string _fileName = "";
        /// <summary>
        /// The name of the file that was set when StartRecording was called.  E.g. the current file being written to.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
        }
    }
}
