# MakeADiscordBotInCSharp (Madbics)
A tiny framework from which to get started making a discord bot.

This framework doesn't do anything special, it just combines a popular [.NET Discord library](https://github.com/discord-net/Discord.Net) with the [CommandLineParser's library](https://github.com/commandlineparser/commandline).

No need to download the source in this project, a [NuGet package](https://www.nuget.org/packages/MakeADiscordBotInCSharp/) is available!

If you are a total novice to CSharp, Visual Studio or Discord Bot making then go to "Quickstart Guide For Total Beginners".

If you have some experience with all the aforementioned then follow these steps:
1. Create a new CSharp .NET console project.
1. Add the NuGet package "Madbics" to it through the NuGet package manager.
1. Add `using Madbics;` to your source file and in your `Main` method add `DiscordBotBase.CreateAndRun(args[0]);`.
1. Open your project proerties and set your *Application Arguments* (under DEBUG) to your bot's login token.
1. As a sanity check run your bot and send it the following message `@[your_bot_name] help` and it should respond with a formatted helptext.
1. If you want to know how to expand its functionality from this humble base go to "How do I add a command" in the FAQ section.

# Quickstart Guide For Total Beginners
If you already have Visual Studio then this guide will get you set up in 5 minutes.
## Windows
You will need:
* Visual Studio (Community 2019 is used in this used in this tutorial).
* A web browser.
* An internet connection.

### Download/ Install Visual Studio
1. Download and install Visual Studio from Microsoft if you do not already have it, don't worry the community edition is free.
1. During the setup process you will need to ensure that *.NET desktop development* is checked on the workloads setup.
> https://visualstudio.microsoft.com/downloads/

### Setup The Bot Project
1. When you have VS downloaded and installed open it and create a new project.
1. Filter languages to C# and select *Console App (.NET Core)* then click next.
1. Name your project and choose a location for it. Ensure *create new solution* is set and click create.
1. In the *Solution Explorer* pane right click your *project* entry (not the solution entry) and click *Manage NuGet Packages*.
1. In the opened pane, click *Browse* and search for *MakeADiscordBotInCSharp* (searching for *Madbics* should also work). Click on the closest match and hit install.
1. Your dependencies are now set up!

### Getting It Working
1. Your project should have been created with a default "Hello World" program, navigate back to it by double clicking *Program.cs* in the *Solution Explorer* pane.
1. Add `using Madbics;` to the top of your file.
1. Replace the `Console.WriteLine(...` line with `DiscordBotBase.CreateAndRun(args[0]);`
1. Your code should now look like the following:
```CS
using System;
using Madbics;

namespace Bot
{
	class Program
	{
		static void Main(string[] args)
		{
			DiscordBotBase.CreateAndRun(args[0]);
		}
	}
}
```
If you attempt to run this code you should get an error print out to console saying: "An empty or null login token was provided to login with." This is good and means everything is going according to plan.

#### Setup Discord Bot Client And Secret
1. Login to the following page https://discord.com/developers/applications, which is used for setting up the coms with Discord. Click the "New Application" button in the top right corner of the screen and give your bot a name.
1. Click on your newly made bot and navigate to its *Bot* page. On this page is information called *Token*, copy the token by clicking the copy button.
1. Go back to your Bot project in Visual Studio and right click on your *project* entry in the *Solution Explorer* pane. Select *Properties*.
1. Go to the debug tab and in *Application arguments* paste the copied *Token* in there. Encase the whole string in quotes for good measure.
You should not share the token with anyone, as it will allow them to control your bot, and it is bad practice to keep it directly in your code, hence why we pass it as an argument. The argument will be saved in `[location_of_csproj_file]\launchSettings.json`. Do not share that file with others unintentionally.

### Make Sure It's All Working
1. In your project in Visual Studio hit F5 to start the program in debug mode.
1. A console window should appear and should confirm the bot has logged in and is sitting idle.
```
23:26:42 Discord     Discord.Net v2.2.0 (API v6)
23:26:43 Gateway     Connecting
23:26:44 Gateway     Connected
23:26:44 Gateway     Ready
```
1. To add the bot to a server re-visit the discord applications page (https://discord.com/developers/applications).
1. Click on your bot and go to the OAuth2 tab.
1. In *SCOPES* tick *bot*, this will make another panel appear called *BOT PERMISSIONS*.
1. In *BOT PERMISSIONS* you will need only *Send Messages* for now however you may need to add more in the future.
1. In the *SCOPES* panel should be a URL you can copy, this is the invite link needed to put the bot in a server of your choice. I recommend you create a server for testing purposes and invite it there.
1. When your bot is in the server type the following: `@[name_of_your_bot] Example-Verb "I am alive"` (replace [name_of_your_bot] with what the bot is called in your server). You will need to ensure the "@[name_of_your_bot]" part of your string resolves to a mention.
1. The bot should respond with "Echoing back: I am alive".
1. Now try `@[name_of_your_bot] help`, and follow its advice.

### How To Expand
This framework doesn't do anything special, it just combines a popular [.NET Discord library](https://github.com/discord-net/Discord.Net) with the [CommandLineParser's library](https://github.com/commandlineparser/commandline).

#### Let's walk through adding a command.

Add `using Discord.WebSocket;`, `using CommandLine;` and `using System.Linq;` to your program. Above your `Program` class add a new class called `ReverseVerb` and declare it to use the `IDiscordAction` interface. Make the new class follow the interface by adding a method with the following signature `public void PerformAction(SocketMessage message, DiscordSocketClient botClient)`. Annotate this new class with `[Verb("reverse", HelpText = "Reverses the given message")]`. You should now have the following:
```CS
[Verb("Reverse", HelpText = "Reverses the message")]
	class ReverseOption : IDiscordAction
	{
		public void PerformAction(SocketMessage message, DiscordSocketClient botClient)
		{
			throw new NotImplementedException();
		}
	}
```

Add a `public string` property to this new class called `Message`. Annotate this property with `[Value(0, MetaName = "Message", HelpText = "The message that will be reversed.")]`. Edit the `PerformAction` method body to be `message.Channel.SendMessageAsync(new string(Message.Reverse().ToArray()));`. You should now have the following:
```CS
[Verb("Reverse", HelpText = "Reverses the message")]
	class ReverseOption : IDiscordAction
	{
		[Value(0, MetaName = "Message", HelpText = "The message that will be reversed.")]
		public string Message { get; set; }

		public void PerformAction(SocketMessage message, DiscordSocketClient botClient)
		{
			message.Channel.SendMessageAsync(new string(Message.Reverse().ToArray()));
		}
	}
```

Now it's time to test this! Run your bot code using F5 and in a server with your bot (or direct message with your bot) send `@[name_of_your_bot] help`. Your new command should appear in the help listing. At the moment your command works by taking a single *value* that it manipulates according to what you have written in the `PerformAction` method. If you try `@[name_of_your_bot] reverse "DING DONG"` and it should respond with `GNOD GNID`! Let's add a little more functionality.

Add a new `public bool` property to the `ReverseVerb` class called `PreserveWordOrder`. Give this property the annotation `[Option('p', "preserve-word-order", Default = false, HelpText = "Will keep order of words while reversing them.")]`. Now you need to change the body of the `PerformAction` method to follow what we want it to do i.e. keep the order of the words while reversing all of their letters. You can try this as an exercise for yourself or you can copy the solution following:
```CS
	[Verb("Reverse", HelpText = "Reverses the message")]
	class ReverseOption : IDiscordAction
	{
		[Value(0, MetaName = "Message", HelpText = "The message that will be reversed.")]
		public string Message { get; set; }

		[Option('p', "preserve-word-order", Default = false, HelpText = "Will keep order of words while reversing them.")]
		public bool PreserveWordOrder { get; set; }

		public void PerformAction(SocketMessage message, DiscordSocketClient botClient)
		{
			var output = new string(Message.Reverse().ToArray());

			if (PreserveWordOrder)
			{
				var words = output.Split(' ');
				output = string.Join(' ', words.Reverse());
			}

			message.Channel.SendMessageAsync(output);
		}
	}
```

Now try to run your command through your bot again. Run your program and send your bot the following message `@[name_of_your_bot] reverse "DING DONG"` and it should still respond with `GNOD GNID`. Now try `@[name_of_your_bot] reverse -p "DING DONG"` and it should respond with `GNID GNOD`.

Let's add one more option to make sure you're getting it. Add a `public char` property with the annotation `[Option('r', "replace-spaces", HelpText = "Define a letter to replace all spaces with.")]`. Again, you can choose to fill in this functionality as an exercise or you can simply use the following solution.
```CS
	[Verb("Reverse", HelpText = "Reverses the message")]
	class ReverseOption : IDiscordAction
	{
		[Value(0, MetaName = "Message", HelpText = "The message that will be reversed.")]
		public string Message { get; set; }

		[Option('p', "preserve-word-order", Default = false, HelpText = "Will keep order of words while reversing them.")]
		public bool PreserveWordOrder { get; set; }

		[Option('r', "replace-spaces", HelpText = "Define a letter to replace all spaces with.")]
		public char VowelReplacer { get; set; }

		public void PerformAction(SocketMessage message, DiscordSocketClient botClient)
		{
			var output = new string(Message.Reverse().ToArray());

			if (PreserveWordOrder)
			{
				var words = output.Split(' ');
				output = string.Join(' ', words.Reverse());
			}

			//We check if this is assigned or not and use that as an indication on whether to replace.
			if (VowelReplacer != default)
			{
				output.Replace(' ', VowelReplacer);
			}

			message.Channel.SendMessageAsync(output);
		}
	}
```

You now know what to do! Run your bot. Send it the following message `@[name_of_your_bot] reverse -r @ -p "DING DONG"` and it should say back to you `GNID@GNOD`. Congratulations you have added a function to your discord bot and fleshed it out with options and arguments.

### Extra Tips
- Keep [Discord's documentation handy](https://discord.com/developers/docs/intro) so you can work out how to interact with Discord's objects.
- Keep [CommandLineParser's documentation handy](https://github.com/commandlineparser/commandline/wiki) so you can work out how to make commands for your bot.

# FAQ
## How do I add a command?
Adding a command should be dead simple:
1. Add a class anywhere in your assembly annotated with `[Verb(...)]` that follows the `IDiscordAction` interface.
2. Configure the command how you wish, possibly by adding some `[Option(...)]`s.
3. You're done! The framework will pick up your new command and (provided you've filled this information in on the annotations) generate help-text for users to read and understand how your bot works.

For more information on these annotations see [this documentation](https://github.com/commandlineparser/commandline/wiki).

The command you create is run asyncronously meaning that your `PerformAction` method will execute while the bot attempts to listen for and execute more commands. This is both a blessing and a curse, so be wary!

**This section needs expanding. You can help that by being the question asker!**
