using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChat.Synca.API
{
    public static class WebDriverBind
    {
        public static void Initialize<TWebDriver>(DriverOptions options) where TWebDriver : IWebDriver
        {
            if (_currentWebDriver != null)
                throw new InvalidOperationException("Cannot initialize when a WebDriver is already initialized!");

            _currentWebDriver = (TWebDriver)Activator.CreateInstance(typeof(TWebDriver), new object[] { options })!;
        }

        public static IWebDriver Driver => _currentWebDriver;
        private static IWebDriver _currentWebDriver;
    }
}
