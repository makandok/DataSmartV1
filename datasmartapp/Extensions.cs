using JhpDataSystem;
using JhpDataSystem.model;
using System;

namespace DataSmart
{
    internal static class Extensions
    {
        //internal static long toSafeDate(this DateTime dateValue)
        //{            
        //    return (dateValue.Year * 100 + dateValue.Month) * 100 + dateValue.Day;
        //}

        //internal static DateTime fromSafeDate(this int safeDate)
        //{
        //    var asString = safeDate.ToString();
        //    var day = Convert.ToInt16(asString.Substring(5, 2));
        //    var month = Convert.ToInt16(asString.Substring(3, 2));
        //    var year = Convert.ToInt16(asString.Substring(0, 4));
        //    return new DateTime(year, month, day);
        //}

        //internal static T GetDataView<T>(this FieldItem field, Android.App.Activity context) where T : Android.Views.View
        //{
        //    //we convert these into int Ids
        //    var fieldName = 
        //        (field.dataType == Constants.DATEPICKER || field.dataType == Constants.TIMEPICKER)
        //        ? 
        //        Constants.DATE_TEXT_PREFIX + field.name :
        //        field.name;

        //    int resourceId = context.Resources.GetIdentifier(
        //        fieldName, "id", context.PackageName);
        //    T view = null;
        //    view = context.FindViewById<T>(resourceId);
        //    return view;
        //}

        internal static string toText(this System.IO.Stream stream)
        {
            //var buffer = new byte[length];
            //prepexFieldsStream.Read(buffer, 0, Convert.ToInt32(length));
            //var asString = Convert.ToString(buffer);

            var mstream = new System.IO.MemoryStream();
            stream.CopyTo(mstream);
            var bytes = mstream.ToArray();
            return System.Text.Encoding.Default.GetString(bytes);
        }

        internal static byte[] toByteArray(this System.IO.Stream stream)
        {
            var mstream = new System.IO.MemoryStream();
            stream.CopyTo(mstream);
            var bytes = mstream.ToArray();
            return bytes;
        }

        internal static string decryptAndGetApiSetting(this System.Json.JsonValue jsonObject, string settingString)
        {
            var value = jsonObject[settingString];
            if (Constants.ENCRYPTED_ASSETS.Contains(settingString)){
                //we decrypt
                return JhpSecurity.Decrypt(value);
            }
            return value;
        }
    }
}