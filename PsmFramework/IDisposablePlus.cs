using System;

namespace PsmFramework
{
	//TODO: Probably better off removing IDisposable completely
	// and moving to a non-interface implimentation so that
	// Dispose doesn't need to be public.
	//And change to a more encompassing ObjectState.
	public interface IDisposablePlus : IDisposable
	{
		Boolean IsDisposed { get; }
	}
}

