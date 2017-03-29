using System;
using System.Collections.Generic;
using JhpDataSystem.model;
using JhpDataSystem;
using System.Linq;

namespace DataSmart.projects.vmc
{
    public class VmmcFormsBase : DataFormsBase<VmmcClientSummary>
    {
        protected override Type getHomeActivityType()
        {
            return typeof(VmmcHomeActivity);
        }

        protected override List<NameValuePair> getModuleClientSummaries(IEnumerable<NameValuePair> data)
        {
            return new List<NameValuePair>();
        }

        protected override List<NameValuePair> getIndexedFormData(List<NameValuePair> data)
        {
            var indexFieldNames = Constants.VMMC_IndexedFieldNames;
            return (data.Where(
                t => indexFieldNames.Contains(t.Name))).ToList();
        }

        protected override List<FieldItem> GetFieldsForView(int viewId)
        {
            return AppInstance.Instance.ModuleContext.ContextManager.FieldItems.Where(t => t.PageId == viewId).ToList();
        }
    }
}