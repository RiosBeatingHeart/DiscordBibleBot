
<h1 align="center">Discord Bible Bot</h1>

<h4 align="center">🙏Bot Of The Divine🙏</h4>

<p align="center">
  <a href="#overview">Overview</a>
  •
  <a href="#installation">Installation</a>
  •
  <a href="#reactions">Reactions</a>
  •
  <a href="#prayers">Prayers</a>
  •
  <a href="#setup">Setup</a>
  •
  <a href="#license">License</a>
</p>

# Overview

The Discord Bible Bot was created by our Lord to server the sole purpose of judging the infidel and bringing Gods wrath over them.
It does this by replying "None Of These Words Are In The Bible!" when none of the words in a sentence are in the Bible.
The Bible Bot was written in the divine language of <code>C#</code> using <code>.Net Core</code> and <code><a href="https://dsharpplus.github.io/">DSharpPlus</a></code>.
It uses the "Catholic Public Domain Bible" hosted on <a href="https://openbible.com/">Open Bible</a>.


# Installation

To invite God into your heart and add the Bible Bot to a discord server, just click this <a href="https://discord.com/api/oauth2/authorize?client_id=959594363820847144&permissions=52288&redirect_uri=https%3A%2F%2Fdiscordapp.com%2Foauth2%2Fauthorize%3F%26client_id%3D959594363820847144%26scope%3Dbot&scope=bot">LINK</a>.
Once the Bible Bot has joined your server, you can add it to channels by typing the following prayer <code>🙏join</code>.

# Reactions

The Bible Bot will react when you react to messages with certain emojis.

- <code>🙏</code>
  - <strong>User Message:</strong> The Bible Bot will make a words check.
  - <strong>Bible Bot Message:</strong> You become a christian.

# Prayers

- <code>🙏help</code> <strong>Pray for Gods help!</strong> Lists all Prayers.

- <code>🙏follow</code> <strong>Become a devout follower of God!</strong> The Bible Bot will check all your messages from now on.

- <code>🙏unfollow</code> <strong>Turn your back on your Lord and Saviour!</strong> Bible Bot will only check your messages when you are in a blessed channel.

- <code>🙏join [#channel]</code> <strong>Embrace God's love in a channel!</strong> Blesses a channel. The Bible bot will check all messages in it from now on.

- <code>🙏leave [#channel]</code> <strong>Ban God from a channel!</strong> Desecrates a channel.

- <code>🙏cum</code> <strong>Pray, pray, pray!</strong> "Come" appears quite often in the Bible.

# Setup

I have no idea how you can setup the project for yourself. Just download the source code and open it with some IDE.
You will definitely need to setup an <code>app.config</code> file that contains
your Godly Bot's token, the url of a Bible in text form and
the path to the SQLite database.

It should about look like this: 

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="BotToken" value="your bot's token" />
        <add key="BiblePath" value="https://openbible.com/textfiles/cpdv.txt" />
        <add key="DatabasePath" value="database\BotDatabase.sqlite" />
    </appSettings>
</configuration>
```

# License

I have no idea how this shit works. If you just copy the whole project, you got to credit me and can't make money with it.
If you substantially build on it, you can do whatever you want. It would be nice if you credited me tho.
