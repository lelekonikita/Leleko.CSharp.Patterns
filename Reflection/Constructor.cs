using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using Leleko.CSharp.Patterns.Creation;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns.Reflection
{
	public abstract class Constructor: Singleton
	{
		public abstract Delegate CtorDelegate { get; }

		private class EqualityTypeArrayComparer: IEqualityComparer<Type[]>
		{
			public bool Equals(Type[] x, Type[] y)
			{
				throw new NotImplementedException();
			}

			public int GetHashCode(Type[] obj)
			{
				throw new NotImplementedException();
			}
		}
	}

	/// <summary>
	/// Конструктор типа и параметров определяемых делегатом
	/// <example>Constructor[Func[double,double,Point]]</example>
	/// </summary>
	public sealed class Constructor<TDelegate>: Constructor
		where TDelegate: class
	{
		#region implemented abstract members of Constructor

		public override Delegate CtorDelegate
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		/// <summary>
		/// The constructor
		/// </summary>
		public static readonly TDelegate Ctor;

		static Constructor()
		{
			if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
				throw new InvalidCastException("TDelegate must be subclass of Delgate");
			var methodInfo = typeof(TDelegate).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public );

			var parameters = methodInfo.GetParameters();
			var parametersTypes = parameters.Length > 0 ? new Type[parameters.Length] : Type.EmptyTypes;
			for (int i = 0; i < parameters.Length; i++)
				parametersTypes[i] = parameters[i].ParameterType;

			var constructorInfo = methodInfo.ReturnType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parametersTypes, null);
			if (constructorInfo == null)
				throw new NotSupportedException("constructor not found");

			DynamicMethod dynamicMethod = new DynamicMethod(string.Concat(methodInfo.ReturnType.FullName, methodInfo.Name), methodInfo.ReturnType, parametersTypes, true);
			var il = dynamicMethod.GetILGenerator();
			for (int i = 0; i < parameters.Length; i++)
				il.Emit(OpCodes.Ldarg_S, i);
			il.Emit(OpCodes.Newobj, constructorInfo);
			il.Emit(OpCodes.Ret);

			Ctor = dynamicMethod.CreateDelegate(typeof(TDelegate)) as TDelegate;
		}
	}
}

