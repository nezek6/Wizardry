namespace WizardryServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WizardryGameServer game = new WizardryGameServer())
            {
                game.Run();
            }
        }
    }
}

