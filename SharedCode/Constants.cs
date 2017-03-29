using JhpDataSystem.model;
using System.Collections.Generic;

namespace JhpDataSystem
{
    public class Constants
    {
        public const string EncryptionSALT = "Innovating to Save Lives. Preventing the needless deaths of women";

        //public const string BUNDLE_ENTITYID = "bundl_entityid";
        public const string BUNDLE_DATATOEDIT = "bundl_datatoedit";
        //public const string BUNDLE_RECEDITSTAGE = "bundl_editstage";
        public const string BUNDLE_SELECTEDCLIENT = "bundl_client";
        public const string BUNDLE_SELECTEDRECORD_ID = "bundl_recordid";
        public const string BUNDLE_SELECTEDRECORD = "bundl_recordsummary";
        public const string BUNDLE_NEXTACTIVITY_TYPE = "bundl_nxtactvty";

        public const string FIELD_ENTITYID = "entityid";
        public const string FIELD_ID = "id";

        public const string SYS_FIELD_USERNAME = "username";
        public const string SYS_FIELD_USERID = "sys_userid";
        public const string SYS_FIELD_USERNAMES = "sys_usernames";
        public const string SYS_FIELD_PASSWDHASH = "sys_pwdhash";


        public const string SYS_FIELD_DATEEDITED = "sys_editdate";

        public const string SYS_FIELD_DATECREATED = "sys_datecreated";
        public const string SYS_FIELD_METADATA = "sys_metadata";

        public const string SYS_FIELD_FirstName = "sys_fname";
        public const string SYS_FIELD_SurName = "sys_surname";
        public const string SYS_FIELD_MaidenName = "sys_maidenname";
        public const string SYS_FIELD_NRC = "sys_nrc";
        public const string SYS_FIELD_ContactNumber = "sys_cellnumb";
        public const string SYS_FIELD_HomeFacilityName = "sys_facname";
        public const string SYS_FIELD_FACILITYID = "sys_facilityid";
        public const string SYS_FIELD_SESSIONDATE = "sys_sessiondate";
        public const string SYS_FIELD_TODAY = "today";
        

        //VMMC
        public const string FIELD_VMMC_DOB = "vm_date_of_birth";
        public const string FIELD_VMMC_MCDATE = "vm_mc_date";
        public const string FIELD_VMMC_DATEOFVISIT = "vm_registration_date";
        public const string FIELD_VMMC_CARD_SERIAL = "vm_intakeform_serial";
        public const string FIELD_VMMC_MCNUMBER = "vm_mc_number";

        public const string FIELD_VMMC_CLIENTLASTNAME = "vm_last_name";
        public const string FIELD_VMMC_CLIENTFIRSTNAME = "vm_first_name";

        public const string FIELD_VMMC_CLIENTTEL = "vm_phone_number";
        //public const string FIELD_VMMC_CLIENTPHYSICALADDR = "clientsphysicaladdress";

        public static List<string> VMMC_IndexedFieldNames = new List<string>() {
                FIELD_VMMC_CLIENTLASTNAME,FIELD_VMMC_CLIENTFIRSTNAME,
                FIELD_VMMC_MCDATE,FIELD_VMMC_DOB,
                FIELD_ID,FIELD_ENTITYID,FIELD_VMMC_DATEOFVISIT,
                FIELD_VMMC_CARD_SERIAL,FIELD_VMMC_MCNUMBER,FIELD_VMMC_CLIENTTEL,
                //FIELD_VMMC_CLIENTPHYSICALADDR
        };


        //tables in the database

        public const string FIELD_PPX_DEVSIZE = "ppxdevsize";
        public const string FIELD_PPX_DEVSIZE_PREFIX = "prepexdevicesize";

        public const string FIELD_PPX_DATEOFVISIT = "dateofvisit";
        public const string FIELD_PPX_CARD_SERIAL = "cardserialnumber";
        public const string FIELD_PPX_CLIENTIDNUMBER = "clientidnumber";
        public const string FIELD_PPX_CLIENTNAME = "clientname";
        public const string FIELD_PPX_DOB = "dob";
        public const string FIELD_PPX_CLIENTTEL = "clienttel";
        public const string FIELD_PPX_CLIENTPHYSICALADDR = "clientsphysicaladdress";
        public const string FIELD_PPX_PLACEMENTDATE = "dateofplacement";
        public static List<string> PP_IndexedFieldNames = new List<string>() {
            //FIELD_PLACEMENTDATE,
                FIELD_PPX_DEVSIZE,
                FIELD_ID,FIELD_ENTITYID,FIELD_PPX_DATEOFVISIT,
                FIELD_PPX_CARD_SERIAL,FIELD_PPX_CLIENTIDNUMBER,FIELD_PPX_CLIENTNAME,FIELD_PPX_DOB,FIELD_PPX_CLIENTTEL,
                FIELD_PPX_CLIENTPHYSICALADDR};

