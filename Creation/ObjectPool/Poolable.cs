using System;

namespace Leleko.CSharp.Patterns.Creation
{
	/// <summary>
	/// Poolable.
	/// </summary>
	public class Poolable<T> : IPoolable<T>
	{
		readonly T value;

		readonly IObjectPool<T> pool;

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public T Value { get { return this.value; } }

		protected virtual void ResetState() { }
		
		Poolable(IObjectPool<T> pool, T value)
		{
			this.pool = pool;
			
			this.value = value;
		}

		~Poolable()
		{
			this.pool.Release(this);
#if DEBUG
			throw new EntryPointNotFoundException("Poolable объект должен освобождаться с помощью IDisposable");
#endif
		}

		void IPoolable.ResetState()
		{
			this.ResetState();
		}

		void IDisposable.Dispose()
		{
			this.pool.Release(this);
		}
	}
}

