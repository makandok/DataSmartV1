using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.projects.ppx
{
    public class PpxLookupProvider: ClientLookupProvider<PPClientSummary>
    {
        public PpxLookupProvider():base(Constants.KIND_DERIVED_PPX_CLIENTSUMMARY)
        {
        }
    }
}