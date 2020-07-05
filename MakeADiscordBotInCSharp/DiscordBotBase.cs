using CommandLine;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//Put in a note for people wrt publishing and how "long windows paths" will need to be enabled
//Also running as admin

/// <summary>
/// <para><c>Madbics</c> is Make A Discord Bot In C Sharp!</para>
/// </summary>
namespace Madbics
{
	/// <summary>
	/// <para>The <c>IDiscordAction</c> interface is a single method interface that exposes the callback which should be called
	/// when a command is sent to the bot.</para>
	/// </summary>
	public interface IDiscordAction
	{
		/// <summary>
		/// <para>This method is called when a successful parse is made for the verb-class it resides inside.</para>
		/// </summary>
		/// <param name="message">The message that was successfully parsed. Useful information can be access from this object such as the channel from where the message was recieved.</param>
		/// <param name="botClient">The client the discord bot is logged in through.</param>
		void PerformAction(SocketMessage message, DiscordSocketClient botClient);
	}

	[Verb("Example-Verb", HelpText = "This is what a description for a command will look like. This command will echo back the string that follows it.")]
	public class ExampleVerb : IDiscordAction
	{
		[Value(0, Required = true, HelpText = "The string that will be echoed back. IF you string has whitespace remember to encase it in quotes e.g. \"Hello World!\"")]
		public string EchoString { get; set; }

		[Option("Option-Switch", Default = false, HelpText = "This is an example option. When present it will print the current time.")]
		public bool AnOptionSwitch { get; set; }

		public void PerformAction(SocketMessage message, DiscordSocketClient _)
		{
			message.Channel.SendMessageAsync($"Echoing back: {EchoString} {(AnOptionSwitch ? DateTime.Now.ToString() : "")}");
		}
	}

	/// <summary>
	/// <para>A base class you can build your discord bot from or use the functionality it currently has.</para>
	/// </summary>
	public class DiscordBotBase
	{
		private readonly Version m_baseVersion = new Version(0, 0, 0, 0);
		private readonly Uri m_gitLink = new Uri("http://www.google.com");

		private readonly string m_loginToken;
		private readonly Parser m_commandParser;
		private readonly StringWriter m_errorStream;
		private readonly IEnumerable<Type> m_commands;

		private DiscordSocketClient m_client;

		/// <summary>
		/// <para>Creates an instance of the discord bot class. Will search the calling assembly for all <c>VerbAttributes</c> and apply them to its command parsing rules..</para>
		/// </summary>
		/// <param name="loginToken"></param>
		public DiscordBotBase(string loginToken)
		{
			m_loginToken = loginToken;
			m_errorStream = new StringWriter();

			//We gather everything with the VerbAttribute, which will be used as the parsing rules.
			var commandList = FunctionalExtras.GetTypesWithAttribute(typeof(VerbAttribute), Assembly.GetEntryAssembly());
			if (commandList.Count() < 1)
			{
				commandList = new Type[] { typeof(ExampleVerb) };
			}
			m_commands = commandList;
			
			m_commandParser = new Parser(settings =>
			{
				settings.HelpWriter = m_errorStream;
			});
		}

		/// <summary>
		/// <para>Called by the <c>ReadCommand</c> method when it deems a message suitable for parsing.</para>
		/// <para>Uses the parsing rules as defined by classes that use the <c>VerbAttribute</c> with the <c>IDiscordAction</c> interface.</para>
		/// <para>If a parse is successful then the <c>PerformAction</c> method of the parsed verb class is called.</para>
		/// </summary>
		/// <param name="messageSentToBot">The message which is currently sending a command to the bot.</param>
		protected virtual void Parse(SocketMessage messageSentToBot)
		{
			ParserResult<object> parseResult;
			string interceptedOutput;

			m_errorStream.GetStringBuilder().Clear();
			var commandLineArgs = FunctionalExtras.CommandLineToArgs(messageSentToBot.ToString()).Skip(1);

			parseResult = m_commandParser.ParseArguments(commandLineArgs, m_commands.ToArray());

			interceptedOutput = m_errorStream.GetStringBuilder().ToString();

			parseResult.MapResult(
				(IDiscordAction opt) => Task.Run(() => opt.PerformAction(messageSentToBot, m_client)),
				errs => Task.Run(() => HandleErrors(errs, interceptedOutput, messageSentToBot))
			);
		}

