using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

struct Edge
{
	public int v1;
	public int v2;
}

namespace BudgetHubAlgoRunner
{
	public enum BaseHubWeightFunctionType
	{
		Simple,
		Square,
		Log
	}

	enum AssociatedNodeWeightFunctionType
	{
		Linear,
		Square
	}



	class Program
	{
		static int BaseWeightForNode = 10;
		//int[,] buckets = new int[4, 1];
		static SortedDictionary<int, int> buckets = new SortedDictionary<int, int>() { { 20, 50 }, { 40, 100 }, { 60, 200 }, { 100, 500 } };

		static int BaseWeightForAdditiveNode = 10;

		static void Main(string[] args)
		{
			string vcisFilesDirectory = "OutVCIS";
			string actualInputFilesDirectory = "Data";
			var directory = new DirectoryInfo(vcisFilesDirectory);
			DateTime cur = DateTime.Now;
			//Console.WriteLine(DateTime.Now);

			//IEnumerable<string> vcisFiles = Directory.EnumerateFiles(vcisFilesDirectory);
			//int wrongInfo = 0;

			List<Tuple<int, int>> weightFunctions = new List<Tuple<int, int>>() {
				new Tuple<int, int>((int)BaseHubWeightFunctionType.Simple, (int)AssociatedNodeWeightFunctionType.Square),
				new Tuple<int, int>((int)BaseHubWeightFunctionType.Log, (int)AssociatedNodeWeightFunctionType.Square),
				new Tuple<int, int>((int)BaseHubWeightFunctionType.Square, (int)AssociatedNodeWeightFunctionType.Square),
				new Tuple<int, int>((int)BaseHubWeightFunctionType.Simple, (int)AssociatedNodeWeightFunctionType.Linear),
			};
			string headers = "WeightFunctionType,Nodes,Edges,GR VC,RR VC,GR IS,RR IS,RR+";

			Console.WriteLine(headers);
			string fileOutPath = "Graphs/" + Guid.NewGuid() + "/";

			Directory.CreateDirectory(fileOutPath);
			int count = 0;


			foreach (var vcisFilePath in directory.GetFiles().OrderBy(fi => fi.FullName).Select(ffi => ffi.FullName))
			{
				Console.WriteLine(vcisFilePath);
				//if (!vcisFilePath.Contains("_200_5572_"))
				//    continue;
				//Console.ReadLine();
				int v_num = 0;
				string actualInputFilePath_1 = vcisFilePath.Split('\\').Last();
				string a2 = actualInputFilePath_1.Remove(actualInputFilePath_1.Count() - 10);
				List<Tuple<int, int>> edges = GetGraphFromFile(actualInputFilesDirectory + "/" + a2, out v_num);

				var vcisFileLines = File.ReadAllLines(vcisFilePath);
				List<int> iss = Regex.Replace(vcisFileLines[0].Trim(), @"\s+", " ").Split(' ').Select(x => int.Parse(x)).ToList();
				List<int> vcs = Regex.Replace(vcisFileLines[1].Trim(), @"\s+", " ").Split(' ').Select(x => int.Parse(x)).ToList();

				//if(iss.Count > vcs.Count)
				//{
				//	Console.WriteLine(++wrongInfo);
				//}

				//Console.WriteLine("Graph: " + (++counter));
				//Console.WriteLine("fileName:" + a2 + ", VC Size: " + vcs.Count + ", IS Size:" + iss.Count);
				for (int weightFuncIndex = 0; weightFuncIndex < weightFunctions.Count; weightFuncIndex++)
				{
					int actualWeightRR_IS = 0;
					int actualWeightGreedy_IS = 0;
					int actualWeightRR_VC = 0;
					int actualWeightGreedy_VC = 0;



					int newMinsWeightRRPlus = CalculateMinWeightForSingleGraph(vcs, iss, edges, v_num, weightFunctions[weightFuncIndex], out actualWeightRR_IS, out actualWeightGreedy_IS, out actualWeightGreedy_VC, out actualWeightRR_VC);
					string[] filenametokens = actualInputFilePath_1.Split('_');
					string fileName = weightFuncIndex + "_" + v_num + ".csv";
					string content = "\n" + weightFuncIndex + "," + v_num + "," + filenametokens[2] + "," + actualWeightGreedy_VC + "," + actualWeightRR_VC + "," + actualWeightGreedy_IS + "," + actualWeightRR_IS + "," + newMinsWeightRRPlus;

					string fileFullPath = fileOutPath + fileName;
					if (!File.Exists(fileFullPath))
					{
						//File.Create(fileFullPath);
						File.AppendAllText(fileFullPath, headers);
					}

					File.AppendAllText(fileFullPath, content);
				}
			}
			Console.WriteLine("S:" + cur + "Now:" + DateTime.Now);
			Console.WriteLine("Done!");
			Console.ReadLine();
		}

