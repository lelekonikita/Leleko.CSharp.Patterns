using System;
using System.Collections;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns
{
    /* Паттерн - Singleton */
	
    /// <summary>
    /// Базовый класс синглтона (один объект на приложение)
    /// </summary>
    public abstract class Singleton: Object, ISourceProvider
    {
        /// <summary>
        /// Экземпляр синглтона
        /// </summary>
        /// <typeparam name="TSingleton">тип синглтона</typeparam>
        public abstract class Instance<TSingleton>
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
			static Instance()
			{
				Value = (TSingleton)GetInstance(typeof(TSingleton));
			}
        }

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
            Singleton singleton = (Singleton)instansTable[singletonType];
            if (singleton != null)
                return singleton;
            if (singletonType.IsSubclassOf(typeof(Singleton)))
                return (Singleton)(Activator.CreateInstance(singletonType, true));
            else
                throw new ArgumentException("Запрашиваемый тип должен быть производным от Singleton", singletonType.FullName);
        }

        /// <summary>
        /// Защищенный конструктор
        /// </summary>
        protected Singleton()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            Hashtable instansTable = InstanceTable;
            lock (instansTable.SyncRoot)
            {
                // Регистрация экземпляра синглтона в таблице
                instansTable.Add(this.GetType(), this);
            }
			// Инициализация указанных синглтонов
			Attribute.GetCustomAttributes(this.GetType(), typeof(SingletonInitAttribute));
        }

		/// <summary>
		/// Получение хэш-кода экземпляра
		/// </summary>
		/// <returns>хэш-код</returns>
		public override int GetHashCode()
		{
			// Поскольку экземпляр является единственным представителем типа, то логично что и HashCode у него как у его типа
			return this.GetType().GetHashCode();
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

		#region [ Nested types ]
		
		public abstract class Selector: Singleton
		{
			public virtual IEnumerable GetKeys(Singleton singleton)
			{
				if (singleton == null)
					return null;

				var attributes = Attribute.GetCustomAttributes(singleton.GetType(),typeof(SingletonKeyAttribute),false);
				object[] keys = new object[attributes.Length];
				for (int i = 0; i < keys.Length; i++)
					keys[i] = ((SingletonKeyAttribute)attributes[i]).Key;
				return keys;
			}
			
			public virtual IEnumerable<Singleton> InitTypes
			{
				get { return null; }
			}
		}
		
		#endregion

		#region ISourceProvider implementation

		ISourceProvider ISourceProvider.Get(object key)
		{
			if ((key is Type) && (key as Type).IsSubclassOf(typeof(Singleton)))
				return GetInstance(key as Type);
			return null;
		}

		#endregion

		#region ISource implementation

		object ISource.Value
		{
			get
			{
				return this;
			}
		}

		#endregion
    }




}
