using System;
using System.Collections.Generic;
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

		/// <summary>
		/// <para>This function takes a string and parses it the same way the native operating system's terminal does.</para>
		/// </summary>
		/// <param name="commandLine">What would be entered on the command line.</param>
		/// <returns>A string array of how the native operating system parsed the input string.</returns>
		public static string[] CommandLineToArgs(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine))
				return new string[] { };

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
