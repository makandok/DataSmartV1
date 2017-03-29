using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToAndroidXML
{
    public class FieldDefinition
    {
        public string FormName { get; set; }
        public string GroupName { get; set; }
        public string ViewName { get; set; }
        public string ViewType { get; set; }
        public string DisplayLabel { get; set; }
        public string ListName { get; set; }
        public int GridColumn { get; set; }
        public int Id { get; set; }
        public string IsIndexed { get; set; }
        public string IsRequired { get; set; }
        public string ViewPage { get; set; }
        public string IsDeprecated { get; set; }
        public FieldChoices FieldOptions { get; set; }
    }
}
