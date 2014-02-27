using System;
using System.Reflection;
using Leleko.CSharp.Patterns.Creation;
using System.Collections;

namespace Leleko.CSharp.Patterns.Action
{
	/*
	public abstract class Clonner: Singleton
	{
		protected static readonly Converter<object, object> MemberwiseClone = 
			Delegate.CreateDelegate (
				typeof(Converter<object,object>),
				typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)
			) as Converter<object,object>;

		protected static readonly Converter<object, object> CloneableClone = 
			Delegate.CreateDelegate (
				typeof(Converter<object,object>),
				typeof(ICloneable).GetMethod("Clone")
			) as Converter<object, object>;
		
		protected static readonly Hashtable MethodTable = new Hashtable();

		protected Clonner() : base() { }

		protected virtual Delegate CreateCloneDelegate(Type type)
		{
			throw new NotImplementedException();
			if (type.IsClass)
			{

			}
			else if (type.IsInterface)
			{

			}
			else
			{
				if (type.IsPrimitive)
				{

				}
			}
		}
	}

	public class Clonner<T> : Clonner
	{
		private static T CloneSelf (T value) { return value; }

		private static T CloneObject(T value) { return (T)MemberwiseClone(value); }

		public readonly Converter<T,T> CloneMethod;

#if DEBUG 
	public
#elif
	protected
#endif
		Clonner() : base() 
		{ 
			var thisType = typeof(T);
			if (thisType.IsClass)
			{
				this.CloneMethod = CloneObject;
			}
			else if (thisType.IsInterface)
			{
				throw new NotSupportedException("Can't clone interface");
			}
			else
			{
				if (thisType.IsPrimitive)
				{
					this.CloneMethod = CloneSelf;
				}
			}
		}

		protected Clonner(bool Empty) : base() { }


	}
	*/
}

