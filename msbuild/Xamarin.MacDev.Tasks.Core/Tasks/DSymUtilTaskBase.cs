using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class DSymUtilTaskBase : XamarinToolTask
	{
		#region Inputs

		[Required]
		public string AppBundleDir { get; set; }

		public string Architectures { get; set; }

		[Required]
		public string DSymDir { get; set; }

		[Output]
		[Required]
		public ITaskItem Executable { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] DsymContentFiles { get; set; }

		#endregion

		protected override string ToolName {
			get { return "dsymutil"; }
		}

		public override bool Execute ()
		{
			var result = base.Execute ();

			var contentsDir = Path.Combine (DSymDir, "Contents");
			if (Directory.Exists(contentsDir))
				DsymContentFiles = Directory.EnumerateFiles (contentsDir).Select (x => new TaskItem (x)).ToArray ();

			return result;
		}

		protected override string GenerateFullPathToTool ()
		{
			if (!string.IsNullOrEmpty (ToolPath))
				return Path.Combine (ToolPath, ToolExe);

			var path = Path.Combine (AppleSdkSettings.DeveloperRoot, "Toolchains", "XcodeDefault.xctoolchain", "usr", "bin", ToolExe);

			return File.Exists (path) ? path : ToolExe;
		}

		protected override string GenerateCommandLineCommands ()
		{
			var args = new CommandLineBuilder ();

			args.AppendSwitch ("-num-threads");
			args.AppendSwitch ("4");
			args.AppendSwitch ("-z");
			args.AppendSwitch ("-o");
			args.AppendFileNameIfNotNull (DSymDir);
			args.AppendFileNameIfNotNull (Executable.ItemSpec);

			return args.ToString ();
		}

		protected override void LogEventsFromTextOutput (string singleLine, MessageImportance messageImportance)
		{
			if (singleLine.StartsWith ("warning:", StringComparison.Ordinal) && !singleLine.Contains ("unable to open object file: No such file or directory"))
				Log.LogWarning (singleLine);
			else
				Log.LogMessage (messageImportance, singleLine);
		}
	}
}
