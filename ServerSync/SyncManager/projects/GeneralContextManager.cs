using Android.App;
using Android.Content.Res;

namespace JhpDataSystem.projects
{
    public class GeneralContextManager: BaseContextManager        
    {
        public GeneralContextManager(AssetManager assetManager, Activity mainContext)
            : base(Constants.FILE_VMMC_FIELDS, assetManager, mainContext)
        {
            ProjectCtxt = ProjectContext.None;
            KindDisplayNames = Constants.VMMC_KIND_DISPLAYNAMES;
            FIELD_VISITDATE = Constants.FIELD_VMMC_DATEOFVISIT;
        }
    }
}