        public const string LABEL_PPX_ACTIVITYLABEL = "Prepex Manager";
        
        public const string SYS_KIND_DEVCONF = "sysdevconf";
        public const string KIND_DERIVED_RECORDSUMMARY = "recordsummary";

        //public const string KIND_PPX = "pp_client";

        public const string KIND_PPX_CLIENTEVAL = "pp_client_eval";
        public const string KIND_PPX_DEVICEREMOVAL = "pp_client_devicerem";
        public const string KIND_PPX_POSTREMOVAL = "pp_client_postrem";
        public const string KIND_PPX_UNSCHEDULEDVISIT = "pp_client_unsched";

        public static Dictionary<string, string> PPX_KIND_DISPLAYNAMES =
            new Dictionary<string, string>() {
                { KIND_PPX_CLIENTEVAL,"A1. Client Evaluation" },
                 { KIND_PPX_DEVICEREMOVAL,"A3. Device Removal Visit" },
                  { KIND_PPX_POSTREMOVAL,"A4. Post Removal" },
                   { KIND_PPX_UNSCHEDULEDVISIT,"A2. Unscheduled Visit" }
            };

        public const string KIND_DERIVED_PPX_CLIENTSUMMARY = "pp_clientsummary";         

        public const string KIND_DERIVED_VMMC_CLIENTSUMMARY = "vmmc_clientsummary";
        public const string KIND_VMMC_POSTOP = "vmmc_postop";
        public const string KIND_VMMC_REGANDPROCEDURE = "vmmc_regandproc";
        public static Dictionary<string, string> VMMC_KIND_DISPLAYNAMES =
                new Dictionary<string, string>() {
                    { KIND_VMMC_POSTOP,"Post Operation" },
                     { KIND_VMMC_REGANDPROCEDURE,"Registration and Procedure" }
                };

        public const string KIND_SITESESSION = "sitesession";
        public const string KIND_SITEPROVIDER = "siteprovider";

        public const string KIND_APPUSERS = "appusers";
        public const string KIND_OUTTRANSPORT = "transport";
        public const string KIND_FAILEDOUTTRANSPORT = "badtransport";

        public const string KIND_DEFAULT = "generalstore";
        public const string KIND_REGISTER = "kindRegister";
        //encryptionkey
        public const string ASSET_NAME_APPNAME = "applicationname";
        public const string ASSET_PROJECT_ID = "projectid";
        public const string ASSET_NAME_SVC_ACCTEMAIL = "serviceaccountemail";
        public const string ASSET_API_KEYFILE = "api_keys.json";
        public const string ASSET_DATASTORE_APPKEY = "datastore_appkey";
        public const string ASSET_P12KEYFILE = "p12keyfile";
        public const string ASSET_ADMIN_HASH = "adminhash";
        public const string ASSET_ADMIN_ENCRYPTIONKEY = "encryptionkey";

        public static readonly List<string> ASSET_LIST = new List<string>(){
            ASSET_NAME_APPNAME , ASSET_PROJECT_ID, ASSET_NAME_SVC_ACCTEMAIL,
            ASSET_DATASTORE_APPKEY,
            ASSET_P12KEYFILE,ASSET_ADMIN_HASH,ASSET_ADMIN_ENCRYPTIONKEY
        };
        //
        public const string API_KEYFILE = "api_keys.json";
        public static System.Collections.Generic.List<string> 
            ENCRYPTED_ASSETS = new System.Collections.Generic.List<string>() { ASSET_DATASTORE_APPKEY };
        public const string DBSAVE_ERROR = "default error value";

        public const string MOTHER_OFBOLG = "Text to Encrypt. Do NOT CHANGE";
        public const string MOTHER_OFALLBOLGS = "ADMIN Text to Encrypt. Do NOT CHANGE";

        public const string ADMIN_USERNAME = "admin";

        public const string SUPPORTADMIN_USERNAME = "support";

        //prepexreg_fields
        public const string FILE_PPX_FIELDS = "ppx_fields.json";
        public const string FILE_VMMC_FIELDS = "vmmc_fields.json";

        public const string PP_VIEWS_1= "prepexreg1";
        public const string PP_VIEWS_2 = "prepexreg2";
        public const string PP_VIEWS_3 = "prepexreg3";
        public const string PP_VIEWS_4 = "prepexreg4";

        public const string TIMEPICKER = "TimePicker";
        public const string DATEPICKER = "DatePicker";
        public const string EDITTEXT = "EditText";
        public const string CHECKBOX = "CheckBox";
        public const string RADIOBUTTON = "RadioButton";

        public const string DATE_BUTTON_PREFIX = "dtbtn_";
        public const string DATE_TEXT_PREFIX = "dttxt_";
        public const string LABEL_PREFIX = "sylbl_";

        public const string DEFAULT_CHECKED = "1";
    }
}