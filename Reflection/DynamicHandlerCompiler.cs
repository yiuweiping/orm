using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Zhengdi.Framework.Data.Entity;

namespace Zhengdi.Framework.Reflection
{
    public class DynamicHandlerCompiler<T>
    {
        readonly Type _type;
   
         
        public DynamicHandlerCompiler( )
        {
            this._type = typeof(T);
        }
        public DynamicHandlerCompiler(Type targetType)
        {
            this._type = targetType;
        }
        public DynamicHandlerCompiler(object target )
        {

            this._type = (target ?? new object()).GetType();
        }
        public Action<T, K,Type> CreaterSetPropertyHandler<K>(string propertyName, Type paramType = null)
        {
            var temp = typeof(Type);
            paramType = paramType ?? typeof(K);
            string methodName = $"set_{propertyName}";
            var callMethod = _type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
            var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new Type[] { paramType, temp });

            var para = callMethod.GetParameters()[0];
            var targetTyppe = this._type.BaseType == typeof(DataEntity) ? typeof(DataEntity) : this._type;
           
            DynamicMethod method = new DynamicMethod("EmitCallable", null, new Type[] { targetTyppe, paramType, temp }, this._type.Module);

            var il = method.GetILGenerator();
            var NotNullLable = il.DefineLabel();
            var local = il.DeclareLocal(typeof(bool));
            var local2 = il.DeclareLocal(para.ParameterType);
            var local3 = il.DeclareLocal(typeof(Type));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Cgt_Un);
            il.Emit(OpCodes.Stloc, local);
            il.Emit(OpCodes.Ldloc, local);
            il.Emit(OpCodes.Brfalse_S, NotNullLable);
       
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, changeTypeMethod);
            if (para.ParameterType.IsValueType)
            {
                if (para.ParameterType.IsEnum)
                {
                    local2 = il.DeclareLocal(typeof(Int32));
                    il.Emit(OpCodes.Unbox_Any, typeof(Int32));
                }
                else
                    il.Emit(OpCodes.Unbox_Any, para.ParameterType);//  string = (string)object;
            }
            else
            {
                il.Emit(OpCodes.Castclass, para.ParameterType);//   Class = object as Class
            }
            il.Emit(OpCodes.Stloc, local2);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc, local2);
            il.EmitCall(OpCodes.Callvirt, callMethod, null);//调用函数
            il.MarkLabel(NotNullLable);
            il.Emit(OpCodes.Ret);
 
            return method.CreateDelegate(typeof(Action<T, K,Type>)) as Action<T, K, Type>;
        }

        public Func<T, K> CreaterGetPropertyHandler<K>(string propertyName,bool isEnum=false)
        {

            string methodName = $"get_{propertyName}";
            var callMethod = _type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic);
            var paramType = callMethod.ReturnType;
            var targetTyppe = this._type.BaseType == typeof(DataEntity) ? typeof(DataEntity) : this._type;
            DynamicMethod method = new DynamicMethod("EmitCallable2", typeof(K), new Type[] { typeof(T) }, this._type.Module);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, callMethod, null);//调用函数
            if (paramType.IsValueType)
            {
                if (paramType.IsEnum && isEnum)
                    il.Emit(OpCodes.Box, typeof(Int32));
                else
                    il.Emit(OpCodes.Box, paramType);
            }
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<T, K>)) as Func<T, K>;
        }

        public Func<T> CreaterInstance(Type type)
        {
          
            DynamicMethod method = new DynamicMethod("GetInstance", this._type,null, this._type.Module);

            var il = method.GetILGenerator();
            var local = il.DeclareLocal(this._type);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<T>)) as Func<T>;
        }
      
        public Func<K, T> CreaterInstance<K>(Type type, Type ParameterType)
        {
            var paramerTypes = new Type[] { ParameterType };
            DynamicMethod method = new DynamicMethod("GetInstance", this._type, new Type[] { typeof(K) }, this._type.Module);
            var c = type.GetConstructor(paramerTypes);
            var il = method.GetILGenerator();
            var local = il.DeclareLocal(this._type);
            il.Emit(OpCodes.Ldarg_0);
            if (ParameterType.IsValueType)
                il.Emit(OpCodes.Unbox_Any, ParameterType);//  string = (string)object;
            else
                il.Emit(OpCodes.Castclass, ParameterType);//   Class = object as Class
            il.Emit(OpCodes.Newobj, c);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<K, T>)) as Func<K, T>;
        }
        public Type CreaterAnonEntity(string clssName, IEnumerable<CreaterDynamicClassProperty> Propertys)
        {
            var assemblyName = new AssemblyName("AnonEntity");
            // create assembly builder
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run  );
            // create module builder
            // var moduleBuilder = assemblyBuilder.DefineDynamicModule("AnonEntityModule", "Framework.dll");
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            // create type builder for a class
            var typeBuilder = moduleBuilder.DefineType(clssName, TypeAttributes.Public);

            var fields = (from t in Propertys select typeBuilder.DefineField(t.Name.ToLower(), t.Type, FieldAttributes.Private)).ToArray(); ;
 
            ConstructorInfo objCtor = typeof(object).GetConstructor(new Type[0]);

            var constructorArgs = from t in Propertys select t.Type;

            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, constructorArgs.ToArray());
            ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Call, objCtor);
            for (var i = 1; i < fields.Count() + 1; i++)
            {
                ilOfCtor.Emit(OpCodes.Ldarg_0);
                ilOfCtor.Emit(OpCodes.Ldarg_S, i);
                ilOfCtor.Emit(OpCodes.Stfld, fields[i - 1]);
            }
            ilOfCtor.Emit(OpCodes.Ret);

            for (var i = 0; i < fields.Count(); i++)
            {
                var methodGet = typeBuilder.DefineMethod($"get_{Propertys.ElementAt(i).Name}", MethodAttributes.Public, Propertys.ElementAt(i).Type, null);
                var ilOfGet  = methodGet.GetILGenerator();
                ilOfGet.Emit(OpCodes.Ldarg_0); // this
                ilOfGet.Emit(OpCodes.Ldfld, fields[i]);
                ilOfGet.Emit(OpCodes.Ret);
                var propertyId = typeBuilder.DefineProperty(Propertys.ElementAt(i).Name, PropertyAttributes.None, Propertys.ElementAt(i).Type, null);
                propertyId.SetGetMethod(methodGet);
            }
            var classType = typeBuilder.CreateType();
            return classType;
 
        }
         
    }
    public class CreaterDynamicClassProperty : IField
    {
        public string Name { get; set; }

        public Type Type { get; internal set; }

        public dynamic Value { get; internal set; }
        public CreaterDynamicClassProperty(string name, dynamic value)
        {
            this.Name = name;
            this.Type = value.GetType();
            this.Value = value; 
        }
        public static object[] GetValues(IEnumerable<CreaterDynamicClassProperty> propertys)
        {
            return (from t in propertys select t.Value as object).ToArray();
        }
    }
    public enum DynamicHandlerType:uint
    {
         Set=0,
         Get=1,
    }
}
