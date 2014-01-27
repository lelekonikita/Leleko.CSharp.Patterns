using System;
using System.Collections;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns
{
    /* Паттерн - Singleton */

	/// <summary>
	/// Задает принципы инициализации сторонних синглтонов при инициализации онного
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class SingleInitAttribute: Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Leleko.CSharp.Patterns.SingleInitAttribute"/> class.
		/// </summary>
		/// <param name="singleObjectType">Single object type.</param>
		public SingleInitAttribute(Type singleObjectType)
		{
			// Инициализируем синглтон
			SingleObject.GetInstance(singleObjectType);
		}
	}

    /// <summary>
    /// Базовый класс синглтона (один объект на приложение)
    /// </summary>
    public abstract class SingleObject: Object
    {
        /// <summary>
        /// Экземпляр синглтона
        /// </summary>
        /// <typeparam name="TSingleObject">тип синглтона</typeparam>
        public abstract class Instance<TSingleObject>
            where TSingleObject : SingleObject
        {
            /// <summary>
            /// Экземпляр синглтона 
            /// <remarks>не возвращает экземпляров SingleObjectThread, для этого используйте SingleObjectThread.Instance[TSingleObject].Value</remarks>
            /// </summary>
            public static readonly TSingleObject Value;

			/// <summary>
			/// Инициализация поля
			/// </summary>
			static Instance()
			{
				Value = (TSingleObject)GetInstance(typeof(TSingleObject));
			}
        }

        /// <summary>
        /// Таблица экземпляров синглтонов
        /// </summary>
        static readonly Hashtable InstanceTable = new Hashtable();

        /// <summary>
        /// Get экземпляр синглтона по типу
        /// </summary>
        /// <param name="singleObjectType">тип синглтона</param>
        /// <returns>экземпляр синглтона</returns>
        /// <exception cref="System.ArgumentException">!singletoneType.IsSubclassOf(typeof(SingleObject))</exception>
        public static SingleObject GetInstance(Type singleObjectType)
        {
			if (singleObjectType == null)
				throw new ArgumentNullException("singleObjectType");

            Hashtable instansTable = InstanceTable;
            SingleObject singleton = (SingleObject)instansTable[singleObjectType];
            if (singleton != null)
                return singleton;
            if (singleObjectType.IsSubclassOf(typeof(SingleObject)))
                return (SingleObject)(Activator.CreateInstance(singleObjectType, true));
            else
                throw new ArgumentException("Запрашиваемый тип должен быть производным от SingleObject", singleObjectType.FullName);
        }

        /// <summary>
        /// Защищенный конструктор
        /// </summary>
        protected SingleObject()
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
			Attribute.GetCustomAttributes(this.GetType(), typeof(SingleInitAttribute));
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
		
		public abstract class Selector<TKey>: SingleObject
		{
			public abstract TKey GetKey(SingleObject singleObject);
			
			public virtual IEnumerable<SingleObject> InitTypes
			{
				get { return null; }
			}
		}
		
		#endregion

    }

	/// <summary>
	/// Класс синглтона, определяющий пул селектора с доступом по ключу
	/// </summary>
	public abstract class SingleObject<TSelector, TKey> : SingleObject
		where TSelector: SingleObject.Selector<TKey>
	{
		/// <summary>
		/// The rule instance
		/// </summary>
		public static readonly TSelector Selector = SingleObject.Instance<TSelector>.Value;

		static readonly Hashtable InstanceTable = new Hashtable();

		protected override void Initialize()
		{
			base.Initialize();
			Hashtable instansTable = InstanceTable;
			lock (instansTable.SyncRoot)
			{
				// Регистрация экземпляра синглтона в таблице
				instansTable.Add(Selector.GetKey(this), this);
			}
		}

		/// <summary>
		/// Gets the init keys.
		/// </summary>
		/// <value>The init types.</value>
		public static ICollection InitKeys { get { return InstanceTable.Keys; } } 
		
		/// <summary>
		/// Gets the init objects.
		/// </summary>
		/// <value>The init objects.</value>
		public new static ICollection InitObjects { get { return InstanceTable.Values; } }
	}

    
}
