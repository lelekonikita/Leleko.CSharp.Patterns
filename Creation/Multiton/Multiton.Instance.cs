using System;

namespace Leleko.CSharp.Patterns.Creation
{
	public abstract partial class Multiton<TKey>
	{
		/// <summary>
		/// Экземпляр контроллера мультитона
		/// </summary>
		/// <typeparam name="TMultiton">тип мультитона</typeparam>
		public static class Instance<TMultiton>
			where TMultiton : Multiton<TKey>
		{
			/// <summary>
			/// The instance of controller.
			/// </summary>
			public static readonly Controller<TMultiton> Value;

			static Instance() { Value = (Controller<TMultiton>)Singleton.GetInstance(typeof(Controller<TMultiton>)); }

			public static TMultiton GetInstance(TKey key)
			{
				return Value.GetInstance(key);
			}
		}
	}
}

