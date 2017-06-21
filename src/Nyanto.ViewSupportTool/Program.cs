using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Nyanto.ViewSupportTool
{
	class Program
	{
		static Dictionary<string,string> replaceName = new Dictionary<string, string>()
		{
			{"android.support.constraint.ConstraintLayout", "android.support.constraints.ConstraintLayout" }
		};
		static void Main(string[] args)
		{
			
			var targetProjectPath = args[0];
			var rootNamespace = args[1];
			var resourcePath = Path.Combine(targetProjectPath, "Resources");
			var layoutPath = Path.Combine(resourcePath, "layout");
			var outputFilePath = Path.Combine(resourcePath, "Layout.Designer.cs");
			var files = Directory.GetFiles(layoutPath);
			var sb = new StringBuilder();
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using System.Linq;");
			sb.AppendLine("using System.Text;");
			sb.AppendLine("using Android.App;");
			sb.AppendLine("using Android.Content;");
			sb.AppendLine("using Android.OS;");
			sb.AppendLine("using Android.Runtime;");
			sb.AppendLine("using Android.Views;");
			sb.AppendLine("using Android.Widget;");
			sb.AppendLine();
			sb.AppendLine($"namespace {rootNamespace}");
			sb.AppendLine("{");
			foreach (var file in files)
			{
				CreateClass(sb, file, rootNamespace);
			}
			sb.AppendLine("}");

			Console.WriteLine(sb);
			var writer = File.CreateText(outputFilePath);
			writer.Write(sb.ToString());
			
			writer.Flush();
			writer.Dispose();

			Console.Read();
		}

		static void CreateClass(StringBuilder sb, string filePath, string nameSpaceName)
		{
			var classname = Path.GetFileNameWithoutExtension(filePath) + "_holder";
			var xml = File.ReadAllText(filePath);
			var xdoc = XDocument.Parse(xml);
			var reader = xdoc.CreateReader();
			var controls = new List<Control>();
			while (reader.Read())
			{
				var control = new Control();
				var basename = reader.Name;
				if (replaceName.ContainsKey(basename))
				{
					basename = replaceName[basename];
				}
				var fileName = string.Join(".", basename.Split('.').Select(x => FirstLetterToUpper(x)));

				control.Name = fileName;

				if (reader.MoveToAttribute("android:id"))
				{
					control.Id = reader.Value.Split('/')[1];
					controls.Add(control);
				}
			}

			sb.Append("\t");
			sb.AppendLine($"public class {classname} : IDisposable");
			sb.Append("\t");
			sb.AppendLine("{");

			foreach (var control in controls)
			{
				sb.Append("\t");
				sb.Append("\t");
				sb.Append($"public {control.Name} {control.Id} ");
				sb.Append("{ get; }");
				sb.AppendLine();
			}
			sb.AppendLine();

			sb.Append("\t");
			sb.Append("\t");
			sb.Append($"public {classname}(View view)");
			sb.AppendLine();

			sb.Append("\t");
			sb.AppendLine("\t{");

			foreach (var control in controls)
			{
				sb.Append("\t");
				sb.Append("\t");
				sb.Append("\t");
				sb.Append($"{control.Id} = view.FindViewById<{control.Name}>({nameSpaceName}.Resource.Id.{control.Id});");
				sb.AppendLine();
			}

			sb.Append("\t");
			sb.AppendLine("\t}");

			sb.Append("\t");
			sb.AppendLine($"\tpublic void Dispose()");
			sb.Append("\t");
			sb.AppendLine("\t{");

			foreach (var control in controls)
			{
				sb.Append("\t");
				sb.Append("\t");
				sb.Append("\t");
				sb.Append($"{control.Id}.Dispose();");
				sb.AppendLine();
			}
			sb.Append("\t");
			sb.AppendLine("\t}");

			sb.Append("\t");
			sb.AppendLine("}");
			sb.AppendLine();


		}

		public static string FirstLetterToUpper(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}

		private class Control
		{
			public string Name { get; set; }
			public string Id { get; set; }
		}
	}
}