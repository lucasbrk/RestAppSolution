using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace RestApp.Common.Utility
{
    public class StringUtility
    {
        public static string ShortenText(object sIn, int length)
        {
            string sOut = sIn.ToString();

            if (sOut.Length > length)
            {
                sOut = sOut.Substring(0, length) + " ...";
            }

            return sOut;
        }

        public static object StringToEnum(Type t, string Value)
        {
            object oOut = null;

            foreach (System.Reflection.FieldInfo fi in t.GetFields())
            {
                if (fi.Name.ToLower() == Value.ToLower())
                {
                    oOut = fi.GetValue(null);
                }
            }

            return oOut;
        }

        /// <summary>
        /// Replace substring in original string with the value from NameValue collection of the same key
        /// </summary>
        /// <param name="Original">Original string need to replace</param>
        /// <param name="VariableCollection">key si the vaariable name and value is the replaced value</param>
        /// <returns></returns>
        public static string SetVariablesWithKeyValue(string Original, NameValueCollection VariableCollection)
        {
            string[] keys = VariableCollection.AllKeys;

            foreach (string s in keys)
            {
                Original = Original.Replace(s, VariableCollection[s]);
            }

            return Original;
        }

        public static int GetInt(String s)
        {
            if (!String.IsNullOrEmpty(s))
                return Convert.ToInt32(s);
            else
                return 0;
        }


    }
}
