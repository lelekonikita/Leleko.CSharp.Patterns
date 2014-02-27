using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Leleko.CSharp.Patterns.Creation
{
	/* Паттерн - Мультитон */

	public abstract class Multiton
	{
		/// <summary>
		/// Объект был удален
		/// </summary>
		bool isRemoved;

		/// <summary>
		/// Gets a value indicating whether this instance is removed.
		/// </summary>
		/// <value><c>true</c> if this instance is removed; otherwise, <c>false</c>.</value>
		public virtual bool IsRemoved { get { return this.isRemoved; } protected set { this.isRemoved = value; } }

		protected Multiton()
		{
			this.isRemoved = false;
		}
	}
	
	/// <summary>
	/// Multiton (пул одиночек) - глобальная точка доступа, содержащая экземпляры самого себя, идентифицируемые ключем
	/// </summary>
	public abstract partial class Multiton<TKey>: Multiton
	{
		/// <summary>
		/// Отобранные контроллеры с ключем по типу контролируемого мультитона
		/// </summary>
		protected static readonly IDictionary<Type,Multiton<TKey>.Controller> ControllerTable;

		/// <summary>
		/// Initializes the <see cref="Leleko.CSharp.Patterns.Multiton`1"/> class.
		/// </summary>
		static Multiton()
		{
			ControllerTable = Singleton.Selector<ControllerSelectRule,Type,Multiton<TKey>.Controller, Multiton<TKey>.Controller>.Value;
		}

		/// <summary>
		/// Gets the instance of multiton with current key.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="multitonType">Multiton type.</param>
		/// <param name="key">Key.</param>
		public static Multiton<TKey> GetInstance(Type multitonType, TKey key)
		{
			return Ctor.GetCtor(multitonType)(key);
		}
		 
		/// <summary>
		/// The key.
		/// </summary>
		readonly TKey key;

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public TKey Key { get { return this.key; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Leleko.CSharp.Patterns.Multiton`1"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		protected Multiton(TKey key)
			:base()
		{
			this.key = key;
			this.Initialize();
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		protected void Initialize()
		{
			Type thisType = this.GetType();
			Controller controller;

			if (ControllerTable.TryGetValue(thisType, out controller)) // если контроллер существует - берем его и регистрируем мультитон
				(controller as IMultitonController).RegistrateMultiton(this);
			else
				(Singleton.GetInstance(typeof(Controller<>).MakeGenericType(new Type[] { typeof(TKey), thisType })) as IMultitonController).RegistrateMultiton(this);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Leleko.CSharp.Patterns.Multiton`1"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Leleko.CSharp.Patterns.Multiton`1"/>.</returns>
		public override string ToString()
		{
			return string.Format("[Multiton<{0}<{1}>>: Key={2}, Removed={3}]", this.GetType().Name, typeof(TKey).Name, this.Key, this.IsRemoved);
		}
	}
}

