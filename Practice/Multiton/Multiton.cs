using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Multiton */

	/// <summary>
	/// Multiton (пул одиночек) - объект содержащий в себе (Singleton) с доступом по ключу
	/// </summary>
	public sealed class Multiton<TSelectRule,TSelectable>: Singleton
		where TSelectRule: Singleton.Rule.SelectRule 
		where TSelectable: Singleton
	{
		#region [ Static ]

		/// <summary>
		/// Инстанс себя
		/// </summary>
		public static readonly Multiton<TSelectRule, TSelectable> Value;

		/// <summary>
		/// управляющий правилом добавления синглтонов
		/// </summary>
		static readonly TSelectRule SelectRule;

		/// <summary>
		/// статический конструктор
		/// </summary>
		static Multiton()
		{
			SelectRule = Singleton.Instance<TSelectRule>.Value;

			Value = Singleton.Instance<Multiton<TSelectRule,TSelectable>>.Value;
		}

		#endregion

		#region [ Selected Table - таблица выбранных ]

		/// <summary>
		/// отобранная таблица
		/// </summary>
		protected readonly Hashtable selectingsTable = new Hashtable();

		public ICollection SelectedKeys { get { return this.selectingsTable.Keys; } }

		public ICollection SelectedObjects { get { return this.selectingsTable.Values; } }

		#endregion

		protected Multiton()
			:base()
		{
		}

		/// <summary>
		/// Выдача значения по ключу
		/// </summary>
		/// <param name="key">ключ</param>
		public TSelectable Get(object key)
		{
			return (TSelectable)this.selectingsTable[key];
		}

		#region [ Selecting - отбор]

		/// <summary>
		/// Внутренний селектор - с помощью правила формирует ключи и заполняет ими таблицу
		/// </summary>
		/// <param name="singleton">объект синглтона</param>
		private void AddToSelect(TSelectable singleton)
		{
			var keysEnumerator = SelectRule.GetKeys(singleton);
			if (keysEnumerator != null)
			{
				var keyNumerator = keysEnumerator.GetEnumerator();
				if (keyNumerator.MoveNext())
				{
					lock (this.selectingsTable.SyncRoot)
						do
						{
							this.selectingsTable.Add(keyNumerator.Current, singleton);
						} while (keyNumerator.MoveNext());
				}
			}
		}

		/// <summary>
		/// 'вешаемый' селектор - фильтрует по типу и передает внутреннему [TSelectable]
		/// </summary>
		/// <param name="singleton">объект синглтона</param>
		private void AddToSelect(Singleton singleton)
		{
			if (singleton is TSelectable)
				this.AddToSelect(singleton as TSelectable);
		}

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

		#region ISourceProvider implementation
		
		ISourceProvider ISourceProvider.Get(object key)
		{
			return this.Get(key);
		}
		
		#endregion
	}
	
}

