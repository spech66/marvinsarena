using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class RobotTeam
	{

		public int RobotCount { get { return Robots.Count; } }
		[XmlAttribute("Team")]
		public int Team { get; set; }
		[XmlElement("Robots", Type = typeof(Collection<RobotLoader>))]
		public Collection<RobotLoader> Robots { get; set; }

		public RobotTeam()
		{
			Robots = new Collection<RobotLoader>();
		}

		public RobotTeam(int team)
		{
			this.Team = team;
			Robots = new Collection<RobotLoader>();
		}

		public void AddRobot(string name, int teamsize, byte[][] map)
		{
			int robotCount = RobotCount;
			for(int i = 1; i <= teamsize; i++)
			{
				RobotLoader loader = new RobotLoader();
				loader.Load(name, robotCount + i, Team, map);
				Robots.Add(loader);
			}
		}

		public void LoadRobotsAfterDeserialization(byte[][] map)
		{
			int squadNumber = 1;
			foreach(RobotLoader loader in Robots)
			{
				loader.Load(loader.AssemblyName, squadNumber, Team, map);
				squadNumber++;
			}
		}
	}
}
