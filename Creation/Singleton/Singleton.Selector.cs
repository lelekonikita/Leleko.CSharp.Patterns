using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns.Creation
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
		public sealed class Selector<TSelectRule, TKey, TValue, TSingleton>: Singleton, IDictionary<TKey, TValue>
			where TSelectRule: Singleton.Rules.SelectRule<TKey, TValue, TSingleton>
			where TSingleton: Singleton
		{
			#region [ Self Instance - сам себя, ведь он синглтон ]

			/// <summary>
			/// Инстанс себя
			/// </summary>
			public static readonly Selector<TSelectRule, TKey, TValue, TSingleton> Value;

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
				Value = Singleton.Instance<Selector<TSelectRule, TKey, TValue, TSingleton>>.Value;
			}

			#endregion

			protected Selector() :base() { }

			#region [ Dictionary ]

			/// <summary>
			/// таблица отобранных
			/// </summary>
			readonly Dictionary<TKey, TValue> selectingTable = new Dictionary<TKey, TValue>();

			/// <summary>
			/// коллекция ключей
			/// </summary>
			/// <value>The keys.</value>
			public ICollection<TKey> Keys { get { return this.selectingTable.Keys; } }

			/// <summary>
			/// коллекция значений
			/// </summary>
			/// <value>The values.</value>
			public ICollection<TValue> Values { get { return this.selectingTable.Values; } }

			/// <summary>
			/// get singleton по ключу
			/// </summary>
			/// <param name="key">значение ключа</param>
			public TValue this[TKey key] { get { return this.selectingTable[key] ; } }

			#endregion

			#region [ Selecting - отбор синглтонов ]
			
			/// <summary>
			/// Внутренний селектор - с помощью правила формирует ключи и заполняет ими таблицу
			/// </summary>
			/// <param name="singleton">объект синглтона</param>
			private void AddToSelect(TSingleton singleton)
			{
				var enumerable = SelectRule.Select(singleton);
				if (enumerable != null)
				{
					var enumerator = enumerable.GetEnumerator();
					if (enumerator.MoveNext())
					{
						lock ((this.selectingTable as ICollection).SyncRoot)
						do
						{
							var current = enumerator.Current;
							this.selectingTable.Add(current.Key, current.Value);
						} while (enumerator.MoveNext());
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

			void IDictionary<TKey, TValue>.Add(TKey key, TValue value) { throw new NotImplementedException(); }

			public bool ContainsKey(TKey key) { return this.selectingTable.ContainsKey(key); }

			bool IDictionary<TKey, TValue>.Remove(TKey key) { throw new NotImplementedException(); }

			public bool TryGetValue(TKey key, out TValue value) { return this.selectingTable.TryGetValue(key, out value); }

			TValue IDictionary<TKey, TValue>.this[TKey key] 
			{ 
				get { return this.selectingTable[key] ; }
				set { throw new NotImplementedException(); }
			}

			#endregion

			#region ICollection implementation

			void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { (this as IDictionary<TKey, TValue>).Add(item.Key, item.Value); }

			void ICollection<KeyValuePair<TKey, TValue>>.Clear() { throw new NotImplementedException(); }

			bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
			{
				TValue value;
				return this.selectingTable.TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value,item.Value);
			}

			void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				if (array == null)
					throw new ArgumentNullException("array");
				foreach(var e in this.selectingTable)
					array[arrayIndex++] = e;
			}

			bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) { throw new NotImplementedException(); }

			public int Count { get { return this.selectingTable.Count; } }

			bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return true; } }

			#endregion

			#region IEnumerable implementation

			IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() { return this.selectingTable.GetEnumerator(); }

			IEnumerator IEnumerable.GetEnumerator() { return this.selectingTable.GetEnumerator(); }

			#endregion

		}
	}
	
}

