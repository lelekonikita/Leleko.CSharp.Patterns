using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace Leleko.CSharp.Patterns
{
	public partial class Multiton<TKey>
	{
		/// Класс отвечающий за конструирование мультитонов
		private static class Ctor
		{
			/// <summary>
			/// таблица обернутых конструкторов 
			/// </summary>
			static readonly Dictionary<Type, Converter<TKey,Multiton<TKey>>> CtorTable = new Dictionary<Type, Converter<TKey, Multiton<TKey>>>();

			/// <summary>
			/// Get constructor delegate
			/// </summary>
			/// <param name="multitonType">Multiton type.</param>
			public static Converter<TKey,Multiton<TKey>> GetCtor(Type multitonType)
			{

				var ctorTable = CtorTable;
				lock ((ctorTable as IDictionary).SyncRoot)
				{
					Converter<TKey,Multiton<TKey>> ctor;
					if (!ctorTable.TryGetValue(multitonType, out ctor))
					{
						ctor = BuildCtor(multitonType);
						ctorTable.Add(multitonType,ctor);
					}
					return ctor;
				}
			}

			/// <summary>
			/// Build the ctor.
			/// </summary>
			/// <returns>The ctor.</returns>
			/// <param name="multitonType">Multiton type.</param>
			static Converter<TKey,Multiton<TKey>> BuildCtor(Type multitonType)
			{
				if (!multitonType.IsSubclassOf(typeof(Multiton<TKey>)))
					throw new InvalidCastException(string.Concat(multitonType.Name," /-> ",typeof(Multiton<TKey>).Name));

				var parametersTypes =  new Type[] { typeof(TKey) };
				
				var constructorInfo = multitonType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parametersTypes, null);
				if (constructorInfo == null)
					throw new NotSupportedException(string.Concat("Конструктор '",multitonType.Name,"(",typeof(TKey).Name,")' не найден, Multiton-производный тип должен иметь скрытый конструктор вида ..ctor(TKey key)"));
				
				DynamicMethod dynamicMethod = new DynamicMethod(string.Concat(multitonType.Name, constructorInfo.Name), multitonType, parametersTypes, true);
				var il = dynamicMethod.GetILGenerator();
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Newobj, constructorInfo);
				il.Emit(OpCodes.Ret);
				
				return dynamicMethod.CreateDelegate(typeof(Converter<TKey, Multiton<TKey>>)) as Converter<TKey, Multiton<TKey>>;
			}
		}
	}
}

