using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphInputConfigGenerator
{
	enum InputParameterType
	{
		No_Vertices,
		No_Edges,
		S_NS,
		W_UW,
		Max_Wt,
		Dir_UnDir,
		Degree,
		Op_file_name,
		Op_file_name_2,
		Op_file_name_3,
		None
	}


	class Program
	{
		static List<List<InputParameterType>> inputParameterTypesList = new List<List<InputParameterType>>()
		{
			new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.S_NS,        InputParameterType.W_UW,           InputParameterType.Op_file_name },
			new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.S_NS,        InputParameterType.W_UW,           InputParameterType.Op_file_name },
			new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.W_UW,        InputParameterType.Op_file_name,   InputParameterType.None,        },
			//new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.W_UW,        InputParameterType.Op_file_name,   InputParameterType.Op_file_name_2},
			//new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.Max_Wt,   InputParameterType.Dir_UnDir,   InputParameterType.Op_file_name,   InputParameterType.None      },
			//new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.Degree,   InputParameterType.Op_file_name,InputParameterType.Op_file_name_2, InputParameterType.Op_file_name_3},
			//new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.Dir_UnDir,   InputParameterType.Op_file_name,   InputParameterType.Op_file_name_2},
			//new List<InputParameterType>() { InputParameterType.No_Vertices, InputParameterType.No_Edges, InputParameterType.Max_Wt,      InputParameterType.Op_file_name,   InputParameterType.None          },
		};


		static void Main(string[] args)
		{
			//GenerateFinalFiles();
			//return;
			string seperator = Environment.NewLine;
			int fileCount = 0;
			int index1 = 1;
			foreach (var inputParamList in inputParameterTypesList)
			{
				if (index1 == 1 || index1 == 2)
				{
					index1++;

					continue;
				}
				List<int> range1 = GetRange(inputParamList[0], 0);
				foreach (int index2 in range1)
				{
					List<int> range2 = GetRange(inputParamList[1], inputParamList[1] == InputParameterType.No_Edges ? index2 : (inputParamList[1] == InputParameterType.Degree ? index2 : 0));

					foreach (int index3 in range2)
					{
						if (inputParamList[2] == InputParameterType.Op_file_name)
						{
							fileCount++;
							Console.WriteLine(index1 + seperator + index2 + seperator + index3 + seperator + index1 + "_" + index2 + "_" + index3 + ".input");
						}
						else
						{
							List<int> range3 = GetRange(inputParamList[2], 0);

							for (int index4 = range3[0]; index4 <= range3.Last(); index4++)
							{

								if (inputParamList[3] == InputParameterType.Op_file_name)
								{
									fileCount++;
									Console.WriteLine(index1 + seperator + index2 + seperator + index3 + seperator + index4 + seperator + index1 + "_" + index2 + "_" + index3 + "_" + index4 + ".input");
								}
								else
								{
									List<int> range4 = GetRange(inputParamList[3], 0);

									for (int index5 = range4[0]; index5 <= range4.Last(); index5++)
									{
										if (inputParamList[4] == InputParameterType.Op_file_name)
										{
											fileCount++;
											Console.WriteLine(index1 + seperator + index2 + seperator + index3 + seperator + index4 + seperator + index5 + seperator + index1 + "_" + index2 + "_" + index3 + "_" + index4 + "_" + index5 + ".input");
										}
									}
								}
							}
						}
					}
				}
				index1++;
			}

			Console.WriteLine(0);
			//Console.WriteLine(fileCount);
			Console.ReadLine();
		}

		private static void Compare()
		{
			//string[] inputparams = File.ReadAllLines(@"C:\Users\deepi\Documents\Visual Studio 2015\Projects\GraphInputConfigGenerator\GraphInputConfigGenerator\bin\Debug\op.txt").Where(s => s.Contains(".input")).ToArray();
			//string[] opparams = File.ReadAllLines(@"C:\Users\deepi\Desktop\dir.txt").ToArray();

			//List<String> diff = inputparams.Where(input => !opparams.Contains(input)).ToList();  // - inputparams.Intersect(opparams).ToList();


		}

		static void GenerateFinalFiles()
		{
			string OutGreedy = "OutGraph3Greedy_WT_nsquare_new";
			string OutRR = "OutGraph3RR_WT_nsquare_new";
			string opFileGreedy = @"C:\Users\deepi\Documents\Visual Studio 2015\Projects\GraphInputConfigGenerator\GraphInputConfigGenerator\bin\Debug\Out\" + OutGreedy;
			string opFileRR = @"C:\Users\deepi\Documents\Visual Studio 2015\Projects\GraphInputConfigGenerator\GraphInputConfigGenerator\bin\Debug\Out\" + OutRR;

			string[] inputFilesGreedy = Directory.GetFiles(opFileGreedy);
			string[] inputFilesRR = Directory.GetFiles(opFileRR);
			string finalOp = String.Empty;
			string error = String.Empty;

			string separator = ",";
			string newLine = Environment.NewLine;
			foreach (var fileName_E in inputFilesGreedy)
			{
				foreach (var fileName2_E in inputFilesRR)
				{
					string fileName = fileName_E.Split('\\').Last();
					string fileName2 = fileName2_E.Split('\\').Last();

					if (fileName.Equals(fileName2))
					{

						string[] fileLines = File.ReadAllLines("Out/"+ OutGreedy + "/" + fileName);
						string[] fileLines2 = File.ReadAllLines("Out/" + OutRR + "/" + fileName2);

						if (fileLines.Length > 0 && fileLines2.Length > 0 )
						{
							string[] contents = fileName.Split('_');
							string[] contents2 = fileName2.Split('_');
							if (!(contents[0].Equals("5") || contents[0].Equals("6")))
							{
								try
								{
									string graphType = contents[0];
									string vertex = contents[1];
									string edge = contents[2];
									string vertex_cover_size = fileLines[0];
									string independent_set_size = fileLines[1];
									string independent_set_weight_greedy = fileLines[2];
									string vertex_cover_weight_greedy = fileLines[3];
									string independent_set_weight_RR = fileLines2[2];
									string vertex_cover_weight_RR = fileLines2[3];

									finalOp += graphType+ separator + vertex + separator + edge + separator + vertex_cover_size + separator + independent_set_size + separator + vertex_cover_weight_greedy 
												+ separator + independent_set_weight_greedy + separator + vertex_cover_weight_RR + separator + independent_set_weight_RR + newLine;
								}
								catch (Exception)
								{
									Console.WriteLine("Error:" + fileName);
									error += fileName + newLine;
								}
							}
						}
						else
						{
							//Console.WriteLine("hahahah");
						}

					}

				}
			}
			File.WriteAllText("data_graph3_NSquare_new.csv", finalOp);
			File.WriteAllText("error", error);


		}

		static List<int> GetRange(InputParameterType inputParam, int param)
		{
			List<int> range = new List<int>();
			switch (inputParam)
			{
				case InputParameterType.No_Vertices:
					range = new List<int>() { 10, 50, 100, 200, 350, 500, 750, 1000 };
					break;
				case InputParameterType.No_Edges:
					range = GetEdges(param);
					break;
				case InputParameterType.S_NS:
					range = Enumerable.Range(1, 2).ToList();
					break;
				case InputParameterType.W_UW:
					range = Enumerable.Range(1, 1).ToList();
					break;
				case InputParameterType.Max_Wt:
					range = Enumerable.Range(1, 1).ToList();
					break;
				case InputParameterType.Dir_UnDir:
					range = Enumerable.Range(2, 1).ToList();
					break;
				case InputParameterType.Degree:
					range = Enumerable.Range(2, param - 1 - 2).ToList();
					break;

				case InputParameterType.Op_file_name:
				case InputParameterType.Op_file_name_2:
				case InputParameterType.Op_file_name_3:
				case InputParameterType.None:

					throw new Exception("Not supported");

				default:
					break;
			}

			if (range.Count == 0)
			{
				throw new Exception("Not supported");
			}

			return range;
		}

		static List<int> GetEdges(int vertices)
		{
			List<int> edges = new List<int>();
			int reqEdgesCount = 100;
			int totalEdgesCount = ((vertices * (vertices - 1)) / 2);
			int currentEdgeCount = vertices - 1;

			int step = totalEdgesCount > reqEdgesCount ? totalEdgesCount / reqEdgesCount : 1;
			reqEdgesCount = Math.Min(totalEdgesCount -vertices + 2, reqEdgesCount);

			while (reqEdgesCount > 0)
			{
				edges.Add(currentEdgeCount);
				currentEdgeCount += step;
				--reqEdgesCount;
			}



			return edges;

		}
	}
}
