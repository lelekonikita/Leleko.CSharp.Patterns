using System;
using System.Collections;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns
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
			public abstract class Rule : Singleton
			{

			}

			public class SelectRule<TKey>: Rule
			{
				public virtual IEnumerable<TKey> GetKeys(Singleton singleton)
				{
					if (singleton == null)
						return null;
					
					var attributes = Attribute.GetCustomAttributes(singleton.GetType(),typeof(SingletonKeyAttribute),false);
					TKey[] keys = new TKey[attributes.Length];
					for (int i = 0; i < keys.Length; i++)
						keys[i] = (TKey)((SingletonKeyAttribute)attributes[i]).Key;
					return keys;
				}
			}
		}
	}
}

