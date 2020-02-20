using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Appium;

namespace UITests
{
    [TestClass]
    public class MainWindowTests
    {
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        private const string ApplicationPath = @"C:\Users\zkxst\source\repos\LenovoDemo\lenovodemo\lenovo_demo\UI\bin\Debug\UI.exe";

        protected static WindowsDriver<WindowsElement> desktopSession;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Launch a new instance of the tested application
            if (desktopSession == null)
            {
                // Create a new session to launch the tested application
                AppiumOptions options = new AppiumOptions();
                options.AddAdditionalCapability("app", ApplicationPath);
                desktopSession = new WindowsDriver<WindowsElement>(new Uri(WindowsApplicationDriverUrl), options);
                Assert.IsNotNull(desktopSession);
                Assert.IsNotNull(desktopSession.SessionId);
                // Set implicit timeout to 5 seconds
                //to make element search to retry every 500 ms
                //for at most three times
                desktopSession.Manage().Timeouts().ImplicitWait =
                    TimeSpan.FromSeconds(1.5);
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            //desktopSession.FindElementByName("TestButton").Click();
            //System.Threading.Thread.Sleep(1000);
            //desktopSession.FindElementByName("TestButton").Click();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            // Close the application and delete the session
            if (desktopSession != null)
            {
                desktopSession.Close();
                desktopSession.Quit();
                desktopSession = null;
            }
        }
    }
    public static class Helper
    {
        public static WindowsElement FindElementByAbsoluteXPath(
            this WindowsDriver<WindowsElement> desktopSession,
            string xPath,
            int nTryCount = 3)
        {
            WindowsElement uiTarget = null;
            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementByXPath(xPath);
                }
                catch
                {
                }
                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(400);
                }
            }
            return uiTarget;
        }
    }

}