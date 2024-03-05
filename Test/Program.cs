using ChromeDriverLib;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //proxy = http://host:port:user:pass
            MyChromeDriver myChromeDriver = null!;
            try
            {
                myChromeDriver = ChromeDriverInstance.GetInstance(0, 0, isHeadless: false, disableImg: false
                , proxy: null);
                myChromeDriver.Driver.GoToUrl("https://www.facebook.com");
            }
            finally
            {
                myChromeDriver?.Close();
            }
            ChromeDriverInstance.KillAllChromes();
        }
    }
}
