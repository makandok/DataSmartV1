using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
using JhpDataSystem.model;
using JhpDataSystem;
using System.Linq;
using DataSmart.store;
using Android.Content;

namespace DataSmart.projects.ppx
{
    public class PPXFormsBase : DataFormsBase<PPClientSummary>
    {
        protected override Type getHomeActivityType()
        {
            return typeof(PPXHomeActivity);
        }

        protected override List<NameValuePair> getModuleClientSummaries(IEnumerable<NameValuePair> data)
        {
            //we get the device size
            var toReturn = new List<NameValuePair>();
            var deviceSizeControl = data.Where(t => t.Name.Contains(Constants.FIELD_PPX_DEVSIZE_PREFIX)).FirstOrDefault();
            if (deviceSizeControl != null)
            {
                var deviceSize = deviceSizeControl.Name.Last().ToString().ToUpperInvariant();
                toReturn.Add(new NameValuePair() { Name = Constants.FIELD_PPX_DEVSIZE, Value = deviceSize });
            }
            return toReturn;
        }

        protected override List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
        {
            var indexFieldNames = Constants.PP_IndexedFieldNames;
            return (data.Where(
                t => indexFieldNames.Contains(t.Name))).ToList();
        }

        protected override List<FieldItem> GetFieldsForView(int viewId)
        {
            return AppInstance.Instance.ModuleContext.ContextManager.FieldItems.Where(t => t.PageId == viewId).ToList();
        }
    }
}