using System;
using System.IO;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class Tournament
	{
		[XmlIgnore]
		public string LoadedFile { get; set; }

		[XmlElement("Bracket", Type = typeof(TournamentBracket))]
		public TournamentBracket Bracket { get; set; }

		[XmlElement("Rules", Type = typeof(TournamentRules))]
		public TournamentRules Rules { get; set; }

		[XmlElement("Map", Type = typeof(TournamentMap))]
		public TournamentMap Map { get; set; }

		[XmlElement("RobotManager", Type = typeof(RobotManager))]
		public RobotManager RobotManager { get; set; }

		public static Tournament ReadFromXml(string fileName)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(Tournament));
			Tournament tournament;

			using(TextReader textReader = new StreamReader(fileName))
			{
				tournament = (Tournament)deserializer.Deserialize(textReader);
				textReader.Close();
			}

			tournament.Bracket.RebuildParentTree();
			tournament.RobotManager.LoadTeamsAfterDeserialization(
										tournament.Map.Map);

			tournament.LoadedFile = fileName;

			return tournament;
		}

		public void SaveToXml()
		{
			if(LoadedFile != String.Empty)
				SaveToXml(LoadedFile);
		}

		public void SaveToXml(string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(Tournament));
			using (TextWriter textWriter = new StreamWriter(fileName))
			{
				serializer.Serialize(textWriter, this);
				textWriter.Close();
			}
			
			//loadedFile = fileName;
		}

		public Tournament()
		{
			RobotManager = new RobotManager();
			Bracket = new TournamentBracket(0);
			Rules = new TournamentRules();
			Map = new TournamentMap();
		}
	}
}
