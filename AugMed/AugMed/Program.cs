using System;

namespace AugMed
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (AnnotationsModule game = new AnnotationsModule())
            {
                game.Run();
            }
        }
    }
}

