// adapted from http://www.blakepell.com/2013-07-26-naudio-loopback-record-what-you-hear-through-the-speaker

using System;
using System.Diagnostics;
using NAudio.Wave;
using VoiceRecorder.Audio;

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

            _sampleAggregator = new SampleAggregator();

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
            var buffer = e.Buffer;
            float sample32 = 0;

            for (var index = 0; index < e.BytesRecorded; index += 2)
            {
                var sample = (short)((buffer[index + 1] << 8) | buffer[index + 0]);
                sample32 = sample / 32768f;
                Debug.WriteLine(sample32);
                LightsController.SetLights(Convert.ToByte(Math.Abs(sample32) * 255));
                _sampleAggregator.Add(sample32);
            }
            var floats = BytesToFloats(buffer);

            var pitchDetect = new FftPitchDetector(sample32);
            var pitch = pitchDetect.DetectPitch(floats, floats.Length);
            Debug.WriteLine($"Pitch {pitch}");
        }

        private static float[] BytesToFloats(byte[] bytes)
        {
            var floats = new float[bytes.Length / 2];
            for (var i = 0; i < bytes.Length; i += 2)
            {
                floats[i / 2] = bytes[i] | (bytes[i + 1] << 8);
            }

            return floats;
        }

        //// http://stackoverflow.com/a/20414331/1860436
        //private void FftCalculated(object sender, FftEventArgs e)
        //{
        //    var x = e.Result[e.Result.Length - 1].X;
        //    var y = e.Result[e.Result.Length - 1].Y;
        //    Debug.WriteLine($"x: {x}, y: {y}");
        //    LightsController.SetLights(x* 100, y * 100);
        //}
    }
}
