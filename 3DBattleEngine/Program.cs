#region Using Statements
using System;
using MarvinsArena.Core;
#endregion

[assembly: CLSCompliant(false)]
namespace BattleEngine3D
{
	public static class Program
	{
		public static Tournament Tournament;
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static int Main(string[] args)
		{
			try
			{
				CoreMain.Instance.ParseMainArgs(args);
				Tournament = CoreMain.Instance.Tourney;

				using(BattleEngine game = new BattleEngine())
				{
					game.Run();
				}
			} catch(Exception e)
			{
				using (System.IO.StreamWriter sr = new System.IO.StreamWriter("BattleEngine.log"))
				{
					sr.WriteLine(e.ToString());
					sr.Close();
				}
				return 1;
			}
			return 0;
		}
	}
}

