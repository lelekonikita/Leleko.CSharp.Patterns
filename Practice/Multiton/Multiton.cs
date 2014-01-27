using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Multiton */


	public static class Multiton
	{
		public static readonly List<Type> multitons = new List<Type>();

		public enum Rule
		{

		}
	}

	public abstract class Multiton<TKey, TSingleObject>
		where TSingleObject: Singleton
	{
		public interface IRule
		{

		}

		protected  Multiton()
		{
			throw new NotSupportedException("Can't create instance of multiton class");
		}
	}

	public abstract class Multiton<TRule, TKey, TSingleObject> : Multiton<TKey, TSingleObject>
		where TRule: Singleton, Multiton<TKey, TSingleObject>.IRule
		where TSingleObject: Singleton
	{
		/// <summary>
		/// The instance table.
		/// </summary>
		static readonly Hashtable InstanceTable = new Hashtable();

		public static readonly TRule Rule = Singleton.Instance<TRule>.Value;

		public static TSingleObject Get(TKey key)
		{
			throw new NotImplementedException();
		}
	}
}