		private static List<Tuple<int, int>> GetGraphFromFile(string filePath, out int vcount)
		{
			List<Tuple<int, int>> edges = new List<Tuple<int, int>>();
			String[] lines = File.ReadAllLines(filePath);
			vcount = 0;

			foreach (var line in lines)
			{
				string[] tokens = Regex.Replace(line.Trim(), @"\s+", " ").Split(' ');

				if (tokens[0] == "p")
				{
					vcount = int.Parse(tokens[2]);
				}
				if (tokens[0] == "e")
				{
					edges.Add(new Tuple<int, int>(int.Parse(tokens[1]), int.Parse(tokens[2])));
				}
			}
			return edges;
		}


		private static int CalculateMinWeightForSingleGraph(List<int> vcs, List<int> iss, List<Tuple<int, int>> input, int v_num, Tuple<int, int> weightFunctionTypes, out int actualWeightRR_IS, out int actualWeightGreedy_IS, out int actualWeightGreedy_VC, out int actualWeightRR_VC)
		{
			vcs.Sort();
			iss.Sort();

			Dictionary<int, int> degreeMap = new Dictionary<int, int>();

			Dictionary<int, List<int>> degreeListVC = new Dictionary<int, List<int>>();
			Dictionary<int, List<int>> degreeListIS = new Dictionary<int, List<int>>();

			Dictionary<int, List<int>> degreeListGlobal = new Dictionary<int, List<int>>();

			foreach (var vc in vcs)
			{
				degreeListVC[vc] = new List<int>();
			}

			foreach (var isS in iss)
			{
				degreeListIS[isS] = new List<int>();
			}

			foreach (var node in Enumerable.Range(1, v_num))
			{
				degreeListGlobal[node] = new List<int>();
			}

			for (int i = 1; i <= v_num; i++)
			{
				degreeMap[i] = 0;
			}

			int currentMinWeight = int.MaxValue;

			Edge[] edge = new Edge[input.Count];

			for (int i = 0; i < edge.Length; i++)
			{
				edge[i].v1 = input[i].Item1;
				edge[i].v2 = input[i].Item2;

				degreeMap[edge[i].v1]++;
				degreeMap[edge[i].v2]++;

				if (vcs.BinarySearch(edge[i].v1) >= 0 && vcs.BinarySearch(edge[i].v2) < 0)
				{
					degreeListVC[edge[i].v1].Add(edge[i].v2);
				}
				else if (vcs.BinarySearch(edge[i].v2) >= 0 && vcs.BinarySearch(edge[i].v1) < 0)
				{
					degreeListVC[edge[i].v2].Add(edge[i].v1);
				}

				if (iss.BinarySearch(edge[i].v1) >= 0 && iss.BinarySearch(edge[i].v2) < 0)
				{
					degreeListIS[edge[i].v1].Add(edge[i].v2);
				}
				else if (iss.BinarySearch(edge[i].v2) >= 0 && iss.BinarySearch(edge[i].v1) < 0)
				{
					degreeListIS[edge[i].v2].Add(edge[i].v1);
				}

				degreeListGlobal[edge[i].v1].Add(edge[i].v2);
				degreeListGlobal[edge[i].v2].Add(edge[i].v1);
			}

			//Console.WriteLine("G VC " + DateTime.Now);
			int givenMinWeightGreedy_VC = CalculateWeight(AssigningVerticesGreedy(edge, vcs.ToList(), v_num, degreeListVC), weightFunctionTypes);
			//Console.WriteLine("RR VC " + DateTime.Now);
			int givenMinWeightRR_VC = CalculateWeight(AssigningVerticesRoundRobin(edge, vcs.ToList(), v_num, degreeListVC, null), weightFunctionTypes);

			//Console.WriteLine("G IS " + DateTime.Now);
			int givenMinWeightGreedy_IS = CalculateWeight(AssigningVerticesGreedy(edge, iss, v_num, degreeListIS), weightFunctionTypes);
			//Console.WriteLine("RR IS " + DateTime.Now);
			int givenMinWeightRR_IS = CalculateWeight(AssigningVerticesRoundRobin(edge, iss, v_num, degreeListIS, null), weightFunctionTypes);

			actualWeightRR_IS = givenMinWeightRR_IS;
			actualWeightGreedy_IS = givenMinWeightGreedy_IS;
			actualWeightGreedy_VC = givenMinWeightGreedy_VC;
			actualWeightRR_VC = givenMinWeightRR_VC;

			List<int> currentList = new List<int>();

			if (vcs.Count < iss.Count)
			{
				currentList = vcs.ToList();
				currentMinWeight = givenMinWeightRR_VC;
			}
			else
			{
				currentList = iss.ToList();
				currentMinWeight = givenMinWeightRR_IS;
			}

			foreach (var isSingle in currentList)
			{
				degreeMap.Remove(isSingle);
			}

			var sortedDegreeMapEnumerator = degreeMap.OrderByDescending(v => v.Value).GetEnumerator();

			while (sortedDegreeMapEnumerator.MoveNext())
			{
				Dictionary<int, List<int>> assoc = null;

				if (vcs.Count < iss.Count)
				{
					assoc = AssigningVerticesRoundRobin(edge, currentList, v_num, null, degreeListGlobal);
				}
				else
				{
					assoc = AssigningVerticesRoundRobin(edge, currentList, v_num, null, degreeListGlobal);
				}

				int weight = CalculateWeight(assoc, weightFunctionTypes);

				if (weight < currentMinWeight)
				{
					currentMinWeight = weight;
				}
				currentList.Add(sortedDegreeMapEnumerator.Current.Key);
			}
			//currentMinWeight = Math.Min(currentMinWeight, givenMinWeight);
			//Console.WriteLine("New Algo Minweight:" + currentMinWeight);
			//if(currentMinWeight != givenMinWeightRR)
			//{
			//	//Console.WriteLine("\t New Min Weight" + currentMinWeight + ". Old Weight:" + givenMinWeight);
			//}

			return currentMinWeight;
		}

