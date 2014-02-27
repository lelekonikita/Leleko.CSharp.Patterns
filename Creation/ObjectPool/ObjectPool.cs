using System;
using System.Collections.Generic;
using System.Collections;

namespace Leleko.CSharp.Patterns.Creation
{
	public abstract class ObjectPool<T, TPooler>: IObjectPool<T>
		where TPooler: Pooler<T>
	{
		static readonly TPooler Pooler = Singleton.Instance<TPooler>.Value;

		readonly Stack<IPoolable<T>> objectsPool;

		public IPoolable<T> Get()
		{
			IPoolable<T> poolable;
			if (this.TryGet(out poolable))
			{
				GC.ReRegisterForFinalize(poolable);
				return poolable;
			}
			else
				throw new InvalidOperationException("Cant't create more instances");
		}

		protected virtual bool TryGet(out IPoolable<T> poolable)
		{
			lock ((this.objectsPool as ICollection).SyncRoot)
			{
				if (this.objectsPool.Count != 0)
				{
					poolable = this.objectsPool.Pop();
					return true;
				}
				else if (Pooler.TryCreate(out poolable))
					return true;
			}
			poolable = null;
			return false;
		}

		void IObjectPool<T>.Release(IPoolable<T> poolable)
		{
			lock ((this.objectsPool as ICollection).SyncRoot)
				this.objectsPool.Push(poolable);
			GC.SuppressFinalize(poolable);
		}
	}
}

