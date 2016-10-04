using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class RobotManager
	{

		public int TeamCount { get { return Teams.Count; } }
		[XmlElement("Teams", Type = typeof(Collection<RobotTeam>))]
		public Collection<RobotTeam> Teams { get; set; }

		public RobotManager()
		{
			Teams = new Collection<RobotTeam>();
		}

		/// <summary>
		/// Add a new robot team
		/// </summary>
		/// <param name="name">Assembly file name</param>
		/// <param name="teamsize">Robots per team</param>
		public void AddRobot(string name, int teamsize, byte[][] map)
		{
			RobotTeam team = new RobotTeam(Teams.Count + 1); // TeamID == Bracket current id
			team.AddRobot(name, teamsize, map);
			Teams.Add(team);
		}

		/// <summary>
		/// Generates GameObjects from tourney
		/// </summary>
		/// <param name="tournament"></param>
		/// <returns></returns>
		public Collection<GameObjectRobot> CreateGameObjects(Tournament tournament)
		{
			Collection<GameObjectRobot> objectList = new Collection<GameObjectRobot>();

			if(tournament.Rules.Mode == TournamentMode.LastManStanding ||
				tournament.Rules.Mode == TournamentMode.TeamLastTeamStanding)
			{
				foreach(Bracket bracket in tournament.Bracket.Root)
				{
					foreach(RobotLoader robot in Teams[bracket.Current - 1].Robots)
					{
						GameObjectRobot tank = new GameObjectRobot(robot);
						objectList.Add(tank);
					}
				}
			} else
			{
				Bracket b = tournament.Bracket.FirstRoot.NextBattle();
				if(b.Left != null)
				{
					/*GameObjectRobot robot = new GameObjectRobot(Teams[b.Left.Current - 1].Robots[0]);
					objectList.Add(robot);*/
					foreach(RobotLoader robot in Teams[b.Left.Current - 1].Robots)
					{
						GameObjectRobot tank = new GameObjectRobot(robot);
						objectList.Add(tank);
					}
				}
				if(b.Right != null)
				{
					/*GameObjectRobot robot = new GameObjectRobot(Teams[b.Right.Current - 1].Robots[0]);
					objectList.Add(robot);*/
					foreach(RobotLoader robot in Teams[b.Right.Current - 1].Robots)
					{
						GameObjectRobot tank = new GameObjectRobot(robot);
						objectList.Add(tank);
					}
				}
			}

			return objectList;
		}

		public void LoadTeamsAfterDeserialization( byte[][] map)
		{
			foreach (RobotTeam team in Teams)
			{
				team.LoadRobotsAfterDeserialization(map);
			}
		}
	}
}
