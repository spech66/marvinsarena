using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class Bracket
	{

		[XmlAttribute("Deepth")]
		public int Deepth { get; set; }
		[XmlAttribute("Round")]
		public int Round { get; set; }
		[XmlAttribute("Current")]
		public int Current { get; set; }
		[XmlIgnore]
		public Bracket Parent { get; set; }
		[XmlElement("Left", Type = typeof(Bracket))]
		public Bracket Left { get; set; }
		[XmlElement("Right", Type = typeof(Bracket))]
		public Bracket Right { get; set; }

		public int BuildTree(int deepth, int min, int max, Bracket parent)
		{
			Deepth = deepth;
			Parent = parent;

			int diff = max - min;
			int middle = min + Convert.ToInt32(Math.Ceiling(diff / 2.0));

			int deepth1 = deepth, deepth2 = deepth;
			if(middle > min && middle < max)
			{
				Left = new Bracket();
				deepth1 = Left.BuildTree(deepth + 1, min, middle, this);
			}
			if(middle < max)
			{
				Right = new Bracket();
				deepth2 = Right.BuildTree(deepth + 1, middle, max, this);
			}

			if(Left == null && Right == null)
				Current = middle;

			if(deepth1 > deepth2)
			{
				Round = deepth1 - deepth + 1; // Starting with 1
				return deepth1;
			} else
			{
				Round = deepth2 - deepth + 1; // Starting with 1
				return deepth2;
			}
		}

		public void GetBrackets(int round, ref List<Bracket> list)
		{
			if(this.Round == round)
			{
				list.Add(this);
				return;
			}

			if(Left != null)
				Left.GetBrackets(round, ref list);
			if(Right != null)
				Right.GetBrackets(round, ref list);
		}

		public Bracket NextBattle()
		{
			Bracket nextBracket;

			if(Left != null)
			{
				if((nextBracket = Left.NextBattle()) != null)
					return nextBracket;
			}
			if(Right != null)
			{
				if((nextBracket = Right.NextBattle()) != null)
					return nextBracket;
			}

			if(Current == 0)
			{
				return this;
			} else
			{
				return null;
			}
		}
		
		/// <summary>
		/// Call after deserialization to get parent which leads to circular references.
		/// </summary>
		public void RebuildParentTree(Bracket parent)
		{
			this.Parent = parent;

			if (Left != null)
				Left.RebuildParentTree(this);
			if (Right != null)
				Right.RebuildParentTree(this);
		}
	}

	[Serializable]
	public class TournamentBracket
	{
		private int maxDepth;

		[XmlAttribute("Teams")]
		public int Teams { get; set; }
		[XmlAttribute("Rounds")]
		public int Rounds { get { return maxDepth + 1; /*Starting with 1*/ } set { maxDepth = value - 1; } }
		/// <summary>
		/// Root stores tree in first element for full tourney tree or list for Last*Standing
		/// </summary>
		[XmlElement("Root", Type = typeof(Collection<Bracket>))]
		public Collection<Bracket> Root { get; set; }
		/// <summary>
		/// Returns only the first root element in the list
		/// </summary>
		public Bracket FirstRoot { get { return Root[0]; } }

		/// <summary>
		/// For Serialization only!
		/// </summary>
		protected TournamentBracket()
		{
		}

		public TournamentBracket(int teams)
			: this(teams, false)
		{
		}

		public TournamentBracket(int teams, bool flat)
		{
			this.Teams = teams;
			Root = new Collection<Bracket>();

			if(flat)
			{
				maxDepth = 0;
				for(int i = 1; i < teams + 1; i++)
				{
					Bracket rootBracket = new Bracket();
					rootBracket.BuildTree(0, i, i, null);
					Root.Add(rootBracket);
				}
			} else
			{
				Bracket rootBracket = new Bracket();
				maxDepth = rootBracket.BuildTree(0, 0, teams, null);
				Root.Add(rootBracket);
			}
		}

		public IEnumerable<Bracket> GetBrackets(int round)
		{
			if (Root == null || Root.Count == 0)
				return null;

			List<Bracket> brackets = new List<Bracket>();
			Root[0].GetBrackets(round, ref brackets);
			return brackets.AsEnumerable();
		}

		public void SetWinner(int winner)
		{
			if (FirstRoot == null)
				return;

			Bracket b = FirstRoot.NextBattle();
			if (b != null)
				b.Current = winner;
		}

		/// <summary>
		/// Call after deserialization to get parent which leads to circular references.
		/// </summary>
		public void RebuildParentTree()
		{
			if(Root == null || Root.Count == 0)
				return;

			foreach(Bracket bracket in Root)
				bracket.RebuildParentTree(null);
		}
	}
}
