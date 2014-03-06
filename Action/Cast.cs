using System;
using Leleko.CSharp.Patterns.Creation;
using System.Collections.Generic;
using System.Reflection;

namespace Leleko.CSharp.Patterns.Action
{
	public interface ICast<TInput,TOutput> 
	{
		Converter<TInput, TOutput> Convert { get; }
		
		Converter<TInput, Object> ConvertInputToObj { get; }
		
		Converter<Object, Object> ConvertObjToObj { get; }
		
		Converter<Object, TOutput> ConvertObjToOutput { get; }
	}

	public abstract class Cast: Singleton
	{
		/// <summary>
		/// Возвращает HashCode который будет идентифицировать комбинацию {typeA,typeB}
		/// </summary>
		/// <returns>The hash code.</returns>
		/// <param name="typeA">Type a.</param>
		/// <param name="typeB">Type b.</param>
		public static long GetUniqueHashCode(Type typeA, Type typeB) { unchecked { return typeA.GetHashCode() ^ (((long)typeB.GetHashCode()) << 32); } }

		protected abstract long GetUniqueHashCode();

		public override int GetHashCode()
		{
			return this.GetUniqueHashCode().GetHashCode();
		}
	}

	abstract class Cast<TCastProvider>: Cast
		where TCastProvider: Singleton
	{

	}

	class Cast<TCastProvider, TInput, TOutput>: Cast<TCastProvider>, ICast<TInput, TOutput>
		where TCastProvider: CastProvider
	{
		public readonly Converter<TInput, TOutput> Convert;

		public readonly Converter<TInput, Object> ConvertInputToObj;

		public readonly Converter<Object, Object> ConvertObjToObj;

		public readonly Converter<Object, TOutput> ConvertObjToOutput;

		protected override long GetUniqueHashCode()
		{
			return GetUniqueHashCode(typeof(TInput), typeof(TOutput));
		}

		#region ICast implementation

		Converter<TInput, TOutput> ICast<TInput, TOutput>.Convert { get { return this.Convert; } }

		Converter<TInput, object> ICast<TInput, TOutput>.ConvertInputToObj { get { return this.ConvertInputToObj; } }

		Converter<object, object> ICast<TInput, TOutput>.ConvertObjToObj { get { return this.ConvertObjToObj; } }

		Converter<object, TOutput> ICast<TInput, TOutput>.ConvertObjToOutput { get { return this.ConvertObjToOutput; } }

		#endregion
	}

	public class CastProvider: Singleton
	{
		public static class Helps
		{
			public static T AsThis<T>(T value)
			{
				return value;
			}
			
			public static readonly MethodInfo MethodAsThis = typeof(Helps).GetMethod("AsThis", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			
			public static TOutput AsClass<TInput, TOutput>(TInput value)
				where TOutput: class
			{
				return value as TOutput;
			}

			public static readonly MethodInfo MethodAsClass = typeof(Helps).GetMethod("AsClass", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		protected CastProvider() : base() {}

		public virtual Delegate MakeCast(Type inType, Type outType)
		{
			Type[] typesInOut = new Type[] { inType, outType };
			Type converterDelegate = typeof(Converter<,>).MakeGenericType(typesInOut);

			MethodInfo methodInfo;


			if (this.TryAssignable(inType, outType, out methodInfo)) 
			{ 
			}
			else if (this.TryCastOpeartor(inType, outType, out methodInfo))
			{
			}
			else
			{
				throw new NotImplementedException();
			}

			return Delegate.CreateDelegate(converterDelegate, methodInfo);
		}

		/// <summary>
		/// Обработка ситуации когда типы имеют прямое приведение outType.IsAssignableFrom(inType)
		/// </summary>
		protected virtual bool TryAssignable(Type inType, Type outType, out MethodInfo methodInfo)
		{
			if (!outType.IsAssignableFrom(inType)) // проверяем на институциональную приводимость
			{
				methodInfo = null;
				return false;
			}
			else if (!inType.IsValueType || outType.IsValueType)
			{
				if (outType.IsValueType && outType.IsGenericType && (outType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				{
					methodInfo = null;
					return false;
				}
				else
				{
					methodInfo = Helps.MethodAsThis.MakeGenericMethod(new Type[]{inType});
					return true;
				}
			}
			else
			{
				methodInfo = Helps.MethodAsClass.MakeGenericMethod(new Type[]{inType, outType});
				return true;
			}

		}

		protected virtual bool TryCastOpeartor(Type inType, Type outType, out MethodInfo methodInfo)
		{

			List<MethodInfo> canCasts = new List<MethodInfo>();

			foreach (var operatorName in new string[] {"op_Implicit", "op_Explicit"})
			{
				MemberInfo[] potentialCasts = inType.GetMember(operatorName, MemberTypes.Method, BindingFlags.Static | BindingFlags.Public);
				for (int i = 0; i < potentialCasts.Length; i++)
				{
					MethodInfo current = (MethodInfo)potentialCasts[i];
					if (current.ReturnType == outType)
					{
						var parameters = current.GetParameters();
						if ( parameters.Length == 1)
						{
							Type parameterType = parameters[0].ParameterType;
							if (parameterType == inType)
							{
								methodInfo = current;
								return true;
							}
							else if (parameterType.IsAssignableFrom(inType))
								canCasts.Add(current);
						}
					}
				}
			}

			Type[] inTypeArr = new Type[] { inType };
			if ( (methodInfo = outType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, inTypeArr, null)) != null )
				return true;
			if ( (methodInfo = outType.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null, inTypeArr, null)) != null ) 
				return true;

			// Абсолютно подходящих вариантов не найдено - ищем примерно подходящие

#warning [ Нужно обработать случай приведения типов для обоих вариантов ]
			throw new NotImplementedException();

			return false;
		}
	}

	public class X
	{
		public struct N<T>
		{
			int value;
			
			private static object Box(N<T> o)
			{
				Console.WriteLine("Box");
				return o.value;
			}
			
			private static N<T> Unbox(object o)
			{
				Console.WriteLine("Unbox");
				return new N<T>();
			}
		}

		public struct M<T>
		{
			int value;
		}

		public static void Nullable1(object o)
		{
			var nx = (int?)o;
		} 

		public static void N1(object o)
		{
			var nx = (N<int>)o;
		} 

		public static void M1(object o)
		{
			var nx = (M<int>)o;
		} 


	}
}

