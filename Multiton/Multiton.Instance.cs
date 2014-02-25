using System;

namespace Leleko.CSharp.Patterns
{
	public partial class Multiton<TKey>
	{
		public static class Instance<TMultiton>
			where TMultiton : Multiton<TKey>
		{
			public static readonly Controller<TMultiton> Value;

			static Instance() { Value = (Controller<TMultiton>)Singleton.GetInstance(typeof(Controller<TMultiton>)); }
		}
	}
}

