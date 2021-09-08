using System;

namespace Consensus
{
	public class ConsensusTypeNotFoundException : Exception
	{
		public ConsensusTypeNotFoundException() : base("The Type of the IEnumerator could not be determined.")
		{
		}
	}
}