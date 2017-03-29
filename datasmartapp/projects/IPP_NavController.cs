using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DataSmart.projects
{
    public interface IPP_NavController
    {
        System.Type GetNextActivity(int currentLayout, bool moveForward);
        int GetNextLayout(int currentLayout, bool moveForward);
        System.Type GetActivityForLayout(int targetLayout);
    }
}