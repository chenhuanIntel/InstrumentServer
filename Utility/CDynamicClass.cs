using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Utility
{
    /// <summary>
    /// We will apply this dynamic attribute to our dynamic method.
    /// </summary>
    public class DateLastUpdated : Attribute
    {
        private string dateUpdated;
        /// <summary>
        /// 
        /// </summary>
        public string DateUpdated
        {
            get
            {
                return dateUpdated;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theDate"></param>
        public DateLastUpdated(string theDate)
        {
            this.dateUpdated = theDate;
        }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.propertybuilder?view=netframework-4.8
    /// https://www.c-sharpcorner.com/UploadFile/87b416/dynamically-create-a-class-at-runtime/
    /// </summary>
    public class DynamicClass
    {
        AssemblyName asemblyName;

        /// <summary>
        /// 
        /// </summary>
        public DynamicClass()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassName"></param>
        public DynamicClass(string ClassName)
        {
            this.asemblyName = new AssemblyName(ClassName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PropertyNames"></param>
        /// <param name="Types"></param>
        /// <param name="NestedListCustomAttributes"></param>
        /// <returns></returns>
        public object CreateObject(string[] PropertyNames, Type[] Types, List<CustomAttributeBuilder>[] NestedListCustomAttributes = null)
        {
            if (PropertyNames.Length != Types.Length)
            {
                Console.WriteLine("The number of property names should match their corresopnding types number");
            }

            TypeBuilder DynamicClass = this.CreateClass();
            this.CreateConstructor(DynamicClass);
            for (int ind = 0; ind < PropertyNames.Count(); ind++)
                CreateProperty(DynamicClass, PropertyNames[ind], Types[ind], NestedListCustomAttributes[ind]);
            Type type = DynamicClass.CreateType();

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeBuilder"></param>
        public void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <param name="ListCustomAttributes"></param>
        public void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, List<CustomAttributeBuilder> ListCustomAttributes = null)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Public);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);


            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);

            if (ListCustomAttributes != null)
            {
                foreach (CustomAttributeBuilder _customAttribute in ListCustomAttributes)
                {
                    propertyBuilder.SetCustomAttribute(_customAttribute);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputObj"></param>
        /// <returns></returns>
        public CustomAttributeBuilder CustomAttBuilder<T>(object inputObj)
        {
            Type obj = inputObj.GetType();
            Type[] ctorParams = new Type[] { obj };
            ConstructorInfo classCtorInfo = typeof(T).GetConstructor(ctorParams);
            CustomAttributeBuilder myCABuilder = new CustomAttributeBuilder(
                                classCtorInfo,
                                new object[] { inputObj });
            return myCABuilder;
        }
    }
}
