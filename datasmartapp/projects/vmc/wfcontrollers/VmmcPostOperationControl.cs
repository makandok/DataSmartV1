using System;
using System.Collections.Generic;

namespace DataSmart.projects.vmc.workflow
{
    public class VmmcPostOperationControl : BaseWorkflowController
    {
        public VmmcPostOperationControl()
        {
            MyActivities = new Dictionary<int, Type>() {
            { Resource.Layout.vmmc_postop1,typeof(VmmcPostOp1) },
            { Resource.Layout.vmmc_postop2,typeof(VmmcPostOp2) },
            { Resource.Layout.DataEntryEnd,typeof(VmmcPostOpEnd) }
            };

            MyLayouts = new[] {
                Resource.Layout.vmmc_postop1,
                Resource.Layout.vmmc_postop2,
                Resource.Layout.DataEntryEnd
            };
        }
    }
}