using System;

namespace GuitarProSharp 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string fileName = "D:\\Users\\Michael\\Downloads\\Muse - Plug In Baby.gp3";

            
            Song? song = GP3File.LoadFromFile(fileName);
            if(song != null)
            {
                Console.WriteLine("Song parsed:");
                Console.WriteLine(song.ToString());

                // Show Song Visualiser form
                //ShowEditor(song);
            }
            else
            {
                Console.WriteLine("Failed to parse GP3 file!");
            }
        }
    }
}