using System.Globalization;
using System.Threading;

namespace Solcery.Utils
{
    public static class CultureUtils
    {
        public static void SetCurrentCulture(Thread currentThread, string cultureName = "en-US")
        {
            var ci = new CultureInfo(cultureName);
            currentThread.CurrentCulture = ci;
            currentThread.CurrentUICulture = ci;
        }
    }
}