		private static int CalculateWeight(Dictionary<int, List<int>> assoc, Tuple<int, int> weightFunctionTypes)
		{
			int weight = CalculateHubCost((BaseHubWeightFunctionType)weightFunctionTypes.Item1, assoc.Keys.Count);
			foreach (var singleMap in assoc)
			{
				weight += CalculateAssociatedWeights((AssociatedNodeWeightFunctionType)weightFunctionTypes.Item2, singleMap.Value.Count);// * singleMap.Value.Count; //(Enumerable.Range(1, singleMap.Value.Count).Sum() * BaseWeightForAdditiveNode) + singleMap.Value.Count;
			}

			return weight;
		}

		private static int CalculateHubCost(BaseHubWeightFunctionType bhwft, int numBaseHubs)
		{
			double baseWeight = 0;
			switch (bhwft)
			{
				case BaseHubWeightFunctionType.Simple:
					baseWeight = 100 * numBaseHubs;
					break;
				case BaseHubWeightFunctionType.Square:
					baseWeight = numBaseHubs * numBaseHubs;
					break;
				case BaseHubWeightFunctionType.Log:
					baseWeight = 100 * numBaseHubs * Math.Log(numBaseHubs);
					break;
				default:
					throw new Exception();
			}
			return (int)baseWeight;
		}

		private static int CalculateAssociatedWeights(AssociatedNodeWeightFunctionType anwft, int assocNum)
		{
			int weight = 0;
			switch (anwft)
			{
				case AssociatedNodeWeightFunctionType.Linear:
					weight = assocNum;
					break;
				case AssociatedNodeWeightFunctionType.Square:
					weight = assocNum * assocNum;
					break;
				default:
					throw new Exception();
			}

			return weight;
		}

		private static int CalculateBucketizedHubsCost(int numNodes)
		{
			int bWeight = 0;
			int currentNumNodes = numNodes;

			foreach (var upperLimit in buckets.Keys)
			{
				if (currentNumNodes >= upperLimit)
				{
					continue;
				}
				else
				{
					bWeight = buckets[upperLimit];
					break;
				}
			}

			return numNodes * bWeight;
		}

