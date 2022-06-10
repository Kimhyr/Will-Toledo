# [Penile.NET](https://github.com/kimhyr/Penile.NET)

A general purpose Discord bot using [Discord.Net](https://discordnet.dev/index.html).

## Warning

I just wanted to put some text here because it is visually pleasing when there is a smaller text above some larger text that then lists items. Also, it is because I have a compulsive urge to do so.

#### [Penile.NET](https://github.com/kimhyr/Penile.NET) is:
1. Severely unfinished.
2. Created by a very lazy, immature, 15-year-old, pubescent, teen who is fairly new to programming.
3. Personally tailored. There may be some features (*e.g. the `rules` command*) that are directly tailored towards my personal preference.

## Features

- [x] Getting user information
- [x] Manipulating users
- [x] Embed creation
- [x] Getting guild information
- [ ] Manipulating and creating guilds
- [ ] Component creation
- [ ] Scam detection
- [ ] Event logger
- [ ] Message filterer
- [ ] Audio play

## Requirements

Again, I just wanted to put some text here because it is visually pleasing when there is a smaller text above some larger text that then lists items. Also, it is because I have a compulsive urge to do so.

#### Nuget Packages

- [Discord.Net](https://www.nuget.org/packages/Discord.Net/) (3.7.1)
- [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/) (6.0.2-mauipre.1.22102.15)
- [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json/) (6.0.2-mauipre.1.22102.15)
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) (6.0.2-mauipre.1.22102.15)

#### Other
- Some sort of cognition.

## Installation

1. Create a Discord bot and make sure all intents are enabled.
2. Clone the repository.
3. In `Source/config.json`, change `TEST_GUILD_ID` to a test server's ID and change `TOKEN` to your Discord bot's token.
4. You might need to copy `config.json` to `bin/Debug/net6.0`, if you get this error:
```
Unhandled exception. System.IO.FileNotFoundException: The configuration file 'config.json' was not found and is not optional. The expected physical path was 'D:\config.json'.
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.HandleException(ExceptionDispatchInfo info)
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load(Boolean reload)
   at Microsoft.Extensions.Configuration.FileConfigurationProvider.Load()
   at Microsoft.Extensions.Configuration.ConfigurationRoot..ctor(IList`1 providers)
   at Microsoft.Extensions.Configuration.ConfigurationBuilder.Build()
   at PenileNET.Program..ctor() in ~\PenileNET\Source\Program.cs:line 16
   at PenileNET.Program.Main(String[] args) in ~\PenileNET\Source\Program.cs:line 24
   at PenileNET.Program.<Main>(String[] args)
```

## Note

Contact me on Discord (i.e. [@Emhyr#0560](https://discord.com/channels/@me/982086180449431553), [@dogman#0113](https://discord.com/channels/@me/982086180449431553)) or Twitter (i.e. [@kingemhyr](https://twitter.com/kingemhyr)) for additional information.

Also, I greatly appreciate constructive criticism .