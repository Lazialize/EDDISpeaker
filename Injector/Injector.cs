using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Injector
{
    public static class Injector
    {
        public static string GetKey(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0) return null;

            var process = processes[0];

            var app = new WindowsAppFriend(process);
            WindowsAppExpander.LoadAssembly(app, typeof(Injector).Assembly);
            dynamic injected = app.Type(typeof(Injector));

            try
            {
                return injected.GetKeyFromInjectedProcess();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetKeyFromInjectedProcess()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == "AI.Framework.App")
                {
                    var type = assembly.GetType("AI.Framework.AppFramework");
                    var property = type.GetProperty("Current");
                    dynamic current = property.GetValue(type);
                    return (string)current.AppSettings.LicenseKey;
                }
            }

            return null;
        }
    }
}
