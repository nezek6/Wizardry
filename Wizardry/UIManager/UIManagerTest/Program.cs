using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIManagerTest
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (UIManagerTestGame game = new UIManagerTestGame())
                game.Run();
        }
    }
#endif
}
