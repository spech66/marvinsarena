using System;

namespace MarvinsArena.Robot
{
	/// <summary>
	/// The interface defines the required methods for a working robot
	/// </summary>
	public interface IRobot
	{
		/// <summary>
		/// The initialize method is the point where a robot can initialize all variables and assign event handler.
		/// </summary>
		void Initialize();
		
		/// <summary>
		/// The run method is called serveral times per second. This is the place to write the robots logic.
		/// </summary>
		void Run();
	}
}
