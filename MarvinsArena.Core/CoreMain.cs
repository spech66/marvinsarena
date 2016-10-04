using System;

namespace MarvinsArena.Core
{
	public class CoreMain
	{
		public static readonly CoreMain Instance = new CoreMain();

		public Tournament Tourney { get; set; }
		public bool Timeout { get; set; }
		public bool Debug { get; set; }
		
		private CoreMain()
		{
			Timeout = true;
		}

		public void ParseMainArgs(string[] args)
		{
			if(args.Length > 0)
			{
				if(args[0].Contains(".xtml"))
				{
					if(System.IO.File.Exists(args[0]))
					{
						Tourney = Tournament.ReadFromXml(args[0]);
					} else
					{
						throw new MarvinsArenaException(String.Format("Tournament file {0} not found", args[0]));
					}
				} else if(args[0].Contains(".dll"))
				{
					Tourney = new Tournament();
					Tourney.Rules.Mode = TournamentMode.LastManStanding;
					for(int i = 0; i < 2; i++)
					{
						Tourney.RobotManager.AddRobot(System.IO.Path.GetFileNameWithoutExtension(args[0]),
														Tourney.Rules.Teamsize,
														Tourney.Map.Map);
					}
					Tourney.Bracket = new TournamentBracket(Tourney.RobotManager.TeamCount, true);
				} else
				{
					throw new MarvinsArenaException(String.Format("Unknown file {0}", args[0]));
				}
			}
			#if DEBUG // TEST ONLY!
			else
			{
				//Tournament = new Tournament();
				//string basedir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				//Tourney = Tournament.ReadFromXml(System.IO.Path.Combine(basedir, "Tournament.xtml"));
				//Tourney = Tournament.ReadFromXml(System.IO.Path.Combine(basedir, "Tournament1on1.xtml"));		
				Tourney = Tournament.ReadFromXml(System.IO.Path.Combine("Tournaments", "1.xtml"));
			}
			#endif

			if(Tourney == null)
				throw new MarvinsArenaException("Tournament could not be loaded");
			if(Tourney.Map == null)
				throw new MarvinsArenaException("Tournament map could not be loaded");
			if(Tourney.Bracket.Root.Count == 1 && Tourney.Bracket.FirstRoot.NextBattle() == null)
				throw new MarvinsArenaException("There is no more battle!");

			// Parse additional arguments
			for(int i = 1; i < args.Length; i++)
			{
				if(args[i] == "-notimeout") Timeout = false;
				if(args[i] == "-debug") Debug = true;
			}
		}
	}
}