		private static Dictionary<int, List<int>> AssigningVerticesRoundRobin(Edge[] edges, List<int> eligible_nodes, int v_numP, Dictionary<int, List<int>> degreeList, Dictionary<int, List<int>> degreeList2)
		{
			Dictionary<int, List<int>> assoc = new Dictionary<int, List<int>>();
			int v_num = v_numP;
			int e_num = edges.Length;
			int lastVertexVisitedIndex = -1;

			bool[] used_v = new bool[v_num + 1];

			for (int i = 0; i < eligible_nodes.Count; i++)
			{
				assoc.Add(eligible_nodes[i], new List<int>());
			}

			int assignedVertices = (v_numP - assoc.Keys.Count);
			for (int i = 0; i < v_num; i++)
			{
				used_v[i] = false;
			}

			foreach (var key in assoc.Keys)
			{
				used_v[key] = true;
			}

			while (assignedVertices > 0)
			{
				int currentVertex = assoc.Keys.ElementAt((++lastVertexVisitedIndex) % assoc.Keys.Count);

				List<int> nodes = new List<int>();

				if (degreeList != null && degreeList.TryGetValue(currentVertex, out nodes))
				{
					foreach (var node in nodes)
					{
						if (!used_v[node])
						{
							assoc[currentVertex].Add(node);
							used_v[node] = true;
							assignedVertices--;
							break;
						}
					}
				}
				else
				{
					List<int> nodes2 = degreeList2[currentVertex];

					foreach (var node in nodes2)
					{
						if (!used_v[node])
						{
							assoc[currentVertex].Add(node);
							used_v[node] = true;
							assignedVertices--;
							break;
						}
					}

					//if (degreeList2 != null)
					//{
					//    List<int> nodes2 = degreeList2[currentVertex];

					//    foreach (var node in nodes2)
					//    {
					//        if (!used_v[node])
					//        {
					//            assoc[currentVertex].Add(node);
					//            used_v[node] = true;
					//            assignedVertices--;
					//            break;
					//        }
					//    }
					//}
				}






			}
			//foreach (var item in assoc)
			//{
			//	Console.WriteLine(item.Key + " {" + String.Join(",", item.Value) + "}");
			//}

			//Console.ReadLine();

			return assoc;
		}

		private static Dictionary<int, List<int>> AssigningVerticesGreedy(Edge[] edge, List<int> eligible_nodes, int v_numP, Dictionary<int, List<int>> degreeList)
		{

			Dictionary<int, List<int>> assoc = new Dictionary<int, List<int>>();
			int v_num = v_numP;
			int e_num = edge.Length;
			bool[] used_v = new bool[v_num + 1];
			int[] vertex_cover = new int[v_numP];

			vertex_cover[0] = 0;

			for (int i = 1; i < vertex_cover.Length; i++)
			{
				if (eligible_nodes.Contains(i))
				{
					vertex_cover[i] = 1;
				}
			}

			for (int i = 1; i < v_num; i++)
			{
				if (vertex_cover[i] != 0)
				{
					assoc.Add(i, new List<int>());
				}
			}

			for (int i = 1; i < v_num; i++)
			{
				if (vertex_cover[i] != 0)
				{
					//bool do_stuff = false;
					List<int> connectedNodes = degreeList[i];

					foreach (var node in connectedNodes)
					{
						if (!used_v[node])
						{
							assoc[i].Add(node);
							used_v[node] = true;
						}
					}

					//               for (int e = 0; e < e_num; e++)
					//{

					//	int temp_v = -1;

					//	if (edge[e].v1 == i)
					//	{
					//		do_stuff = true;
					//		temp_v = edge[e].v2;
					//	}
					//	else if (edge[e].v2 == i)
					//	{
					//		do_stuff = true;
					//		temp_v = edge[e].v1;
					//	}

					//	for (int j = 1; j < v_num; j++)
					//	{
					//		if ((temp_v == j) && vertex_cover[j] != 0)
					//		{
					//			do_stuff = false;
					//		}
					//	}

					//	if (do_stuff)
					//	{
					//		if (!used_v[temp_v - 1])
					//		{
					//			assoc[i].Add(temp_v);
					//			used_v[temp_v - 1] = true;
					//		}
					//	}
					//}
				}
			}

			//foreach (var item in assoc)
			//{
			//	Console.WriteLine(item.Key + " {" + String.Join(",", item.Value) + "}");
			//}
			//Console.ReadLine();

			return assoc;
		}

	}

	public class UnorderedTupleComparer<T> : IEqualityComparer<Tuple<T, T>>
	{
		private IEqualityComparer<T> comparer;
		public UnorderedTupleComparer(IEqualityComparer<T> comparer = null)
		{
			this.comparer = comparer ?? EqualityComparer<T>.Default;
		}

		public bool Equals(Tuple<T, T> x, Tuple<T, T> y)
		{
			return comparer.Equals(x.Item1, y.Item1) && comparer.Equals(x.Item2, y.Item2) ||
					comparer.Equals(x.Item1, y.Item2) && comparer.Equals(x.Item1, y.Item2);
		}

		public int GetHashCode(Tuple<T, T> obj)
		{
			return comparer.GetHashCode(obj.Item1) ^ comparer.GetHashCode(obj.Item2);
		}
	}
}
