using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class CCompareUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool wildCardMatch(string s, string p)
        {
            int m = s.Length, n = p.Length;
            int i = 0, j = 0, asterisk = -1, match = 0;
            while (i < m)
            {
                if (j < n && p[j] == '*')
                {
                    match = i;
                    asterisk = j++;
                }
                else if (j < n && (s[i] == p[j] || p[j] == '?'))
                {
                    i++;
                    j++;
                }
                else if (asterisk >= 0)
                {
                    i = ++match;
                    j = asterisk + 1;
                }
                else
                    return false;
            }
            while (j < n && p[j] == '*')
                j++;
            return j == n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objArray1"></param>
        /// <param name="objArray2"></param>
        /// <returns></returns>
        public static bool CompareIEnumerable(IEnumerable objArray1, IEnumerable objArray2)
        {
            ArrayList al1 = new ArrayList();
            foreach (Object obj in objArray1)
            {
                al1.Add(obj);
            }

            ArrayList al2 = new ArrayList();
            foreach (Object obj in objArray2)
            {
                al2.Add(obj);
            }

            //Make sure the arrays are the same size
            if (al1.Count != al2.Count)
                return false;

            for (int n = 0; n < al1.Count; n++)
            {
                if (!CompareObject(al1[n], al2[n]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsObjectIEnumerable(Object obj)
        {
            try
            {
                IEnumerable ie = (IEnumerable)obj;//attempt cast
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool CompareClassObject(Object obj1, Object obj2)
        {
            Object fldObj1, fldObj2;

            Type objType = obj1.GetType();

            //Parse and map all Fields of both objects
            int n = 0;
            FieldInfo[] objFIar = objType.GetFields();
            for (n = 0; n < objFIar.Length; n++)
            {
                fldObj1 = objFIar[n].GetValue(obj1);
                Debug.WriteLine("Obj1.Field." + objFIar[n].Name + " = " + fldObj1);

                fldObj2 = objFIar[n].GetValue(obj1);
                Debug.WriteLine("Obj2.Field." + objFIar[n].Name + " = " + fldObj2);

                if (!CompareObject(fldObj1, fldObj2)) return false;
            }

            //Parse and map all Properties of both objects
            PropertyInfo[] objPIar = objType.GetProperties();
            Object propObj1, propObj2;
            for (n = 0; n < objPIar.Length; n++)
            {
                propObj1 = objPIar[n].GetValue(obj1, null);
                if (null != propObj1)
                {
                    Debug.WriteLine("Obj1.Prop." + objPIar[n].Name + " = " + propObj1);
                }
                else
                {
                    Debug.WriteLine("Obj1.Prop." + objPIar[n].Name + " = null");
                }

                propObj2 = objPIar[n].GetValue(obj2, null);

                if (null != propObj2)
                {
                    Debug.WriteLine("Obj2.Prop." + objPIar[n].Name + " = " + propObj2);
                }
                else
                {
                    Debug.WriteLine("Obj2.Prop." + objPIar[n].Name + " = null");
                }

                if (!CompareObject(propObj1, propObj2)) return false;
            }

            return true;
        }

        /// <summary>
        /// Static method to compare 2 objects of identical types.  It will recursively walk through and do the comparison
        /// </summary>
        /// <param name="obj1">1st object to compare</param>
        /// <param name="obj2">2nd object to copare</param>
        /// <returns></returns>
        public static bool CompareObject(Object obj1, Object obj2)
        {
            if ((null == obj1) && (null == obj2)) return true;

            //Assert that the types are the same and from the same assembly
            Type obj1Type = obj1.GetType();
            Type obj2Type = obj2.GetType();

            if (!obj1Type.AssemblyQualifiedName.Equals(obj2Type.AssemblyQualifiedName))
                return false;

            if (obj1Type.IsPrimitive)
            {
                if (!obj1.Equals(obj2))
                    return false;
            }

            if (obj1Type.IsClass)
            {
                if (obj1Type == typeof(string))
                {
                    if (!obj1.Equals(obj2))
                        return false;
                }
                else
                {
                    //Are the objects IEnumerable
                    if (IsObjectIEnumerable(obj1))
                    {
                        if (!CompareIEnumerable((IEnumerable)obj1, (IEnumerable)obj2))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!CompareClassObject(obj1, obj2))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}

