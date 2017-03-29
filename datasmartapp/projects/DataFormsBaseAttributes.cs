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
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.projects
{
    public class DataFormsBaseAttributes : Activity
    {
        protected bool _isRegistration = false;
        protected int myView = -1;
        protected IPP_NavController myNavController = null;
        protected bool IsFirstPage = false;
        public KindName _kindName { get; set; }
        //public bool AllowShowView = true;
        public DataFormsBaseAttributes InitialiseAttributes()
        {
            doPreCreate(null);
            return this;
        }

        protected virtual void doPreCreate(Bundle savedInstanceState)
        {
            _kindName = null; // new KindName(Constants.KIND_PPX_UNSCHEDULEDVISIT);
            myView = -1; // Resource.Layout.prepexunscheduled2;
            myNavController = null; // new PP_UnscheduledVisitControl() { };
        }
    }
}