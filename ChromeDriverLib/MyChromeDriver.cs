using SeleniumUndetectedChromeDriver;

namespace ChromeDriverLib
{
    public class MyChromeDriver
    {
        public UndetectedChromeDriver Driver { get; set; } = null!;

        public string ProfileDir { get; set; } = null!;

        public bool IsDeleteProfile { get; set; } = false;

        public void Close()
        {
            ChromeDriverInstance.Close(this).Wait();
        }
    }
}
