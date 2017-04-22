using System;
using LibGit2Sharp;

namespace GitTest
{
	public class BaseFileUnmatchException : Exception
	{
		public BaseFileUnmatchException (Patch patch)
		{
		}
	}
}

