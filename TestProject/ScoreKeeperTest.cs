using BattleEngineCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for ScoreKeeperTest and is intended
    ///to contain all ScoreKeeperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ScoreKeeperTest
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
		///A test for RemoveScore
		///</summary>
		[TestMethod()]
		public void RemoveScoreTest1()
		{
			Scorekeeper target = new Scorekeeper();
			string name = "hugo";
			int points = 10;
			target.AddScore(name, points);
			target.RemoveScore(name, points / 2);
			target.RemoveScore(name, points / 2);
			int actual = target.GetScore(name);
			Assert.AreEqual(0, actual);
		}

		/// <summary>
		///A test for RemoveScore
		///</summary>
		[TestMethod()]
		public void RemoveScoreTest()
		{
			Scorekeeper target = new Scorekeeper();
			string name = "hugo";
			target.AddScore(name);
			target.RemoveScore(name);
			int actual = target.GetScore(name);
			Assert.AreEqual(0, actual);
		}

		/// <summary>
		///A test for AddScore
		///</summary>
		[TestMethod()]
		public void AddScoreTest1()
		{
			Scorekeeper target = new Scorekeeper();
			string name = "hugo";
			target.AddScore(name);
			int actual = target.GetScore(name);
			Assert.AreEqual(1, actual);
		}

		/// <summary>
		///A test for AddScore
		///</summary>
		[TestMethod()]
		public void AddScoreTest()
		{
			Scorekeeper target = new Scorekeeper();
			string name = "hugo";
			int points = 10;
			target.AddScore(name, points);
			int actual = target.GetScore(name);
			Assert.AreEqual(points, actual);
		}

		/// <summary>
		///A test for GetScore
		///</summary>
		[TestMethod()]
		public void GetScoreTest()
		{
			Scorekeeper target = new Scorekeeper();
			string name = "hugo";
			int expected = -1;
			int actual = target.GetScore(name);
			Assert.AreEqual(expected, actual);
		}
	}
}
