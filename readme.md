Budget Hub solution

About
-----

This is a cost-optimzation solution which, given a graph, provides the hub set that gives the lowest cost that connects the rest of the nodes in the graph.


This solution contains 5 components with their functionalities  mentioned below. Each of these components should be run in the same order to get the simulation results. Below are the instructions to execute each of the components:

1. Graph Configuration Generator:

This component generates different configurations that is required by the Graph Generator for creating multiple graph topologies. The following choices were made for the configurations:

A. Graph type : Connected
B. Vertex count (V) : ranging from 10 to 1000
C. Edge count (E) : ranging from V-1 to (V(V+1)/2)
D. Weighted? : No

Using the above choices, this component generates a single file that contains all possible combinations of graph input configurations. This file forms the input to the Graph Generator.


2. Graph Generator:

Using the output of the Graph Configuration Generator, the Graph Generator component generates a single graph for each configuration provided in the input file. We use this component to create all the input graphs and feed it to NuMVC to get the Vertex cover and Independent set.


3. NuMVC:

NuMVC is an external component that we use to obtain the minimum vertex cover and maximal independent set. 


4. Python script to run NuMVC for all graphs:

Each of the graphs that were generated from the Graph Generator component is fed into NuMVC sequentially via a python script and the corresponding output file with the minimum vertex cover and maximal independent set is placed in a output folder by the script.


5. Node Assignment & cost calculation component:

Once we have our hub set from NuMVC (vertex cover or independent set) , we perform the node assignment using this component. Here, we use three algorithms to assign nodes: Greedy, Round Robin and Round Robin Plus (RR+). After we assign the regular nodes to the hub set through the three algorithms, we calculate the overall cost of the graph network. To calculate this cost, we use different weight functions which have been modelled from existing cost price models.


6. Pyplot Generator
Once we have the corroborated results for all combinations of algorithms, hub sets and weight function, we plot the graphs for each combination.

Languages used: C++ for NuMVC, C Graph Generator, C# for Graph Configuration Generator and Node Assignment component, and Python for Graph plot generations.


Installation and Usage
----------------------

To build the binary, please make sure the following prerequisition are met:

1. Windows 10 OS for components 1,4,5,6
2. Linux OS for component 2,3
3. Tools/Compilers used: Visual Studio (for 1, 5), gcc (for 2 & 3), Python 2.7 runtime (4 & 6).

Before starting the steps below, copy the contents of the Linux folder on a Linux machine.

To execute run the components in the following order strictly:

In Windows box:

1. Build "GraphInputConfigGenerator" project under Windows folder. Run it from command line like "GraphInputConfigGenerator.exe > op.txt". This will create a file called op.txt in the same path as the exe.

2. Copy the newly generated op.txt file in ~Graph_generator\Data in the Linux box.

In Linux box:

3. Compile the Graph Generator component with "gcc Graph_generator.c -o gg.out" command.

4. Copy the executable in the "Data" folder and execute the Graph Generator with "./gg.out < op.txt" command. You should see a bunch of ".input" files being created in the "Data" folder.

5. Execute "python scriptforvcis.py" command from Graph_generator directory. Change the paths as required in this script. :) This command will generate a bunch of files in "OutVCIS" folder.

6. Copy the contents of this "OutVCIS" folder and copy them in "~\BudgetHubAlgoRunner\BudgetHubAlgoRunner\bin\Debug\OutVCIS" folder in Windows machine. Also copy the contents of "Data" folder from Linux into "\BudgetHubAlgoRunner\BudgetHubAlgoRunner\bin\Debug\Data" folder in Windows.

In Windows Box:

7. Build and run the "BudgetHubAlgoRunner" project. It will generate a bunch of csv files in "~\BudgetHubAlgoRunner\BudgetHubAlgoRunner\bin\Debug\Graphs\<GUID>" folder.

8. Execute "python GraphCreator.py" from "~\BudgetHubAlgoRunner\BudgetHubAlgoRunnerc\bin\Debug\" folder after updating the path in the python script. This will give the simulation results in png image file format.

