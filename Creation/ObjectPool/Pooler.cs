using System;
using Leleko.CSharp.Patterns.Creation;

namespace Leleko.CSharp.Patterns.Creation
{
	public abstract class Pooler<T>: Singleton
	{
		protected Pooler() { }

		public abstract bool TryCreate(out IPoolable<T> poolable);
	}
}

