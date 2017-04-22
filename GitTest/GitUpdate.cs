using System;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace GitTest
{
	public class GitUpdate
	{
		public void GitClean (RegisterInfo register)
		{
			using (Repository repo = new Repository (register.gitRepositoryPath)) {
				
				CheckoutOptions options = new CheckoutOptions (){ CheckoutModifiers = CheckoutModifiers.Force };
				var files = repo.RetrieveStatus ()
					.Modified
					.Select (file => file.FilePath)
					.ToArray ();
				repo.CheckoutPaths (register.branch.baseBranch, files, options);

				repo.RetrieveStatus ()
					.Untracked
					.AsParallel ()
					.ForAll (file => {
					new FileInfo (Path.Combine (register.gitRepositoryPath, file.FilePath)).Delete ();
				});
			}
		}

		public void UpdateToLatest (RegisterInfo register)
		{
			using (Repository repo = new Repository (register.gitRepositoryPath)) {
				
				Commands.Pull (
					repo,
					register.author,
					new PullOptions () {
						FetchOptions = new FetchOptions () {
							CredentialsProvider = new CredentialsHandler ((url, userName, types) => new UsernamePasswordCredentials () {
								Username = "",
								Password = ""
							}),
							Prune = true
						},
						MergeOptions = new MergeOptions () {
							FastForwardStrategy = FastForwardStrategy.FastForwardOnly,
						}
					}
				);
			}
		}

		public void CheckPreviousFile (RegisterInfo register)
		{
			using (Repository repo = new Repository (register.gitRepositoryPath)) {
				Utils.instance.DirectoryCopy (register.baseDirPath, register.gitRepositoryPath);
				var modifiedFiles = repo.RetrieveStatus ()
					.Where (item => item.State != FileStatus.Ignored);
				if (modifiedFiles.Count () > 0)
					throw new BaseFileUnmatchException (repo.Diff.Compare<Patch> ());
			}
		}

		public void Load (RegisterInfo register)
		{
			using (Repository repo = new Repository (register.gitRepositoryPath)) {
				string from = register.branch.baseBranch;
				string name = register.branch.newBranch;
				Commands.Checkout (repo, from);
				var b = repo.Branches [name];
				if (b != null)
					repo.Branches.Remove (name);
				Branch featureBranch = repo.Branches.Add (name, from);
				Commands.Checkout (repo, featureBranch);

				var modifiedFiles = repo.RetrieveStatus ()
					.Where (item => item.State != FileStatus.Ignored);
				if (modifiedFiles.Count () > 0) {
					Commands.Stage (repo, "*");
					Signature author = register.author;
					Signature commitor = author;
					repo.Commit (register.commitMessage, author, commitor);
				}
			}
		}
	}
}

