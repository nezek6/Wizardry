using System;

namespace WizardryClient
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WizardryGameClient game = new WizardryGameClient())
            {
                game.Run();
            }
        }
    }
#endif
}

