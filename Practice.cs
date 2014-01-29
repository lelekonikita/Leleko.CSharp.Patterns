using System;

namespace Leleko.CSharp.Patterns
{
	#region [ MethodsCaller ]

	[AttributeUsage(AttributeTargets.Method, Inherited = true)]
	public abstract class CallBaseAttribute: Attribute
	{
		public string Remark { get; set; }
	
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CallBaseBeforeAttribute: CallBaseAttribute
	{

	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CallBaseNoAttribute: CallBaseAttribute
	{
		
	}

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CallBaseAfterAttribute: CallBaseAttribute
	{
		
	}

	#endregion

	#region [ Singleton ]

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
			if (singletonType == null)
				throw new ArgumentNullException("singletonType");

			// Инициализируем синглтон
			Singleton.GetInstance(singletonType);
		}
	}

	/// <summary>
	/// Задает синглтону ассоциированный атрибут
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class SingletonAssociationAttribute: Attribute
	{
		/// <summary>
		/// Тип интерфейса с которым будет ассоциирован инстанс
		/// </summary>
		/// <value>The type of the class.</value>
		public Type ClassType { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Leleko.CSharp.Patterns.SingletonAssociationAttribute"/> class.
		/// </summary>
		/// <param name="classType">Тип класса</param>
		public SingletonAssociationAttribute(Type classType)
		{
			if (classType == null)
				throw new ArgumentNullException("classType");

			// Тип ассоциируемого класса
			this.ClassType = classType;
		}
	}
	
	/// <summary>
	/// Позволяет задавать ключ для SelectorAttribute при инициализации синглтонов
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class MultitonKeyAttribute: Attribute
	{
		public object Key { get; private set; }
		
		public MultitonKeyAttribute(object key)
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

	#endregion
}

