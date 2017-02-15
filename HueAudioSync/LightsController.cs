using System;
using System.Diagnostics;
using SharpHue;

namespace HueAudioSync
{
    internal class LightsController
    {
        private static LightCollection _lights;

        public static void CreateLights()
        {
            if (_lights != null) return;
            _lights = new LightCollection();
            foreach (var light in _lights)
            {
               Debug.WriteLine($"Light: {light.Name}"); 
            }
        } 

        public static void SetLights(byte brightness)
        {
            new LightStateBuilder()
                .For(_lights[4])
                .TurnOn()
                .Brightness(brightness)
                .Apply();
        }
    }
}
