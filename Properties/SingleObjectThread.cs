using System;
using System.Collections;

namespace Leleko.CSharp.Patterns
{
	// Пока что в использовании отказано

	/*

	/// <summary>
	/// Базовый класс потокоизолированного синглтона (один объект на каждый поток)
	/// </summary>
	public abstract class SingleObjectThread : Object
	{
		/// <summary>
		/// Экземпляр синглтона
		/// </summary>
		/// <typeparam name="TSingleObject">тип синглтона</typeparam>
		public abstract class Instance<TSingleObjectThread>
			where TSingleObjectThread : SingleObjectThread
		{
			/// <summary>
			/// Экземпляр синглтона (потокоизолированный)
			/// </summary>
			[ThreadStatic]
			static TSingleObjectThread value;
			
			/// <summary>
			/// Экземпляр синглтона (потокоизолированный)
			/// </summary>
			public static TSingleObjectThread Value
			{
				get
				{
					return value ?? (value = (TSingleObjectThread)SingleObjectThread.GetInstance(typeof(TSingleObjectThread)));
				}
			}
		}
		
		/// <summary>
		/// Таблица экземпляров синглтонов (потокоизолированный)
		/// </summary>
		[ThreadStatic]
		static Hashtable InstanceTable;
		
		/// <summary>
		/// Get экземпляр синглтона по типу
		/// </summary>
		/// <param name="singleObjectType">тип синглтона</param>
		/// <returns>экземпляр синглтона</returns>
		/// <exception cref="System.ArgumentException">!singletoneType.IsSubclassOf(typeof(Singletone))</exception>
		public static SingleObjectThread GetInstance(Type singleObjectThreadType)
		{
			Hashtable instanceTable = (InstanceTable ?? (InstanceTable = new Hashtable()));
			SingleObjectThread singleObjectThread = (SingleObjectThread)instanceTable[singleObjectThreadType];
			if (singleObjectThread != null)
				return singleObjectThread;
			if (singleObjectThreadType.IsSubclassOf(typeof(SingleObjectThread)))
				return (SingleObjectThread)(Activator.CreateInstance(singleObjectThreadType, true));
			else
				throw new ArgumentException("Запрашиваемый тип должен быть производным от SingleObjectThread", singleObjectThreadType.FullName);
		}
		
		/// <summary>
		/// Защищенный конструктор
		/// </summary>
		protected SingleObjectThread()
		{
			this.Initialize();
		}
		
		/// <summary>
		/// Инициализатор
		/// </summary>
		protected virtual void Initialize()
		{
			Hashtable instansTable = InstanceTable;
			lock (instansTable.SyncRoot)
			{
				// Регистрация экземпляра синглтона в таблице
				instansTable.Add(this.GetType(), this);
			}
		}
	}

	*/
}

