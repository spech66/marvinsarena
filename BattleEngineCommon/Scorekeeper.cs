using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleEngineCommon
{
	/// <summary>
	/// Keep scores per name. Score will never be negative.
	/// </summary>
	public class Scorekeeper
	{
		private Dictionary<string, int> scores = new Dictionary<string, int>();
		
		/// <summary>
		/// Adds points to name.
		/// </summary>
		/// <param name="name">The Robot name</param>
		/// <param name="points">The amount of points</param>
		public void AddScore(string name, int points)
		{
			if (scores.ContainsKey(name))
			{
				scores[name] += points;

				if (scores[name] < 0)
					scores[name] = 0;
			}
			else
			{
				if (points < 0)
					points = 0;

				scores.Add(name, points);
			}
		}

		/// <summary>
		/// Adds 1 point to name.
		/// </summary>
		/// <param name="name">The Robot name</param>
		public void AddScore(string name)
		{
			AddScore(name, 1);
		}

		/// <summary>
		/// Substracts 1 point from name. If score would go below 0 it will be set to 0.
		/// </summary>
		/// <param name="name">The Robot name</param>
		public void RemoveScore(string name)
		{
			AddScore(name, -1);
		}

		/// <summary>
		/// Substracts points from name. If score would go below 0 it will be set to 0.
		/// </summary>
		/// <param name="name">The Robot name</param>
		public void RemoveScore(string name, int points)
		{
			AddScore(name, -points);
		}

		/// <summary>
		/// Returns the score for named robot.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>The score or -1 if not found!</returns>
		public int GetScore(string name)
		{
			if (scores.ContainsKey(name))
			{
				return scores[name];
			}

			return -1;
		}

		/// <summary>
		/// Get the highest score
		/// </summary>
		/// <returns></returns>
		public int GetHighscore()
		{
			return scores.Values.Max();
		}

		/// <summary>
		/// Get List of best robots (> 1 if equal scores)
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetHighscoreName()
		{
			var query = from s in scores
						where s.Value == GetHighscore()
						select s.Key;
			return query.AsEnumerable();
		}

		/// <summary>
		/// Return all names and scores ordered by scores.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, int>> GetAllScores()
		{
			var query = from s in scores
						orderby s.Value descending
						select s;
			return query.AsEnumerable();
		}
	}
}
