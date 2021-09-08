using System;

namespace Consensus
{
	public class ConsensusAttribute : Attribute
	{
		public PatchEnumerator ToPatch { get; } = PatchEnumerator.MoveNext;

		public ConsensusAttribute()
		{
		}

		public ConsensusAttribute(PatchEnumerator toPatch)
		{
			ToPatch = toPatch;
		}
	}

	public enum PatchEnumerator
	{
		Current,
		MoveNext
	}
}