using ObjCRuntime;
using UIKit;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace XAMLDebuggingTechniques
{
    public class Program
    {
        /// <summary>
        /// iOS process entry point. Delegates to <see cref="AppDelegate"/> to
        /// bootstrap the cross-platform MAUI application.
        /// </summary>
        static void Main(string[] args)
        {
            // Wire the earliest possible culture and exception handlers prior to UIApplication.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Debug.WriteLine($"\n*** iOS EARLY UNHANDLED EXCEPTION ***\n{ex}\n*** END ***\n");
            };
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                e.SetObserved();
                Debug.WriteLine($"\n*** iOS EARLY UNOBSERVED TASK EXCEPTION (observed) ***\n{e.Exception}\n*** END ***\n");
            };
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
