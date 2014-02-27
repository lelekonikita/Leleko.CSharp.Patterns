using System;
using System.Collections;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns.Creation
{
	/* Паттерн - Singleton 
	 * Правила - Rule
	 */

	/// <summary>
	/// Singleton (одиночка) - объект с единственным экземпляром типа на приложение
	/// </summary>
	public abstract partial class Singleton
	{
		/// <summary>
		/// Rules(правила) - правила для использования сцецифических синглтонов
		/// </summary>
		public static class Rules
		{
			/// <summary>
			/// Rule(правило) - правило так же является синглтоном
			/// </summary>
			public abstract class Rule : Singleton
			{
			}

			/// <summary>
			/// SelectRule(правило отбора) - правила согласно которым осуществляется выборка {ключ/значение} при событии инициализирования синглтона
			/// </summary>
			public abstract class SelectRule<TKey,TValue,TSingleton> : Rule
				where TSingleton: Singleton
			{
				public abstract IEnumerable<KeyValuePair<TKey, TValue>> Select(TSingleton singleton);
			}
		}
	}
}

