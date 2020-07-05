using Madbics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MakeADiscordBotInCSharpTests
{
	[TestClass]
	public class UnitTests
	{
		[TestMethod]
		public void SimpleCommandLineArgumentsAreBrokenUpCorrectly()
		{
			const string command = "verb -option1 --option2 \"lots of words\"";

			var argParts = FunctionalExtras.CommandLineToArgs(command);

			Assert.AreEqual(4, argParts.Length);
		}

		[TestMethod]
		public void EmptyCommandLineArgumentsDoesNotThrowException()
		{
			string command = "";

			var argParts = FunctionalExtras.CommandLineToArgs(command);

			Assert.AreEqual(0, argParts.Length);

			command = null;

			argParts = FunctionalExtras.CommandLineToArgs(command);

			Assert.AreEqual(0, argParts.Length);
		}

		[TestMethod]
		public void UnequalQuotesDoesNotInterfereWithParsing()
		{
			const string command = "verb \"blah\" \" \"blah";

			var argParts = FunctionalExtras.CommandLineToArgs(command);

			Assert.AreEqual(3, argParts.Length);
		}

		[TestMethod]
		public void EmptyArgsAreStillParsed()
		{
			const string command = "verb \" \" \"\" \" something \"";

			var argParts = FunctionalExtras.CommandLineToArgs(command);

			Assert.AreEqual(4, argParts.Length);
			Assert.AreEqual(" ", argParts[1]);
			Assert.AreEqual("", argParts[2]);
			Assert.AreEqual(" something ", argParts[3]);
		}
	}
}
