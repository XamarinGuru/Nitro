using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using System.Collections.Generic;
using System.Linq;
using Android.Preferences;
using Android.Content.PM;
using Android.Webkit;
using Android.Net;
using goheja.Services;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace goheja
{

    //by Afroz date 24/07/2016
    [Activity(Label = "Closing Application")]
    public class CloseApplication : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.CancelAll();
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            Finish();
            // Create your application here
        }
    }

    //update end by Afroz
}