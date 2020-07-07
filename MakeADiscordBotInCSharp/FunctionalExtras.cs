using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Madbics
{
	/// <summary>
	/// <para>A collection of static functions that fulfil miscellanious utility.</para>
	/// </summary>
	public static class FunctionalExtras
	{
		[DllImport("shell32.dll", SetLastError = true)]
		static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

		private static string[] CommandLineToArgsWindows(string commandLine)
		{
			var argv = CommandLineToArgvW(commandLine, out int argc);
			if (argv == IntPtr.Zero)
				throw new System.ComponentModel.Win32Exception();
			try
			{
				var args = new string[argc];
				for (var i = 0; i < args.Length; i++)
				{
					var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
					args[i] = Marshal.PtrToStringUni(p);
				}

				return args;
			}
			finally
			{
				Marshal.FreeHGlobal(argv);
			}
		}

		private static IEnumerable<string> Split(this string str, Func<char, bool> controller)
		{
			int nextPiece = 0;

			for (int c = 0; c < str.Length; c++)
			{
				if (controller(str[c]))
				{
					yield return str.Substring(nextPiece, c - nextPiece);
					nextPiece = c + 1;
				}
			}

			yield return str.Substring(nextPiece);
		}

		public static string TrimMatchingQuotes(this string input, char quote)
		{
			if ((input.Length >= 2) &&
				(input[0] == quote) && (input[input.Length - 1] == quote))
				return input.Substring(1, input.Length - 2);

			return input;
		}


		//Shamefully pinched from https://stackoverflow.com/a/298990/5490974.
		//This could possibly do with a better solution in the future.
		//Maybe just remove the call to shell32.dll and keep this general solution?
		private static string[] CommandLineToArgsUnix(string commandLine)
		{
			bool inQuotes = false;

			return commandLine.Split(c =>
			{
				if (c == '\"')
					inQuotes = !inQuotes;

				return !inQuotes && c == ' ';
			})
				//We want to copy the Windows behaviour as much as possible so we
				//don't remove empty entries as shown in the SO post.
							.Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
							.ToArray();
		}

		/// <summary>
		/// <para>This function takes a string and parses it the same way the native operating system's terminal does (or close approximate).</para>
		/// </summary>
		/// <param name="commandLine">What would be entered on the command line.</param>
		/// <returns>A string array of how the native operating system parsed the input string.</returns>
		public static string[] CommandLineToArgs(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine))
				return new string[] { };

			//Windows
			if (Environment.OSVersion.Platform < PlatformID.Unix)
			{
				return CommandLineToArgsWindows(commandLine);
			}
			else //Unix
			{
				return CommandLineToArgsUnix(commandLine);
			}
		}

		/// <summary>
		/// <para>Finds all classes of the specified type in the given assembly.</para>
		/// </summary>
		/// <param name="typeToFind">The type to find.</param>
		/// <param name="assemblyToSearch">The assembly to search through.</param>
		/// <returns></returns>
		public static IEnumerable<Type> GetTypesWithAttribute(Type typeToFind, Assembly assemblyToSearch)
		{
			foreach (Type type in assemblyToSearch.GetTypes())
			{
				if (type.GetCustomAttributes(typeToFind, true).Length > 0)
				{
					yield return type;
				}
			}
		}
	}
}
