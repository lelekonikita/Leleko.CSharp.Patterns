using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Singleton 
	 * Селектор - Selector - позволяет организовать отбор синглтонов по ключам, формируемым правилами
	 */

	/// <summary>
	/// Singleton (одиночка) - объект с единственным экземпляром типа на приложение
	/// </summary>
	public abstract partial class Singleton
	{
		/// <summary>
		/// Selector (отборщик) - отбирает синглтоны в соответсвии с правилом отбора
		/// </summary>
		public sealed class Selector<TSelectRule,TKey,TSingleton>: Singleton, IDictionary<TKey, TSingleton>, ISourceProvider
			where TSelectRule: Singleton.Rules.SelectRule<TKey>
			where TSingleton: Singleton
		{
			#region [ Self Instance - сам себя, ведь он синглтон ]

			/// <summary>
			/// Инстанс себя
			/// </summary>
			public static readonly Selector<TSelectRule, TKey, TSingleton> Value;

			/// <summary>
			/// управляющий правилом добавления синглтонов
			/// </summary>
			static readonly TSelectRule SelectRule;

			/// <summary>
			/// статический конструктор
			/// </summary>
			static Selector()
			{
				SelectRule = Singleton.Instance<TSelectRule>.Value;
				Value = Singleton.Instance<Selector<TSelectRule, TKey, TSingleton>>.Value;
			}

			#endregion

			internal Selector() :base() { }

			#region [ Dictionary ]

			/// <summary>
			/// таблица отобранных
			/// </summary>
			readonly Dictionary<TKey, TSingleton> selectingsTable = new Dictionary<TKey, TSingleton>();

			/// <summary>
			/// коллекция ключей
			/// </summary>
			/// <value>The keys.</value>
			public ICollection<TKey> Keys { get { return this.selectingsTable.Keys; } }

			/// <summary>
			/// коллекция значений
			/// </summary>
			/// <value>The values.</value>
			public ICollection<TSingleton> Values { get { return this.selectingsTable.Values; } }

			/// <summary>
			/// get singleton по ключу
			/// </summary>
			/// <param name="key">значение ключа</param>
			public TSingleton this[TKey key] { get { return this.selectingsTable[key] ; } }

			#endregion

			#region [ Selecting - отбор синглтонов ]
			
			/// <summary>
			/// Внутренний селектор - с помощью правила формирует ключи и заполняет ими таблицу
			/// </summary>
			/// <param name="singleton">объект синглтона</param>
			private void AddToSelect(TSingleton singleton)
			{
				var keysEnumerable = SelectRule.GetKeys(singleton);
				if (keysEnumerable != null)
				{
					var keyNumerator = keysEnumerable.GetEnumerator();
					if (keyNumerator.MoveNext())
					{
						lock ((this.selectingsTable as ICollection).SyncRoot)
						do
						{
							var key = keyNumerator.Current;
							if (key != null)
								this.selectingsTable.Add(key, singleton);
						} while (keyNumerator.MoveNext());
					}
				}
			}
			
			/// <summary>
			/// 'вешаемый' селектор - фильтрует по типу и передает внутреннему [TSingleton]
			/// </summary>
			/// <param name="singleton">объект синглтона</param>
			private void AddToSelect(Singleton singleton) { if (singleton is TSingleton) this.AddToSelect(singleton as TSingleton); }
			
			/// <summary>
			/// Постинициализация
			/// </summary>
			/// <remarks>'вешаем' наш селектор</remarks>
			protected override void DoAfterInitialize()
			{
				// Вызываем базовую постинициализацию
				base.DoAfterInitialize();
				// Добавляем селектор текущего типа
				base.AddSelectorsAdd(this.AddToSelect);
			}
			
			#endregion

			#region IDictionary implementation

			void IDictionary<TKey, TSingleton>.Add(TKey key, TSingleton value) { throw new NotImplementedException(); }

			public bool ContainsKey(TKey key) { return this.selectingsTable.ContainsKey(key); }

			bool IDictionary<TKey, TSingleton>.Remove(TKey key) { throw new NotImplementedException(); }

			public bool TryGetValue(TKey key, out TSingleton value) { return this.selectingsTable.TryGetValue(key, out value); }

			TSingleton IDictionary<TKey, TSingleton>.this[TKey key] 
			{ 
				get { return this.selectingsTable[key] ; }
				set { throw new NotImplementedException(); }
			}

			#endregion

			#region ICollection implementation

			void ICollection<KeyValuePair<TKey, TSingleton>>.Add(KeyValuePair<TKey, TSingleton> item) { (this as IDictionary<TKey, TSingleton>).Add(item.Key, item.Value); }

			void ICollection<KeyValuePair<TKey, TSingleton>>.Clear() { throw new NotImplementedException(); }

			bool ICollection<KeyValuePair<TKey, TSingleton>>.Contains(KeyValuePair<TKey, TSingleton> item)
			{
				TSingleton value;
				return this.selectingsTable.TryGetValue(item.Key, out value) && EqualityComparer<TSingleton>.Default.Equals(value,item.Value);
			}

			void ICollection<KeyValuePair<TKey, TSingleton>>.CopyTo(KeyValuePair<TKey, TSingleton>[] array, int arrayIndex)
			{
				if (array == null)
					throw new ArgumentNullException("array");
				foreach(var e in this.selectingsTable)
					array[arrayIndex++] = e;
			}

			bool ICollection<KeyValuePair<TKey, TSingleton>>.Remove(KeyValuePair<TKey, TSingleton> item) { throw new NotImplementedException(); }

			public int Count { get { return this.selectingsTable.Count; } }

			bool ICollection<KeyValuePair<TKey, TSingleton>>.IsReadOnly { get { return true; } }

			#endregion

			#region IEnumerable implementation

			IEnumerator<KeyValuePair<TKey, TSingleton>> IEnumerable<KeyValuePair<TKey, TSingleton>>.GetEnumerator() { return this.selectingsTable.GetEnumerator(); }

			IEnumerator IEnumerable.GetEnumerator() { return this.selectingsTable.GetEnumerator(); }

			#endregion

			#region ISourceProvider implementation
			
			ISourceProvider ISourceProvider.Get(object key)
			{
				if (key is TKey)
					return this.selectingsTable[(TKey)key];
				else if ((key is Type) && (key as Type).IsSubclassOf(typeof(Singleton)))
					return Singleton.GetInstance(key as Type);
				return null;
			}
			
			#endregion
		}
	}
	
}

