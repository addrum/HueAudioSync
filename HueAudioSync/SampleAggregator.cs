using System;
using System.Diagnostics;
using NAudio.Dsp;

// http://stackoverflow.com/a/20414331/1860436
namespace HueAudioSync
{ // The Complex and FFT are here!

    internal class SampleAggregator
    {
        // FFT
        public event EventHandler<FftEventArgs> FftCalculated;
        // ReSharper disable once InconsistentNaming
        public bool PerformFFT { get; set; }

        // This Complex is NAudio's own! 
        private Complex[] _fftBuffer;
        private FftEventArgs _fftArgs;
        private int _fftPos;
        private int _fftLength;
        private int _m;

        public SampleAggregator(int fftLength)
        {
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            _m = (int)Math.Log(fftLength, 2.0);
            _fftLength = fftLength;
            _fftBuffer = new Complex[fftLength];
            _fftArgs = new FftEventArgs(_fftBuffer);
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        public void Add(float value)
        {
            if (PerformFFT && FftCalculated != null)
            {
                // Remember the window function! There are many others as well.
                _fftBuffer[_fftPos].X = (float)(value * FastFourierTransform.HammingWindow(_fftPos, _fftLength));
                _fftBuffer[_fftPos].Y = 0; // This is always zero with audio.
                _fftPos++;

                if (_fftPos >= _fftLength)
                {
                    _fftPos = 0;
                    FastFourierTransform.FFT(true, _m, _fftBuffer);
                    FftCalculated(this, _fftArgs);
                }
            }
        }
    }

    public class FftEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public FftEventArgs(Complex[] result)
        {
            Result = result;
        }
        public Complex[] Result { get; private set; }
    }
}
