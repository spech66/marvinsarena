#pragma once

using namespace System;
using namespace MarvinsArena::Robot;

namespace TestCpp {

	public ref class TestCpp : BaseRobot, IRobot
	{
	private:
		void WalkWalls();

	public:
		virtual void Initialize();
		virtual void Run();

		void OnScannedRobot(Object^ o, ScannedRobotEventArgs^ e);
		void OnHitWall(Object^ o, EventArgs^ e);
		void OnHitRobot(Object^ o, EventArgs^ e);
	};
}
