using GuitarProSharp;

namespace GuitarSongEditor
{
    internal static class Program
    {
        const string fileName = "D:\\Users\\Michael\\Downloads\\Muse - Plug In Baby.gp3";
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            SongExplorer app = new SongExplorer(fileName);

            Application.Run(app);
        }
    }
}