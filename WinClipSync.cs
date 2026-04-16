using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace ClipSync
{
    class Program
    {
        static string path = @"E:\.clipboard\clip.dat";
        static string last = "";

        [STAThread]
        static void Main()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Hide();

            new Thread(() =>
            {
                while (true)
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            string t = File.ReadAllText(path);
                            if (!string.IsNullOrEmpty(t))
                                Clipboard.SetText(t);
                        }
                        catch { }
                    }
                    Thread.Sleep(800);
                }
            }).Start();

            while (true)
            {
                try
                {
                    string t = Clipboard.GetText();
                    if (t != last && !string.IsNullOrEmpty(t))
                    {
                        last = t;
                        File.WriteAllText(path, t);
                    }
                }
                catch { }
                Thread.Sleep(300);
            }
        }

        static void Hide()
        {
            try
            {
                System.Diagnostics.Process.Start(
                    "attrib", "+s +h \"" + Path.GetDirectoryName(path) + "\"");
            }
            catch { }
        }
    }
}
