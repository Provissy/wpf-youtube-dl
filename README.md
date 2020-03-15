# wpf-youtube-dl
Simple youtube-dl WPF GUI tool

Simple means it is extremely simple. Do not expect it to do complicated jobs.

Default download quality is best VP9 + best OPUS.

## Usage
You must have `youtube-dl.exe` and `avconv` being placed within the same folder of this app.

Paste youtube link and click Check Quality or Start Download. 

You should check available quality first before downloading AV1 coded videos, since AV1 support on youtube is not completed yet.

## Dependencies
.net Core 3.1

System.Management.Automation 7.0

Microsoft.PowerShell.SDK 7.0

## Build
VS 2019

Open the solution, install required NuGet packges then build.

## License
The GPL v3 License.

![image](http://www.gnu.org/graphics/gplv3-127x51.png)
