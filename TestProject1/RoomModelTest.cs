using GR_Calcul.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for RoomModelTest and is intended
    ///to contain all RoomModelTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RoomModelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DeleteRoom
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        [UrlToTest("http://localhost:49893/")]
        public void DeleteRoomTest()
        {
            RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            target.DeleteRoom(id);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetRoom
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        [UrlToTest("http://localhost:49893/")]
        public void GetRoomTest()
        {
            RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
            int id = 0; // TODO: Initialize to an appropriate value
            Room expected = null; // TODO: Initialize to an appropriate value
            Room actual;
            actual = target.GetRoom(id);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ListRooms
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        [UrlToTest("http://localhost:49893/")]
        public void ListRoomsTest()
        {
            RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
            List<Room> expected = null; // TODO: Initialize to an appropriate value
            List<Room> actual;
            actual = target.ListRooms();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateRoom
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        [UrlToTest("http://localhost:49893/")]
        public void CreateRoomTest()
        {
            RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
            Room room = null; // TODO: Initialize to an appropriate value
            target.CreateRoom(room);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
