using GR_Calcul.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Collections.Generic;
using System.Web.Security;

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


        ///// <summary>
        /////A test for DeleteRoom
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        //[UrlToTest("http://localhost:49893/")]
        //public void DeleteRoomTest()
        //{
        //    RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
        //    int id = 0; // TODO: Initialize to an appropriate value
        //    target.DeleteRoom(id);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        ///// <summary>
        /////A test for GetRoom
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        //[UrlToTest("http://localhost:49893/")]
        //public void GetRoomTest()
        //{
        //    RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
        //    int id = 0; // TODO: Initialize to an appropriate value
        //    Room expected = null; // TODO: Initialize to an appropriate value
        //    Room actual;
        //    actual = target.GetRoom(id);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for ListRooms
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        //[UrlToTest("http://localhost:49893/")]
        //public void ListRoomsTest()
        //{
        //    RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
        //    List<Room> expected = null; // TODO: Initialize to an appropriate value
        //    List<Room> actual;
        //    actual = target.ListRooms();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}

        ///// <summary>
        /////A test for CreateRoom
        /////</summary>
        //// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        //// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        //// whether you are testing a page, web service, or a WCF service.
        //[TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        //[UrlToTest("http://localhost:49893/")]
        //public void CreateRoomTest()
        //{
        //    RoomModel target = new RoomModel(); // TODO: Initialize to an appropriate value
        //    Room room = null; // TODO: Initialize to an appropriate value
        //    target.CreateRoom(room);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for CreateRoom
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        //[AspNetDevelopmentServerHost("C:\\Users\\Tipsee\\Documents\\Visual Studio 2010\\Projects\\GR-Calcul\\GR-Calcul", "/")]
        //[UrlToTest("http://localhost:49893/")]
        //[HostType("ASP.NET")]
        //[TestMethod()]
        public void TestGeneral()
        {
            RoomModel target = new RoomModel();
            List<Room> rooms = RoomModel.ListRooms();
            int k = rooms.Count;
            List<Int32> initialIds = new List<int>();
            foreach(var r in rooms)
                initialIds.Add(r.ID);

            RoomModel.CreateRoom(new Room());
            RoomModel.CreateRoom(new Room());

            int k2 = RoomModel.ListRooms().Count;
            Assert.AreEqual(k + 2, k2);

            List<Room> rooms2 = RoomModel.ListRooms();
            List<Int32> newIds = new List<int>();
            foreach (var r in rooms2)
                newIds.Add(r.ID);

            newIds.RemoveAll(delegate(int i){ return initialIds.Contains(i); });

            foreach (int i in newIds)
                RoomModel.DeleteRoom(i, RoomModel.GetRoom(i));
            int p = RoomModel.ListRooms().Count;
            Assert.AreEqual(k, p);
        }

        [TestMethod()]
        public void TestRooms()
        {
            List<Int32> list1 = new List<int>();
            list1.Add(1);
            list1.Add(2);
            list1.Add(3);
            list1.Add(4);
            int k = list1.Count;

            List<Int32> list2 = new List<int>(list1);

            list2.Add(5);
            list2.Add(6);

            int k2 = list2.Count;
            Assert.AreEqual(k + 2, k2);

            list2.RemoveAll(delegate(int i) { return list1.Contains(i); });
            int p = list2.Count;
            Assert.AreEqual(2, p);

        }

    }
}
