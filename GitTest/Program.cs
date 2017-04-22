using System;
using LibGit2Sharp;

namespace GitTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var r = new RegisterInfo (){ 
				repositoryPath=@"/path/to/git/repo",
				signature =new Signature("username","email",DateTime.Now),
				baseDirPath=@"/path/to/before/directory",
				modifyDirPath=@"/path/to/register/directory",
				branch = new BranchInfo(){baseBranch = "master", newBranch="develop"}
			};
			var g = new GitUpdate ();
			g.GitClean (r);
			g.UpdateToLatest (r);
			g.CheckPreviousFile (r);

		}
	}
}
