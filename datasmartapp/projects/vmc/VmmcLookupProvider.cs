using JhpDataSystem.model;
using JhpDataSystem;
namespace DataSmart.projects.vmc
{
    public class VmmcLookupProvider: ClientLookupProvider<VmmcClientSummary>
    {
        public VmmcLookupProvider():base(Constants.KIND_DERIVED_VMMC_CLIENTSUMMARY)
        {
        }
    }
}