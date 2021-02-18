using System;
using System.IO;

namespace TemperatuurLogger.Model
{

    
    public static class Utils
    {
        public const string DB3FILE = "Temperatuurlogger.db3";

        public static string GetDb3FullPath(){
            return Path.Combine(GetUserProfilePath(), DB3FILE);
        }

        public static string GetUserProfilePath()
        {
            var currentPlatform = Environment.OSVersion.Platform;
            switch (currentPlatform)
            {
                case PlatformID.Win32NT:
                    return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                case PlatformID.Unix:
                    return System.Environment.GetEnvironmentVariable("HOME");
                default:
                    throw new Exception($"Platform {currentPlatform} not supported.");
            }
        }
    }
}