using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Multiton */
	
	/// <summary>
	/// Класс мультитона, определяющий пул селектора с доступом по ключу
	/// </summary>
	public abstract class Multiton<TSelector> : Singleton
		where TSelector: Singleton.Selector
	{
		/// <summary>
		/// The rule instance
		/// </summary>
		public static readonly TSelector Selector = Singleton.Instance<TSelector>.Value;
		
		static readonly Hashtable InstanceTable = new Hashtable();
		
		protected override void Initialize()
		{
			base.Initialize();
			
			// регистрируем по ключам в селекторе
			var keysEnumerator = Selector.GetKeys(this);
			if (keysEnumerator != null)
			{
				var keyNumerator = keysEnumerator.GetEnumerator();
				if (keyNumerator.MoveNext())
				{
					Hashtable instansTable = InstanceTable;
					lock (instansTable.SyncRoot)
					{
						do
						{
							instansTable.Add(keyNumerator.Current, this);
						} while (keyNumerator.MoveNext());
					}
				}
			}
		}

		public static Multiton<TSelector> Get(object key)
		{
			return InstanceTable[key] as Multiton<TSelector>;
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

		#region ISourceProvider implementation
		
		ISourceProvider ISourceProvider.Get(object key)
		{
			return Get(key);
		}
		
		#endregion
	}
}

