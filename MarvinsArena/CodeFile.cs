using System;
using System.IO;
using System.Xml.Serialization;

namespace MarvinsArena
{
	public enum Language
	{
		VisualBasic = 0,
		CSharp
	}

	[Serializable]
	public class CodeFile
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string Author { get; set; }
		public string Description { get; set; }
		public string Code { get; set; }

		[XmlElement("CodeLanguage", Type = typeof(MarvinsArena.Language))]
		public MarvinsArena.Language CodeLanguage { get; set; }

		public CodeFile()
		{
			CodeLanguage = MarvinsArena.Language.CSharp;
			SetDefault(CodeLanguage);
		}

		public CodeFile(MarvinsArena.Language newLanguage)
		{
			CodeLanguage = newLanguage;
			SetDefault(newLanguage);
		}

		private void SetDefault(MarvinsArena.Language newLanguage)
		{
			CodeLanguage = newLanguage;

			Name = "MyNewRobot";
			Version = "1.0.0.0";
			Author = "YourName " + DateTime.Now.Year;
			Description = "My new robot";

			if (newLanguage == Language.CSharp)
			{
				Code = "// C# Template\n";
				Code += "using System;\nusing MarvinsArena.Robot;\n\n";
				Code += "namespace MyNewRobot\n{\n\tpublic class MyNewRobot : BaseRobot, IRobot\n\t{\n";
				Code += "\t\tpublic void Initialize()\n\t\t{\n\t\t}\n\n";
				Code += "\t\tpublic void Run()\n\t\t{\n\t\t}\n";
				Code += "\t}\n}";
			}

			if (newLanguage == Language.VisualBasic)
			{
				Code = "' Visual Basic Template\n";
				Code += "Imports MarvinsArena.Robot\n\n";
				Code += "Public Class MyRobot\n\tInherits BaseRobot\n\tImplements IRobot\n\n";
				Code += "\tPublic Sub Initialize() Implements IRobot.Initialize\n\tEnd Sub\n\n";
				Code += "\tPublic Sub Run() Implements IRobot.Run\n\tEnd Sub\n";
				Code += "End Class";
			}
		}

		public static CodeFile ReadFromXml(string fileName)
		{
			XmlSerializer deserializer = new XmlSerializer(typeof(CodeFile));
			CodeFile codeFile;

			using (TextReader textReader = new StreamReader(fileName))
			{
				codeFile = (CodeFile)deserializer.Deserialize(textReader);
				textReader.Close();
			}

			return codeFile;
		}

		public void SaveToXml(string fileName)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(CodeFile));
			using (TextWriter textWriter = new StreamWriter(fileName))
			{
				serializer.Serialize(textWriter, this);
				textWriter.Close();
			}
		}
	}
}
