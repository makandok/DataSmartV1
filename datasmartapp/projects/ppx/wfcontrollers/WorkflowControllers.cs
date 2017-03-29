using System;
using System.Collections.Generic;

namespace DataSmart.projects.ppx.wfcontrollers
{
    public class PP_PostRemovalControl : BaseWorkflowController
    {
        public PP_PostRemovalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexpostremoval1,typeof(PP_PostRemovalVisit1) },
            { Resource.Layout.prepexpostremoval2,typeof(PP_PostRemovalVisit2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_PostRemovalVisitEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexpostremoval1,
                Resource.Layout.prepexpostremoval2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_DeviceRemovalControl : BaseWorkflowController
    {
        public PP_DeviceRemovalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexdevremoval1,typeof(PP_DeviceRemoval1) },
            { Resource.Layout.prepexdevremoval2,typeof(PP_DeviceRemoval2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_DeviceRemovalEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexdevremoval1,
                Resource.Layout.prepexdevremoval2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_UnscheduledVisitControl : BaseWorkflowController
    {
        public PP_UnscheduledVisitControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexunscheduled1,typeof(PP_Unscheduled1) },
            { Resource.Layout.prepexunscheduled2,typeof(PP_Unscheduled2) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_UnscheduledEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexunscheduled1,
                Resource.Layout.prepexunscheduled2,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }

    public class PP_ClientEvalControl : BaseWorkflowController
    {
        public PP_ClientEvalControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.prepexreg1,typeof(PP_ClientEval1) },
            { Resource.Layout.prepexreg2,typeof(PP_ClientEval2) },
            { Resource.Layout.prepexreg3,typeof(PP_ClientEval3) },
            { Resource.Layout.prepexreg4,typeof(PP_ClientEval4) },
            { Resource.Layout.prepexreg5,typeof(PP_ClientEval5) },
            { Resource.Layout.PrepexDataEntryEnd,typeof(PP_ClientEvalEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.prepexreg1,
                Resource.Layout.prepexreg2,
                Resource.Layout.prepexreg3,
                Resource.Layout.prepexreg4,
                Resource.Layout.prepexreg5,
                Resource.Layout.PrepexDataEntryEnd
            };
        }
    }
}