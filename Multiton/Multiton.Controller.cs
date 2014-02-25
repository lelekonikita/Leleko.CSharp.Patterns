using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;


namespace Leleko.CSharp.Patterns
{
	/* Паттерн - Мультитон */
	/* Каждому типу мультитона - свой единственный, глобальный и неповторимый контроллер(синглтон)*/ 
	

	/// <summary>
	/// Multiton.IController - интерфейс контроллера мультитона
	/// </summary>
	interface IMultitonController
	{
		/// <summary>
		/// Gets the type of the controlled multiton.
		/// </summary>
		/// <value>The type of the multiton.</value>
		Type MultitonType { get; }
		
		/// <summary>
		/// Registerate the multiton.
		/// </summary>
		/// <param name="multiton">Multiton.</param>
		void RegistrateMultiton(object multiton);
	}
	
	public partial class Multiton<TKey>
	{
		public abstract class Controller : Singleton
		{

		}

		/// <summary>
		/// The controller class
		/// </summary>
		public sealed class Controller<TMultiton> : Controller, IDictionary<TKey, TMultiton>, IMultitonController, ISourceProvider, ISource
			where TMultiton: Multiton<TKey>
		{
			/// <summary>
			/// Конструктор мультитона
			/// </summary>
			readonly Converter<TKey, TMultiton> MultitonCtor;
			
			/// <summary>
			/// Таблица экземпляров мультитонов
			/// </summary>
			readonly Dictionary<TKey, TMultiton> InstanceTable = new Dictionary<TKey, TMultiton>();
			
			internal Controller() : base() 
			{ 
				// инициализируем точку быстрого доступа к конструкторам
				// посредством динамического метода
				
				var parametersTypes =  new Type[] { typeof(TKey) };
				
				var constructorInfo = typeof(TMultiton).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parametersTypes, null);
				if (constructorInfo == null)
					throw new NotSupportedException(string.Concat("constructor '",typeof(TMultiton).FullName,"(",typeof(TKey).FullName,")' not found"));
				
				DynamicMethod dynamicMethod = new DynamicMethod(string.Concat(typeof(TMultiton).FullName, constructorInfo.Name), typeof(TMultiton), parametersTypes, true);
				var il = dynamicMethod.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Newobj, constructorInfo);
				il.Emit(OpCodes.Ret);
				
				this.MultitonCtor = dynamicMethod.CreateDelegate(typeof(Converter<TKey, TMultiton>)) as Converter<TKey, TMultiton>;
			}

			/// <summary>
			/// Gets the type of the controlled multiton.
			/// </summary>
			/// <value>The type of the multiton.</value>
			public Type MultitonType { get { return typeof(TMultiton); } }

			/// <summary>
			/// Registerate the multiton.
			/// </summary>
			/// <param name="multiton">Multiton.</param>
			void IMultitonController.RegistrateMultiton(object multiton)
			{
				Dictionary<TKey, TMultiton> instansTable = InstanceTable;
				lock ((instansTable as IDictionary).SyncRoot)
				{
					var tMultition = (TMultiton)multiton;
					try
					{
						// Регистрация экземпляра синглтона в таблице
						instansTable.Add(tMultition.Key, tMultition);
					}
					catch(ArgumentException ex)
					{
						throw new ApplicationException(string.Concat("Класс ",tMultition.GetType().FullName, " является Multiton'ом и уже существует с ключом '",tMultition.Key,"'"),ex);
					}
				}
			}

			/// <summary>
			/// Get the instance by key
			/// </summary>
			/// <param name="key">Key.</param>
			public TMultiton Get(TKey key)
			{
				TMultiton value;
				var instanceTable = InstanceTable;
				if (instanceTable.TryGetValue(key, out value))
					return value;
				else return this.MultitonCtor(key);
			}

			#region IDictionary implementation

			void IDictionary<TKey, TMultiton>.Add(TKey key, TMultiton value) { throw new NotImplementedException(); }

			bool IDictionary<TKey, TMultiton>.ContainsKey(TKey key) { throw new NotImplementedException(); }

			bool IDictionary<TKey, TMultiton>.Remove(TKey key) { throw new NotImplementedException(); }

			bool IDictionary<TKey, TMultiton>.TryGetValue(TKey key, out TMultiton value) { throw new NotImplementedException(); }

			TMultiton IDictionary<TKey, TMultiton>.this[TKey key] { get { return this.Get(key); } set { throw new NotImplementedException(); } }

			public ICollection<TKey> Keys { get { return this.InstanceTable.Keys; } }

			public ICollection<TMultiton> Values { get { return this.InstanceTable.Values; } }

			#endregion

			#region ICollection implementation

			void ICollection<KeyValuePair<TKey, TMultiton>>.Add(KeyValuePair<TKey, TMultiton> item) { (this as IDictionary<TKey, TMultiton>).Add(item.Key, item.Value); }

			void ICollection<KeyValuePair<TKey, TMultiton>>.Clear() { throw new NotImplementedException(); } 

			bool ICollection<KeyValuePair<TKey, TMultiton>>.Contains(KeyValuePair<TKey, TMultiton> item) { throw new NotImplementedException(); }

			void ICollection<KeyValuePair<TKey, TMultiton>>.CopyTo(KeyValuePair<TKey, TMultiton>[] array, int arrayIndex) { throw new NotImplementedException(); }

			bool ICollection<KeyValuePair<TKey, TMultiton>>.Remove(KeyValuePair<TKey, TMultiton> item) { throw new NotImplementedException(); }

			int ICollection<KeyValuePair<TKey, TMultiton>>.Count { get { return this.InstanceTable.Count; } }

			bool ICollection<KeyValuePair<TKey, TMultiton>>.IsReadOnly { get { return true; } }

			#endregion

			#region IEnumerable implementation

			IEnumerator<KeyValuePair<TKey, TMultiton>> IEnumerable<KeyValuePair<TKey, TMultiton>>.GetEnumerator() { return this.InstanceTable.GetEnumerator(); }

			#endregion

			#region IEnumerable implementation

			IEnumerator IEnumerable.GetEnumerator() { return this.InstanceTable.GetEnumerator(); }

			#endregion
		}

		/// <summary>
		/// Rule for selection controllers by multiton types
		/// </summary>
		protected internal sealed class ControllerSelectRule: Singleton.Rules.SelectRule<Type>
		{
			/// <summary>
			/// Gets the keys.
			/// </summary>
			/// <returns>The keys.</returns>
			/// <param name="singleton">Singleton.</param>
			public override IEnumerable<Type> GetKeys(Singleton singleton)
			{
				yield return (singleton as IMultitonController).MultitonType;
			}
		}
	}
}

