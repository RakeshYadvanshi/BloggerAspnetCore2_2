using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BloggerAPI.Tests
{
    public class Helper
    {
        public static bool CompareProperties<T>(T obj1
            , T obj2)
        {

            var propertiesObj1 = obj1.GetType().GetProperties();
            var propertiesObj2 = obj2.GetType().GetProperties();
            for (var i = 0; i < propertiesObj1.Length; i++)
            {

                if (propertiesObj1[i].GetValue(obj1) != propertiesObj2[i].GetValue(obj2))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
