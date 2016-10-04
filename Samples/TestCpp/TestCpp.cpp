#include "stdafx.h"

#include "TestCpp.h"

namespace TestCpp
{
	void TestCpp::Initialize()
	{
		ScannedRobot += gcnew EventHandler<ScannedRobotEventArgs^>(this, &TestCpp::OnScannedRobot);
		HitWall += gcnew EventHandler<EventArgs^>(this, &TestCpp::OnHitWall);
		HitRobot += gcnew EventHandler<EventArgs^>(this, &TestCpp::OnHitRobot);
	}

	void TestCpp::Run()
	{
		if(RemainingRotation == 0)
		{
			MoveForward(32);
		}
	}

	void TestCpp::OnScannedRobot(Object^ o, ScannedRobotEventArgs^ e)
	{
		FireMissile();
	}

	void TestCpp::OnHitWall(Object^ o, EventArgs^ e)
	{
		WalkWalls();
	}

	void TestCpp::OnHitRobot(Object^ o, EventArgs^ e)
	{
		RotateRightDeg(90);
	}

	void TestCpp::WalkWalls()
	{
		double rot = 90 - (int)RotationDeg % 90;
		if(rot > 90)
		{
			rot = 90;
		}

		RotateRightDeg(90);
		RotateGunRightDeg(RotationDeg - RotationGunDeg + 180);
		RotateRadarRightDeg(RotationDeg - RotationRadarDeg + 180);
	}
}
