# NSETIC
Novaschem SATS export to iCalendar converter

### Prerequisites

You will either need Mono (www.mono-project.com) or Visual Studio C# to compile this project.

### Build

Clone the repository and enter the directory

```
git clone git@github.com:villy/NSETIC.git
cd NSETIC
```

And build on Linux

```
xbuild NSETIC.sln
```

or on Windows

```
msbuild NSETIC.sln
```

### Usage

Currently NSETIC does not take any commandline arguments and expects a DATA.TXT(Export file from Novaschem)
to be in the same directory and will output iCalendar files for all the teachers.

These can be for example imported into Google Calendar or Outlook.

TODO: commandline arguments.
