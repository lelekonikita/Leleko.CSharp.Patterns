using System;
using Leleko.CSharp.Patterns.Creation;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns.Action
{
	/*
	public abstract class Castor: Singleton
	{
		/// <summary>
		/// Возвращает HashCode который будет идентифицировать комбинацию {typeA,typeB}
		/// </summary>
		/// <returns>The hash code.</returns>
		/// <param name="typeA">Type a.</param>
		/// <param name="typeB">Type b.</param>
		public static long GetLongHashCode(Type typeA, Type typeB) { unchecked { return typeA.GetHashCode() ^ (((long)typeB.GetHashCode()) << 32); } }

		protected abstract long GetLongHashCode();

		public override int GetHashCode()
		{
			return this.GetLongHashCode().GetHashCode();
		}

	}

	abstract class Castor<TCastProvider>: Castor
		where TCastProvider: Singleton
	{
		/// <summary>
		/// Rule for selection castor's by LongHashCode
		/// </summary>
		protected internal sealed class CastorSelectRule: Singleton.Rules.SelectRule<long>
		{
			/// <summary>
			/// Gets the keys.
			/// </summary>
			/// <returns>The keys.</returns>
			/// <param name="singleton">Singleton.</param>
			public override IEnumerable<long> GetKeys(Singleton singleton)
			{
				return new long[] { ((Castor<TCastProvider>)singleton).GetLongHashCode() };
			}
		}

		protected static readonly Singleton.Selector<CastorSelectRule,long,Castor<TCastProvider>> CastorTable;

		static Castor()
		{
			CastorTable = Singleton.Selector<CastorSelectRule,long,Castor<TCastProvider>>.Value;
		}
	}

	class Castor<TCastProvider, TInput, TOutput>: Castor<TCastProvider>
		where TCastProvider: Singleton
	{
		protected override long GetLongHashCode()
		{
			return GetLongHashCode(typeof(TInput), typeof(TOutput));
		}
	}
*/

}

