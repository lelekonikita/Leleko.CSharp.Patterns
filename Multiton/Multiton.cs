using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Мультитон */
	
	/// <summary>
	/// Multiton (пул одиночек) - глобальная точка доступа, содержащая экземпляры самого себя, идентифицируемые ключем
	/// </summary>
	public abstract partial class Multiton<TKey>
	{
		/// <summary>
		/// Отобранные контроллеры с ключем по типу контролируемого мультитона
		/// </summary>
		protected static readonly IDictionary<Type,Multiton<TKey>.Controller> ControllersTable;

		/// <summary>
		/// Initializes the <see cref="Leleko.CSharp.Patterns.Multiton`1"/> class.
		/// </summary>
		static Multiton()
		{
			ControllersTable = Singleton.Selector<ControllerSelectRule,Type,Multiton<TKey>.Controller>.Value;
		}
		 
		/// <summary>
		/// The key.
		/// </summary>
		public readonly TKey key;

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

			if (ControllersTable.TryGetValue(thisType, out controller)) // если контроллер существует - берем его и регистрируем мультитон
				(controller as IMultitonController).RegistrateMultiton(this);
			else
			{
				// если контроллер не существует - пробираемся по иерархии наследования аж до типа Multiton<TKey> чтобы определить тип ключа
				(Singleton.GetInstance(typeof(Controller<>).MakeGenericType(new Type[] { typeof(TKey), thisType })) as IMultitonController).RegistrateMultiton(this);
			}
		}

		public override string ToString()
		{
			return string.Format("[Multiton<{1}>: Key={0}]", Key, this.GetType().Name);
		}
	}
}

