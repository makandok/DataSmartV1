
using Android.App;
using Android.Content.Res;

namespace JhpDataSystem.projects
{
    public class VmmcContextManager: BaseContextManager        
    {
        public VmmcContextManager(AssetManager assetManager, Activity mainContext)
            : base(Constants.FILE_VMMC_FIELDS, assetManager, mainContext)
        {
            Name = "vmc";
            ProjectCtxt = ProjectContext.Vmmc;
            KindDisplayNames = Constants.VMMC_KIND_DISPLAYNAMES;
            FIELD_VISITDATE = Constants.FIELD_VMMC_DATEOFVISIT;
        }
    }
}