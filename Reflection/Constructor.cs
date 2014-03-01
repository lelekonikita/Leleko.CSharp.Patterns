using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using Leleko.CSharp.Patterns.Creation;
using System.Collections.Generic;

namespace Leleko.CSharp.Reflection
{
	/// <summary>
	/// Динамическая точка для получения конструктора
	/// </summary>
	public static class Constructor
	{
		#region [ Delegates Cache ]

		/// <summary>
		/// Кэш подготовленных конструкторов
		/// </summary>
		static readonly Dictionary<Type, Delegate> CtorTable;

		/// <summary>
		/// Кэш делегатов, ключом делегата выступает его сигнатура {typeof(Targ0), ... , typeof(TargN), typeof(TResult)}, значением - ссылка на метод
		/// </summary>
		/// <remarks>таблица кэша обеспечивает важную вещь - привязку кэша к сигнатуре, а не к конкретному типу делегата </remarks>
		static readonly Dictionary<Type[],Delegate> CtorSignatureTable;

		/// <summary>
		/// Компаратор - обеспечивает структурное сравнение массивов типов
		/// </summary>
		class EqualityTypesComparer: IEqualityComparer<Type[]>
		{
			/// <summary>
			/// Equals the specified x and y.
			/// </summary>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			bool IEqualityComparer<Type[]>.Equals(Type[] x, Type[] y)
			{
				if (x.Length == y.Length)
				{
					for(int i = 0; i < x.Length; i++)
						if (!object.ReferenceEquals(x[i],y[i]))
							return false;
					return true;
				}
				return false;
			}

			/// <summary>
			/// Gets the hash code.
			/// </summary>
			/// <returns>The hash code.</returns>
			/// <param name="obj">Object.</param>
			int IEqualityComparer<Type[]>.GetHashCode(Type[] obj)
			{
				unchecked
				{
					int value = obj.Length;
					for (int i = 0; i < obj.Length; i++)
						value = (value << 5) - value + ((value >> 27) ^ obj[i].GetHashCode());
					return value;
				}
			}
		}

		static Constructor()
		{
			// инициализируем таблицу кэша делегатов
			CtorTable = new Dictionary<Type, Delegate>();

			// инициализируем таблицу кэша сигнатур делегатов
			CtorSignatureTable = new Dictionary<Type[], Delegate>(new EqualityTypesComparer());
		}

		#endregion

		/// <summary>
		/// Gets конструктор согласно типу делегата(сигнатуре)
		/// </summary>
		/// <returns>конструктор, если не найден - null</returns>
		/// <param name="delegateType">тип делегата</param>
		/// <exception cref="System.InvalidCastException">!delegateType.IsSubclassOf(typeof(Delegate))</exception>
		public static Delegate GetCtor(Type delegateType)
		{
			Delegate ctor;
			if (CtorTable.TryGetValue(delegateType, out ctor))
				return ctor;
			return BuildCtor(delegateType);
		}

		/// <summary>
		/// Создаем конструктор по типу делегата
		/// </summary>
		/// <returns>конструктор, если не найден - null</returns>
		/// <param name="delegateType">тип делегата</param>
		static Delegate BuildCtor(Type delegateType)
		{
			// проверяем на 'делегатность', получаем метод
			if (!delegateType.IsSubclassOf(typeof(Delegate)))
				throw new InvalidCastException("TDelegate must be subclass of Delgate");
			var methodInfo = delegateType.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public );

			// разбираем сигнатуру в массив типов {typeof(Targ0), ... , typeof(TargN), typeof(TResult)}
			var parameters = methodInfo.GetParameters();
			var types = new Type[parameters.Length + 1];
			for (int i = 0; i < parameters.Length; i++)
				types[i] = parameters[i].ParameterType;
			types[parameters.Length] = methodInfo.ReturnType;

			// определяем наличие конструктора с аналогичной сигнатурой в кэше сигнатур
			var ctorSignatureTable = CtorSignatureTable;
			lock ((ctorSignatureTable as ICollection).SyncRoot)
			{
				Delegate ctor;
				if (ctorSignatureTable.TryGetValue(types, out ctor))
					ctor = BuildFromDelegate(delegateType, ctor);	// построение делегата методом "переобертывания" точки входа
				else
				{
					ctor = BuildFromMethod(delegateType, methodInfo, parameters); 	// полноценное построение делегата с 0 
					ctorSignatureTable.Add(types, ctor);							// регистрация делегата в кэше сигнатур
				}
				CtorTable.Add(delegateType, ctor);	// регистрация делегата в кэше делегатов
				return ctor;	// возвращаем делегат, содержащий конструктор или null, 
			}
		}

		/// <summary>
		/// Builds from method.
		/// </summary>
		/// <returns>The from method.</returns>
		/// <param name="delegateType">Delegate type.</param>
		/// <param name="methodInfo">Method info.</param>
		/// <param name="parameters">Parameters.</param>
		static Delegate BuildFromMethod(Type delegateType, MethodInfo methodInfo, ParameterInfo[] parameters)
		{
			// строим массив типов параметров
			var parametersTypes = parameters.Length > 0 ? new Type[parameters.Length] : Type.EmptyTypes;
			for (int i = 0; i < parameters.Length; i++)
				parametersTypes[i] = parameters[i].ParameterType;

			// получаем конструктор по сигнатуре параметров
			var constructorInfo = methodInfo.ReturnType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parametersTypes, null);
			if (constructorInfo == null)
				return null;

			// стоим динамический метод по вызову конструктора
			DynamicMethod dynamicMethod = new DynamicMethod(string.Concat(methodInfo.ReturnType.FullName, methodInfo.Name), methodInfo.ReturnType, parametersTypes, true);
			var il = dynamicMethod.GetILGenerator();
			for (int i = 0; i < parameters.Length; i++)
				il.Emit(OpCodes.Ldarg_S, i);
			il.Emit(OpCodes.Newobj, constructorInfo);
			il.Emit(OpCodes.Ret);

			// компилируем его в делегат
			return dynamicMethod.CreateDelegate(delegateType);
		}

		/// <summary>
		/// Builds from delegate.
		/// </summary>
		/// <returns>The from delegate.</returns>
		/// <param name="delegateType">Delegate type.</param>
		/// <param name="ctor">Ctor.</param>
		static Delegate BuildFromDelegate(Type delegateType, Delegate ctor)
		{
			if (ctor == null)
				return null;
			else if (delegateType.IsAssignableFrom(ctor.GetType())) // они приводятся средствами CLR - отлично! - выполняем
				return ctor;
			else     
				return Delegate.CreateDelegate(delegateType,ctor.Method); // переобертываем метод делегатом новго типа
		}
	}

	/// <summary>
	/// Статическая точка для получения конструктора
	/// <example>Constructor[Func[double,double,Point]].Ctor(1.1,2.2)</example>
	/// </summary>
	public static class Constructor<TDelegate>
		where TDelegate: class
	{
		/// <summary>
		/// The constructor
		/// </summary>
		public static readonly TDelegate Ctor;

		static Constructor() { Ctor = Constructor.GetCtor(typeof(TDelegate)) as TDelegate; }
	}
}

