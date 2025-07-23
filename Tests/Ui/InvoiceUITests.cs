using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace Facturon.Tests.Ui
{
    [TestClass]
    public class InvoiceUITests
    {
        private const string WinAppDriverUrl = "http://127.0.0.1:4723";
        private WindowsDriver<WindowsElement>? _session;

        [TestInitialize]
        public void Setup()
        {
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", "Facturon.App.exe");
            _session = new WindowsDriver<WindowsElement>(new Uri(WinAppDriverUrl), options);
        }

        [TestCleanup]
        public void TearDown()
        {
            _session?.Quit();
        }

        [TestMethod]
        public void TogglePricesIncludeVat_RecalculatesTotals()
        {
            Assert.IsNotNull(_session);
            var numberBox = _session.FindElementByAccessibilityId("InvoiceNumberBox");
            var issuerBox = _session.FindElementByAccessibilityId("IssuerBox");
            var netTotalText = _session.FindElementByAccessibilityId("NetTotalText");

            var initialNumber = numberBox.Text;
            var initialIssuer = issuerBox.Text;
            var initialNet = netTotalText.Text;

            var vatCheck = _session.FindElementByAccessibilityId("IsGrossBasedCheckBox");
            vatCheck.Click();

            var newNet = _session.FindElementByAccessibilityId("NetTotalText").Text;

            Assert.AreEqual(initialNumber, numberBox.Text);
            Assert.AreEqual(initialIssuer, issuerBox.Text);
            Assert.AreNotEqual(initialNet, newNet);
        }

        [TestMethod]
        public void BeginEdit_OnF2_DoesNotCrash()
        {
            Assert.IsNotNull(_session);
            var grid = _session.FindElementByAccessibilityId("InvoiceItems");
            grid.SendKeys(OpenQA.Selenium.Keys.F2);
        }
    }
}
