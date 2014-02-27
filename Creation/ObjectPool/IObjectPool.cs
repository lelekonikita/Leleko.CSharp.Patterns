using System;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns.Creation
{
	public interface IObjectPool<T>
	{
		IPoolable<T> Get();

		void Release(IPoolable<T> poolable);
	}
}

