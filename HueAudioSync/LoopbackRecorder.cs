// adapted from http://www.blakepell.com/2013-07-26-naudio-loopback-record-what-you-hear-through-the-speaker

using System;
using System.Diagnostics;
using NAudio.Wave;

namespace HueAudioSync
{
    /// <summary>
    /// A wrapper for the WasapiLoopbackCapture that will implement basic recording to a file that is overwrite only.
    /// </summary>
    public class LoopbackRecorder
    {
        public static int FftLength { get; } = 8192;

        private bool _isRecording;
        private IWaveIn _waveIn;
        private SampleAggregator _sampleAggregator;
        //private WaveFileWriter _writer;

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

            _sampleAggregator = new SampleAggregator(FftLength);
            _sampleAggregator.FftCalculated += FftCalculated;
            _sampleAggregator.PerformFFT = true;

            //Debug.WriteLine(WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice().FriendlyName);
            //_writer = new WaveFileWriter("C:\\users\\adams\\downloads\\test.wav", _waveIn.WaveFormat);

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
        public void OnRecordingStopped(object sender, StoppedEventArgs e)
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
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            //Debug.WriteLine(e.BytesRecorded);
            //_writer.Write(e.Buffer, 0, e.BytesRecorded);

            //var buffer = e.Buffer;
            //var bytesRecorded = e.BytesRecorded;
            //var bufferIncrement = _waveIn.WaveFormat.BlockAlign;

            //for (var index = 0; index < bytesRecorded; index += bufferIncrement)
            //{
            //    var sample32 = BitConverter.ToSingle(buffer, index);
            //    if (sample32 > 0)
            //    {
            //        Debug.WriteLine(sample32);
            //    }
            //}

            var buffer = e.Buffer;
            var bytesRecorded = e.BytesRecorded;
            var bufferIncrement = _waveIn.WaveFormat.BlockAlign;

            for (var index = 0; index < bytesRecorded; index += bufferIncrement)
            {
                var sample32 = BitConverter.ToSingle(buffer, index);
                _sampleAggregator.Add(sample32);
            }
        }

        private void FftCalculated(object sender, FftEventArgs e)
        {
            // Do something with e.result!
            Debug.WriteLine("x: {0}, y: {1}", e.Result[e.Result.Length - 1].X, e.Result[e.Result.Length - 1].Y);
        }
    }
}
