using System;
using NAudio.CoreAudioApi;

namespace HueAudioSync
{
    internal class FormController
    {
        public MMDeviceCollection GetAudioEndPoints()
        {
            var devices = new MMDeviceEnumerator();

            var endPoints = devices.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in endPoints)
            {
                Console.WriteLine("{0}, {1}", device.FriendlyName, device.State);
            }

            return endPoints;
        }
    }
}
