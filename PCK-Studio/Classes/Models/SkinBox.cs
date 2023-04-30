using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PckStudio.Classes.Models
{
	public class SkinBox
	{
		public string Type;
		public Vector3 Pos;
		public Vector3 Size;
		public float U, V;
		public bool HideWithArmor;
		public bool Mirror;
		public float Scale;
		public SkinBox(string input)
		{
			try
			{
				string[] arguments = Regex.Split(input, @"\s+"); // split by whitespace

				int old_length = arguments.Length - 1;

				Array.Resize<string>(ref arguments, 12);

				for (int x = 11; x > old_length; x--)
				{
					arguments[x] = "0"; // set any missing arguments to '0'
				}

				Type = arguments[0].ToUpper(); // just in case a box has all lower, the editor still parses correctly

				Pos = new Vector3(float.Parse(arguments[1]), float.Parse(arguments[2]), float.Parse(arguments[3]));
				Size = new Vector3(float.Parse(arguments[4]), float.Parse(arguments[5]), float.Parse(arguments[6]));
				U = float.Parse(arguments[7]);
				V = float.Parse(arguments[8]);
				HideWithArmor = Convert.ToBoolean(int.Parse(arguments[9]));
				Mirror = Convert.ToBoolean(int.Parse(arguments[10]));
				Scale = float.Parse(arguments[11]);
			}
			catch (FormatException fex)
			{
				MessageBox.Show($"A Format Exception was thrown\nFailed to parse BOX value\n{fex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			catch (IndexOutOfRangeException iex) // this should be MUCH more rare now
			{
				MessageBox.Show($"A box paramater was out of range\nFailed to parse BOX value\n{iex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			catch (Exception ex)
			{
				Type = string.Empty;
			}
		}

		public ValueTuple<string, string> ToProperty()
		{
			string value = $"{Type} {Pos.X} {Pos.Y} {Pos.Z} {Size.X} {Size.Y} {Size.Z} {U} {V} {Convert.ToInt32(HideWithArmor)} {Convert.ToInt32(Mirror)} {Scale}";
			return new ValueTuple<string, string>("BOX", value.Replace(',', '.'));
		}
	}
}
