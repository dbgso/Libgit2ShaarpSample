using System;
using LibGit2Sharp;

namespace GitTest
{
	public class RegisterInfo
	{
		public string baseDirPath{ get; set;}
		public string modifyDirPath{ get; set;}
		public Signature author{ get; set;}
		public string gitRepositoryPath{ get; set;}
		public BranchInfo branch{ get; set;}
		public string commitMessage {get;set;}

		public RegisterInfo ()
		{
		}
	}
}

