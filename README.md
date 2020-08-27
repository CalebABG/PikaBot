# Synopsis
- This project is all about making a fun and wonky Discord bot :D
- This is also the first Discord bot I've made so bear with me through its evolution!


## Getting Started
1. The main thing you'll need to get started with this project is having a Discord developer account
    - If you don't have one, head over here: [Discord Developer Portal](https://discord.com/developers) and either sign in with your existing Discord account, or create an account, and then login to Discord's Developer Portal.
2. Once you've got your Developer account, head over to this link and read the Heading paragraph titled ```Creating the discord application``` - [Creating a Discord Bot w/ C#](https://dev.to/bizzycola/creating-a-discord-bot-with-c-net-core-and-dsharpplus-1obg). That article will hopefully help you get started with setting up the bot/application on the Developer Portal side!
3. Then, once you've got your new application in the Dev portal, click on your new application, then click on the 'Bot' menu item. From there you'll want to keep handy your Bot ```Token``` 
4. Now, onto the Meat and Potatoes of setting up the C# project!
    1. The application uses dotnet's Secret Manager to store sensitive information that the application can consume at runtime.
    2. First, open a Command Prompt or Powershell window (on Windows), or Terminal (for Linux or Mac), then change into the projects main folder ```PikaBotDiscord/PikaBot```
    3. Then, with your token copied, type the following command and then press enter: (replace the ```<token>``` with the token you copied)
    ```
   dotnet user-secrets set "discord:token" "<token>"
   ```
   4. Then boom! You're all set with securely storing your Discord token for use in the application! Here's the docs for [Microsoft's Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows#remove-a-single-secret) for reference.
5. The last thing to do is to make sure you've added your Bot to a discord channel as well as giving the Bot any necessary permissions it may need. You can refer to the [Discord Docs for making an app](https://discord.com/developers/docs/intro) or Google for how to set those up :)
6. Have fun!!

## Project Dependencies
1. [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
    - This awesome dependency is a C# library which wraps the Discord API and makes it incredibly easy to make Bots! Huge shout out to the creators and everyone who has contributed to that project! 