using System;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class RobotLoader
	{
		private PolicyLevel levelOfAccess;
		private PermissionSet executeOnly;
		[NonSerialized]
		private AppDomain appDomain;

		[XmlIgnore]
		public RobotHost RobotHost { get; private set; }
		[XmlAttribute("Name")]
		public string AssemblyName { get; set; }

		public RobotLoader()
		{
			SetSecurity();
			CreateDomain();
		}

		private void SetSecurity()
		{
			levelOfAccess = PolicyLevel.CreateAppDomainLevel();
			executeOnly = new PermissionSet(PermissionState.None);
			executeOnly.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			//executeOnly.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, "C:\\Windows\\"));
			PolicyStatement executeOnlyStatement = new PolicyStatement(executeOnly);
			UnionCodeGroup applicablePolicy = new UnionCodeGroup(new AllMembershipCondition(), executeOnlyStatement);
			levelOfAccess.RootCodeGroup = applicablePolicy;
		}

		private void CreateDomain()
		{
			AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
			appDomainSetup.ApplicationBase = "Robots";
			appDomain = AppDomain.CreateDomain("Sandbox", null, appDomainSetup, executeOnly);
		}

		/// <summary>
		/// Load the robot
		/// </summary>
		/// <param name="name">The name of the robot (not the fileName and path).</param>
		public void Load(string name, int squadNumber, int team, byte[][] map)
		{
			// Fix wrong calls although this should not be valid as name
			name = System.IO.Path.GetFileNameWithoutExtension(name);
			Console.WriteLine(appDomain.SetupInformation.ApplicationBase);
			if (!System.IO.File.Exists(System.IO.Path.Combine(appDomain.SetupInformation.ApplicationBase, String.Format("{0}.dll", name))))
			{
				throw new MarvinsArenaException(String.Format("Robot not found!\n{0}", name));
			}

			AssemblyName = name;

			RobotHost robotHost = (RobotHost)Activator.CreateInstanceFrom(appDomain, Assembly.GetExecutingAssembly().Location, typeof(RobotHost).FullName).Unwrap();
			robotHost.LoadAssembly(name, squadNumber, team, map);			
			this.RobotHost = robotHost;
		}

		public void Unload()
		{
			RobotHost = null;
			AppDomain.Unload(appDomain);
		}
	}
}
