using System;

namespace Leleko.CSharp.Patterns
{

	public abstract class ObjectPool: Singleton
	{
		public abstract class PoolableObject: IDisposable
		{
			protected ObjectPool pool;

			~PoolableObject()
			{

			}

			#region IDisposable implementation
			public void Dispose()
			{
				throw new NotImplementedException();
			}
			#endregion
		}
	}

	public class ObjectPool<T>: ObjectPool
	{

	}
}

