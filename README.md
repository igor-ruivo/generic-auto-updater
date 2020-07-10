<a href="https://github.com/igor-ruivo/generic-auto-updater"><img src="https://i.imgur.com/UdteXJN.png" title="Generic Auto-Updater Main Window" alt="Generic Auto-Updater Main Window"></a>
# Generic Auto-Updater

A robust, user-friendly, clean and efficient Auto-Updater for any client.

### The Idea

After finishing a stable beta version of the "Auto-Updater" to be used by a specific company, I decided to duplicate the repository and making a few tweaks in the copy to turn it public (and generic) in order for it to be used by anyone. And thus, the first version of the "Generic Auto-Updater" was born! - A robust, user-friendly, clean and efficient Auto-Updater for anyone who wishes to maintain their client's users always updated based on some server directory.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

```
Git;
Visual Studio 2019 (administrator priviledges) with Windows Forms App (.NET Framework);
.NET Framework 4.7.2.
```

### Installing

1. Clone this repository

```
git clone https://github.com/igor-ruivo/generic-auto-updater.git
```

2. Read the documentation!

For instance, keep in mind that the engine is expecting a server metadata file with a specific structure, like so:

```
http://your-directory-with-files/
filename1
hash1
filename2
hash2
filenameN
hashN
```
Meaning that the first line should be the actual URL to the server directory containing the files, and for the following lines, every even line number contains a relative path from the URL at line 1 to a file and every odd line number contains the respective file's md5 hash. You can change this implementation adapting it to your own server metadata, or do the opposite.

3. Configure the Auto-Updater at your will

```
Edit any strings, configurations and the icon in ./GenericAutoUpdater/Resources/**/*
Edit the logo-image that is embedded in .GenericAutoUpdater/UI/Screens/PatcherMainWindow.resx
```

4. Build it with Visual Studio

```
Ctrl+Shift+B
```

5. Run the debugger or execute the executable file

```
./GenericAutoUpdater/bin/Debug/Generic Auto-Updater.exe
```

The Auto-Updater will start.

## Running the tests

* Automated tests for this system will be added in the future.
* [VirusTotal analysis](https://www.virustotal.com/gui/file/ba65abac1f556990235cc8421c89b50fc53d08492be84c3d07885e0b73027cba/detection)

## Built With

* [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) - The framework used;
* [Visual Studio 2019](https://visualstudio.microsoft.com/) - IDE;
* [Icon](https://icon-icons.com/pt/download/106672/ICO/128/);
* [Image](https://www.nicepng.com/png/detail/246-2467547_your-logo-here-your-logo-here-logo-png.png).

## Contributing

You can contribute by submitting a pull-request.

## Versioning

For the versions available, see the [tags on this repository](https://github.com/igor-ruivo/generic-auto-updater/tags). 

## Authors

* **Igor Ruivo** - *Full stack* - [Igor Ruivo](https://www.linkedin.com/in/igor-ruivo/)

See also the list of [contributors](https://github.com/igor-ruivo/generic-auto-updater/contributors) who participated in this project.
