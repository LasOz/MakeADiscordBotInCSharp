# MakeADiscordBotInCSharp (Madbics)
A tiny framework from which to get started making a discord bot.

No need to download the source in this project, a NuGet package is available!

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
This framework doesn't do anything special, it just combines Discord's library with CommandLineParser's library.

If you want to add a command all you need to do is the following:
THIS DOCUMENT IS A WIP.

### Extra Tips
- Keep Discord's documentation handy (link) so you can work out how to interact with Discord's objects.
- Keep CommandLineParser's documentation handy (link) so you can work out how to make commands for your bot.
