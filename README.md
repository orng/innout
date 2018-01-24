# Innout
In-N-Out - simple time tracking on Windows

Innout runs as a Windows service and keeps track of when you lock/unlock your computer.

# Disclaimer
This is currently a quick hack and not stable, properly tested software, use on your own risk.

## Motivation
Let's say you work flexible hours so it varies when you arrive at work and when you leave from work. You need to keep track of when you arrive and when you leave.
Innout runs as a Windows service and registers the first login or screen unlock of the day as well as the last logout/screen lock so that you can easily see when you arrived at your computer and the last time you left.

### Design goals
Innout aims to be simple, both in implementation and in usage. Everything is stored in textfiles which are human readable and easily edited.

## Usage
Compile.

Install the Innout windows service by opening the VS Developer Command prompt and running 

```
installutil.exe Innout.exe.
```

Start the Innout service.

Once the service is running Innout will automatically write the times when you lock/unlock your computer to a file with a name in the form of `{month}{year}.txt` e.g. `jan18.txt` for January 2018. For information on how to configure where this file is written see the 'Configuration' below.

### Configuration
By default the times will be written to a file in the same location as the executable. To change the location of the outputted files edit Innout.exe.config and set the FileDirectory property to wherever you want the files written.
```
  <applicationSettings>
    <Innout.Properties.Settings>
      <setting name="FileDirectory" serializeAs="String">
        <value>
          C:\Users\orn.gudjonsson\programs\times
        </value>
      </setting>
    </Innout.Properties.Settings>
  </applicationSettings>
```

## Limitations
Innout does not understand pauses, so if you leave during the day this will not be registered, likewise if you bring your computer home from work and open it during the evening.
Innout can not discern intent, if you log on to your computer to play games, Innout will not know this.
Things probably don't work great if you work past midnight.
