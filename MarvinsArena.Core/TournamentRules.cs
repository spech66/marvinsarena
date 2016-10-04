using System;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	public enum TournamentMode
	{
		LastManStanding,
		OneOnOne,
		TeamLastTeamStanding,
		TeamTwoOnTwo
	}

	[Serializable]
	public class TournamentRules
	{
		public const int TeamsizeMin = 1, TeamsizeMax = 4, TeamsizeDefault = 2;
		public const int RoundsMin = 1, RoundsMax = 9, RoundsDefault = 3;
		public const int HitpointsMin = 10, HitpointsMax = 1000, HitpointsDefault = 100;
		public const int MissilesMin = 0, MissilesMax = 100, MissilesDefault = 10;
		public const int MaxHeatMin = 1, MaxHeatMax = 100, MaxHeatDefault = 20;

		private TournamentMode mode;
		private int teamsize;
		private int rounds;
		private int hitpoints;
		private int missiles;
		private int maxHeat;
		
		public TournamentRules()
		{
			mode = TournamentMode.LastManStanding;
			teamsize = TeamsizeDefault;
			rounds = RoundsDefault;
			hitpoints = HitpointsDefault;
			missiles = MissilesDefault;
			maxHeat = MaxHeatDefault;
		}

		[XmlAttribute("Mode")]
		public TournamentMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				
				if(mode > TournamentMode.TeamTwoOnTwo || mode < 0)
					mode = 0;

				ResetTeamsize();
			}
		}

		[XmlAttribute("Teamsize")]
		public int Teamsize
		{
			get { return teamsize; }
			set
			{
				teamsize = value;
				if (teamsize > TeamsizeMax || teamsize < TeamsizeMin)
					teamsize = TeamsizeMin;

				ResetTeamsize();
			}
		}

		private void ResetTeamsize()
		{
			if(mode == TournamentMode.TeamTwoOnTwo)
				teamsize = 2;

			if(mode == TournamentMode.LastManStanding ||
				mode == TournamentMode.OneOnOne)
			{
				teamsize = 1;
			}
		}

		[XmlAttribute("Rounds")]
		public int Rounds
		{
			get { return rounds; }
			set
			{
				rounds = value;
				if (rounds > RoundsMax || rounds < RoundsMin)
					rounds = RoundsMin;
			}
		}

		[XmlAttribute("Hitpoints")]
		public int Hitpoints
		{
			get { return hitpoints; }		
			set
			{
				hitpoints = value;
				if (hitpoints > HitpointsMax || hitpoints < HitpointsMin)
					hitpoints = HitpointsMin;
			}
		}

		[XmlAttribute("Missiles")]
		public int Missiles
		{
			get { return missiles; }
			set
			{
				missiles = value;
				if (missiles > MissilesMax || missiles < MissilesMin)
					missiles = MissilesMin;
			}
		}

		[XmlAttribute("MaxHeat")]
		public int MaxHeat
		{
			get { return maxHeat; }
			set
			{
				maxHeat = value;
				if (maxHeat > MaxHeatMax || maxHeat < MaxHeatMin)
					maxHeat = MaxHeatMin;
			}
		}
	}
}