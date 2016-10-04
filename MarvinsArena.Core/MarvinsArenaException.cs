using System;

namespace MarvinsArena.Core
{
	[Serializable]
	public class MarvinsArenaException : Exception
	{
		public MarvinsArenaException() { }
		public MarvinsArenaException(string message) : base(message) { }
		public MarvinsArenaException(string message, Exception inner) : base(message, inner) { }

		protected MarvinsArenaException(System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context) :base (info, context) { }
	}
}
