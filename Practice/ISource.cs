using System;

namespace Leleko.CSharp.Patterns
{
	public interface ISource
	{
		object Value { get; }
	}

	public interface ISourceProvider: ISource
	{
		ISourceProvider Get(object key);
	}
}

