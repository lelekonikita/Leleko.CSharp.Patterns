using System;

namespace Leleko.CSharp.Patterns.Creation
{
	/// <summary>
	/// Singleton (одиночка) - объект с единственным экземпляром типа на приложение
	/// </summary>
	public abstract partial class Singleton
	{
		/// <summary>
		/// Экземпляр синглтона
		/// </summary>
		/// <typeparam name="TSingleton">тип синглтона</typeparam>
		public static class Instance<TSingleton>
			where TSingleton : Singleton
		{
			/// <summary>
			/// Экземпляр синглтона 
			/// <remarks>не возвращает экземпляров SingletonThread, для этого используйте SingletonThread.Instance[TSingleton].Value</remarks>
			/// </summary>
			public static readonly TSingleton Value;
			
			/// <summary>
			/// Инициализация поля
			/// </summary>
			static Instance() { Value = (TSingleton)Singleton.GetInstance(typeof(TSingleton)); }
		}
	}
}

