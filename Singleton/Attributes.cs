using System;

namespace Leleko.CSharp.Patterns
{
	/// <summary>
	/// Задает принципы инициализации сторонних синглтонов при инициализации онного
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class SingletonInitAttribute: Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Leleko.CSharp.Patterns.SingleInitAttribute"/> class.
		/// </summary>
		/// <param name="singletonType">Single object type.</param>
		public SingletonInitAttribute(Type singletonType)
		{
			// Думаю это не нужная строка if (singletonType != null && !singletonType.IsAbstract && !singletonType.IsSubclassOf(typeof(Singleton)))
			Singleton.GetInstance(singletonType); // Инициализируем синглтон
		}
	}
	
	/// <summary>
	/// Позволяет задавать ключ для SelectorAttribute при инициализации синглтонов
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class SingletonKeyAttribute: Attribute
	{
		public object Key { get; private set; }

		/// <summary>
		/// Ключ, идентифицирующий синглтон
		/// </summary>
		/// <param name="key">Key.</param>
		public SingletonKeyAttribute(object key)
		{
			this.Key = key;
		}
	}
	
	/// <summary>
	/// Тип ресурса синглетона
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class SingletonSourceAttribute: Attribute
	{
	}
}

