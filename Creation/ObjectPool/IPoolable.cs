using System;
using System.Collections;
using System.Collections.Generic;

namespace Leleko.CSharp.Patterns.Creation
{
	public interface IPoolable: IDisposable
	{
		void ResetState();
	}

	public interface IPoolable<out T> : IPoolable
	{
		T Value { get; }
	}

}

