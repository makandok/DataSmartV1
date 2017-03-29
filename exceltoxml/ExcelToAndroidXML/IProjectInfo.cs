using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToAndroidXML
{
    public abstract class ProjectInfo
    {
        public abstract Dictionary<string, string> moduleNamePrefixes { get;  }
        public abstract string LookupChoicesFile { get; }
        public abstract string FieldDictionaryFile { get; }
        public abstract string Prefix { get; }
        //Prefix
    }

    public class vmmcHelper : ProjectInfo
    {
        Dictionary<string, string> _moduleNamePrefixes = new Dictionary<string, string>()
            {
                {"Registration and MC Procedure","vmmc_regandproc"},
                {"Post Operation","vmmc_postop"}
            };
        string _LookupChoicesFile = "vmmc/LookupChoices.json";
        string _FieldDictionaryFile = "vmmc/FieldDictionary.json";

        public override Dictionary<string, string> moduleNamePrefixes
        {
            get
            {
                return _moduleNamePrefixes;
            }
        }
        public override string LookupChoicesFile
        {
            get
            {
                return _LookupChoicesFile;
            }
        }
        public override string FieldDictionaryFile
        {
            get
            {
                return _FieldDictionaryFile;
            }
        }
        public override string Prefix
        {
            get
            {
                return "vmmc";
            }
        }
    }

    public class ppxHelper : ProjectInfo
    {
        Dictionary<string, string> _moduleNamePrefixes = new Dictionary<string, string>()
            {
                {"A1 Client Evaluation and Registration","prepexreg"},
                {"A3 Device Removal Visit or Follow Up","prepexdevremoval"},
                {"A4 Post Removal Visit Assessment","prepexpostremoval"},
                {"A2 Unscheduled or Follow-up Prepex Form","prepexunscheduled"}
            };
        string _LookupChoicesFile = "ppx/LookupChoices.json";
        string _FieldDictionaryFile = "ppx/FieldDictionary.json";

        public override Dictionary<string, string> moduleNamePrefixes
        {
            get
            {
                return _moduleNamePrefixes;
            }
        }
        public override string LookupChoicesFile
        {
            get
            {
                return _LookupChoicesFile;
            }
        }
        public override string FieldDictionaryFile
        {
            get
            {
                return _FieldDictionaryFile;
            }
        }
        public override string Prefix
        {
            get
            {
                return "ppx";
            }
        }
    }
}
