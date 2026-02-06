using System.Diagnostics;

const string url = "https://youtu.be/dQw4w9WgXcQ";

for (int i = 0; i < 25; i++)
{
    Process.Start(new ProcessStartInfo
    {
        FileName = url,
        UseShellExecute = true
    });
}
