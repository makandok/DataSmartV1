using System;
using System.Collections.Generic;

namespace DataSmart.projects
{
    public class BaseWorkflowController : IPP_NavController
    {
        public Dictionary<int, Type> MyActivities;

        public int[] MyLayouts;

        public System.Type GetNextActivity(int currentLayout, bool moveForward)
        {
            var targetLayout = GetNextLayout(currentLayout, moveForward);
            return GetActivityForLayout(targetLayout);
        }

        public System.Type GetActivityForLayout(int targetLayout)
        {
            return MyActivities[targetLayout];
        }

        public int GetNextLayout(int currentLayout, bool moveForward)
        {
            var targetIndex = -1;
            var currIndx = Array.IndexOf(MyLayouts, currentLayout);
            var max = MyLayouts.Length;
            if (currIndx == -1)
            {
                return -1;
            }
            var next = moveForward ? currIndx + 1 : currIndx - 1;
            targetIndex = moveForward ? (next >= max ? currentLayout : next) :
                targetIndex = next < 0 ? 0 : next;
            var targetLayout = MyLayouts[targetIndex];
            return targetLayout;
        }
    }
}