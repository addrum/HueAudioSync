using System.Configuration;
using System.Diagnostics;
using System.Timers;
using HueAudioSync.Properties;
using SharpHue;

namespace HueAudioSync
{
    internal class BridgeConnector
    {
        public BridgeConnector()
        {
            //var userRegistered = false;
            var userRegistered = InitialiseExistingUser();
            if (!userRegistered)
            {
                Debug.WriteLine("Couldn't register existing user, attempting to register new user...");
                userRegistered = RegisterNewUser();
            }
            if (userRegistered)
            {
                LightsController.CreateLights();
            }
            else
            {
                Debug.WriteLine("Couldn't register any user");
            }
        }

        private static bool InitialiseExistingUser()
        {
            try
            {
                var username = Settings.Default["Username"].ToString();
                Configuration.Initialize(username);
                Debug.WriteLine($"Initialised Hue connection with user: {Configuration.Username}");
                return true;
            }
            catch (HueApiException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (SettingsPropertyNotFoundException e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        private static bool RegisterNewUser()
        {
            Debug.WriteLine("Press button on hue bridge now!");
            var timer = new Timer();
            timer.Interval = 30000;
            timer.Start();

            var linked = false;
            while (!linked && timer.Enabled)
            {
                try
                {
                    Configuration.AddUser("HueAudioSync");
                    Settings.Default["Username"] = Configuration.Username;
                    Settings.Default.Save();
                    linked = true;
                    timer.Stop();
                    Debug.WriteLine("Bridge linked!");
                    return true;
                }
                catch (HueApiException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return false;
        }
    }
}