		/// <summary>
		/// <para>This method is called when there is an error trying to parse a command.</para>
		/// </summary>
		/// <param name="parseErrors">The parse errors from the command line parser.</param>
		/// <param name="interceptedOutput">What would have been printed to the console running the discord.</param>
		/// <param name="messageSentToBot">The message from which the command was received.</param>
		protected void HandleErrors(IEnumerable<Error> parseErrors, string interceptedOutput, SocketMessage messageSentToBot)
		{
			if (parseErrors.All(err => err.Tag == ErrorType.HelpRequestedError || err.Tag == ErrorType.HelpVerbRequestedError))
			{
				messageSentToBot.Author.SendMessageAsync("Here's what I can do!\n" +
					$"```{interceptedOutput}```");
			}
			else if (parseErrors.All(err => err.Tag == ErrorType.VersionRequestedError))
			{
				messageSentToBot.Author.SendMessageAsync($"I was made by using the git project {m_gitLink} `version {m_baseVersion}` as a base.\n" +
					"The version of this particular bot is as follows: " +
					$"`{interceptedOutput}`");
			}
			else
			{
				messageSentToBot.Author.SendMessageAsync("Oh dear! I didn't understand that.\n" +
					$"```{interceptedOutput}```");
			}
		}

		/// <summary>
		/// <para>Starts the discord bot and logs in using it's login key.</para>
		/// <para>Adds the <c>Log</c> method to the callbacks called on the Discord Framework's Log event.</para>
		/// <para>Adds the <c>ReadCommand</c> method to the callbacks called on the Discord Framework's MessageReceived event.</para>
		/// </summary>
		/// <returns>Does not actually return. Will wait forever until the bot is stopped.</returns>
		public async Task Start()
		{
			m_client = new DiscordSocketClient();
			m_client.Log += Log;
			m_client.MessageReceived += ReadCommand;

			if (string.IsNullOrEmpty(m_loginToken))
			{
				Console.Error.WriteLine("An empty or null login token was provided to login with.");
				return;
			}

			await m_client.LoginAsync(TokenType.Bot, m_loginToken);
			await m_client.StartAsync();

			_ = Task.Delay(-1);
		}

		/// <summary>
		/// <para>A standard logging function that is set as a callback in the <c>Start</c> method.</para>
		/// </summary>
		/// <param name="msg">The log message from Discord's framework.</param>
		/// <returns>A task that represents the log message being sent to console.</returns>
		protected virtual Task Log(LogMessage msg)
		{
			return Task.Run(() => Console.WriteLine(msg.ToString()));
		}

		/// <summary>
		/// <para>This function is a callback set to fire on the discord bot's <c>MessageRecieved</c> event.
		/// This event fires when the bot recieves a message through any means, whether it be DM or in a channel.</para>
		/// <para>This method by default will only try to parse a message if it meets the following criteria:
		/// The message is from a user (not a bot),
		/// the bot is the only mentioned user, and the bots mention comes at the beginning of the message.</para>
		/// </summary>
		/// <param name="arg">Data recieved from the <c>MessageReceived</c> event</param>
		/// <returns>A task that represents the parsing and the callback served by the parser, either a parse-success or parse-error routine.</returns>
		protected virtual Task ReadCommand(SocketMessage arg)
		{
			//If the message isn't from a user then we don't need to interpret it at all.
			if (arg.Source != MessageSource.User)
			{
				return Task.CompletedTask;
			}

			//If the bot isn't the only one mentioned then we don't interpret it.
			if (arg.MentionedUsers.FirstOrDefault()?.Id != m_client.CurrentUser.Id)
			{
				return Task.CompletedTask;
			}

			//The mention didn't happen at the start of the message
			if (!Regex.IsMatch(arg.ToString(), $"^<@!?{m_client.CurrentUser.Id}>"))
			{
				return Task.CompletedTask;
			}

			return Task.Run(() => Parse(arg));
		}
	}
}
