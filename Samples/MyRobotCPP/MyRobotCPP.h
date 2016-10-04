// MyRobotCPP.h

#pragma once

using namespace System;
using namespace MarvinsArena::Robot;

namespace MyRobot {

	/// <summary>
	/// My robot
	/// </summary>
	public ref class MyRobotCPP : BaseRobot, IRobot
	{
	private:
		// This is the place to define class variables

	public:
		virtual void Initialize();
		virtual void Run();
	};

}
