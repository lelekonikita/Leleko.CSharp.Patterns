using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using System.Threading;

namespace Leleko.CSharp.Patterns.Creation
{
    /* Паттерн - Singleton */
	
    /// <summary>
    /// Singleton (одиночка) - объект с единственным экземпляром типа на приложение
    /// </summary>
    public abstract partial class Singleton: Object, ICloneable, IEquatable<Singleton>
    {
		/*
		/// <summary>
		/// Ассоциированный синглтон с интерфейсом
		/// </summary>
		/// <typeparam name="TInterface">тип интерфейса</typeparam>
		public static class Association<TClass>
			where TClass: class
		{
			public static Singleton Value { get; internal set; }
		}

		#region [ Association - регистрация ассоциации ]

		static void RegisterAssociationInternal<TClass>(Singleton singleton)
			where TClass: class 
		{
			Association<TClass>.Value = singleton;
		}

		static readonly MethodInfo RegisterAssociationInternalMi = typeof(Singleton).GetMethod("RegisterAssociationInternal", BindingFlags.Static | BindingFlags.NonPublic );

		static void RegisterAssociation(Type type, Singleton singleton)
		{
			RegisterAssociationInternalMi.MakeGenericMethod(type).Invoke(null, new object[] { singleton });
		}

		#endregion
		*/

		#region [ Static ]

        /// <summary>
        /// Таблица экземпляров синглтонов
        /// </summary>
        static readonly Hashtable InstanceTable = new Hashtable();

		/// <summary>
		/// Get экземпляр синглтона по типу
		/// </summary>
		/// <param name="singletonType">тип синглтона</param>
		/// <returns>экземпляр синглтона</returns>
		/// <exception cref="System.ArgumentException">!singletoneType.IsSubclassOf(typeof(Singleton))</exception>
		public static Singleton GetInstance(Type singletonType)
		{
			if (singletonType == null)
				throw new ArgumentNullException("singletonType");
			
			Hashtable instansTable = InstanceTable;
			lock(instansTable.SyncRoot)
			{
				Singleton singleton = (Singleton)instansTable[singletonType];
				if (singleton != null)
					return singleton;
				if (singletonType.IsSubclassOf(typeof(Singleton)))
					return (Singleton)(Activator.CreateInstance(singletonType, true));
				else
					throw new ArgumentException("Запрашиваемый тип должен быть производным от Singleton", singletonType.FullName);
			}
		}

		/// <summary>
		/// Gets the init types.
		/// </summary>
		/// <value>The init types.</value>
		public static ICollection InitTypes { get { return InstanceTable.Keys; } } 

		/// <summary>
		/// Gets the init objects.
		/// </summary>
		/// <value>The init objects.</value>
		public static ICollection InitObjects { get { return InstanceTable.Values; } }

		#endregion

		#region [ Selectors events ]

		/// <summary>
		/// Добавленные селекторы мн-ва синглтонов
		/// </summary>
		static Action<Singleton> SelectorsAdd = null;

		/// <summary>
		/// Adds the selectors add.
		/// </summary>
		/// <param name="selectorAdd">Selector add.</param>
		protected void AddSelectorsAdd(Action<Singleton> addToSelect)
		{
			if (addToSelect == null)
				throw new ArgumentNullException("addToSelect");

			Hashtable instansTable = InstanceTable;
			lock (instansTable.SyncRoot)
			{
				// добавляем селектор
				Singleton.SelectorsAdd += addToSelect;
				// перебираем все синглтоны из таблицы
				foreach(Singleton singleton in instansTable.Values)
					addToSelect(singleton);
			}
		}

		#endregion
		
		/// <summary>
		/// Защищенный конструктор
		/// </summary>
		internal protected Singleton()
		{
			this.Initialize();
		}

		/// <summary>
		/// Инициализатор
		/// </summary>
		/// <remarks></remarks>
		void Initialize()
		{
			Hashtable instansTable = InstanceTable;
			lock(instansTable.SyncRoot)
			{
				try
				{
					// Регистрация экземпляра синглтона в таблице
					instansTable.Add(this.GetType(), this);
				}
				catch(ArgumentException ex)
				{
					throw new ApplicationException(string.Concat("Класс ",this.GetType().FullName, " является Singleton'ом и поэтому не может иметь более 1го экземпляра в рамках одного приложения. Рекомендуется сделать конструктор класса закрытым а для запроса экземпляра использовать Singleton.Instance<TSingleton>.Value."),ex);
				}
			}

			// Уведомление селекторов о появлении нового синглтона
			if (SelectorsAdd != null)
				SelectorsAdd(this);
			// Вызываем после инициализации
			this.DoAfterInitialize();
		}

		/// <summary>
		/// Постинициализатор
		/// </summary>
		protected virtual void DoAfterInitialize() { }
		
		/// <summary>
		/// Получение хэш-кода экземпляра
		/// </summary>
		/// <returns>хэш-код</returns>
		public override int GetHashCode() { return this.GetType().GetHashCode(); } // Поскольку экземпляр является единственным представителем типа, то логично что и HashCode у него как у его типа

		#region IEquatable implementation

		public bool Equals(Singleton other) { return object.ReferenceEquals(this,other); }

		#endregion

		#region ICloneable implementation

		/// <summary>
		/// Clone this instance.
		/// </summary>
		object ICloneable.Clone()
		{
			return this;
		}

		#endregion
    }
	
}
