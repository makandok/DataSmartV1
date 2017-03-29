using Android.App;
using Android.Content.Res;
using System.Collections.Generic;

namespace JhpDataSystem.projects
{
    public class PpxContextManager : BaseContextManager
    {
        public PpxContextManager(AssetManager assetManager, Activity mainContext)
            : base(Constants.FILE_PPX_FIELDS, assetManager, mainContext)
        {
            Name = "ppx";
            ProjectCtxt = ProjectContext.Ppx;
            KindDisplayNames = Constants.PPX_KIND_DISPLAYNAMES;
            FIELD_VISITDATE = Constants.FIELD_PPX_DATEOFVISIT;
            //FieldTablenameToKind = new Dictionary<string, string>() {
            //    {"prepexreg",Constants.KIND_PPX_CLIENTEVAL },
            //    { "prepexdevremoval",Constants.KIND_PPX_DEVICEREMOVAL },
            //    { "prepexpostremoval",Constants.KIND_PPX_POSTREMOVAL },
            //    { "prepexunscheduled",Constants.KIND_PPX_UNSCHEDULEDVISIT },
            //};
            KindToFieldTablename = new Dictionary<string, string>() {
                {Constants.KIND_PPX_CLIENTEVAL,"prepexreg" },
                { Constants.KIND_PPX_DEVICEREMOVAL ,"prepexdevremoval"},
                {Constants.KIND_PPX_POSTREMOVAL, "prepexpostremoval" },
                { Constants.KIND_PPX_UNSCHEDULEDVISIT, "prepexunscheduled"},
            };
        }
    }
}