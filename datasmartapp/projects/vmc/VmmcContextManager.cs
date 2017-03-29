
using Android.App;
using Android.Content.Res;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.projects
{
    public class VmmcContextManager: BaseContextManager        
    {
        public VmmcContextManager(AssetManager assetManager, Activity mainContext)
            : base(Constants.FILE_VMMC_FIELDS, assetManager, mainContext)
        {
            ProjectCtxt = ProjectContext.Vmmc;
            KindDisplayNames = Constants.VMMC_KIND_DISPLAYNAMES;
            FIELD_VISITDATE = Constants.FIELD_VMMC_DATEOFVISIT;
        }
    }
}