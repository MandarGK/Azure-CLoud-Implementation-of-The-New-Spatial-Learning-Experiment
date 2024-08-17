using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Drawing;
using System.IO;

namespace NeoCortexApiExperiment
{
    /// <summary>
    /// Implements an experiment that demonstrates how to learn spatial patterns.
    /// SP will learn every presented input in multiple iterations.
    /// </summary>
    public class SpatialLearningExperiment
    {
        public SpatialPooler Run(double minvalue, double maxvalue,double boostmax, double minimumoctoverlapcycles, int inputbits, int numcolumns, int expcellspercolumn, int expdutycycleperiod, int explocalareadensity, int expactivationthreshold, string filePathName)
        {
            //Console.WriteLine($"Hello NeocortexApi! Experiment {nameof(SpatialLearningExperiment)}");

            
            //
            // This is a set of configuration parameters used in the experiment.
            HtmConfig cfg = new HtmConfig(new int[] { inputbits }, new int[] { numcolumns })
            {
                CellsPerColumn = expcellspercolumn,
                MaxBoost = boostmax,
                DutyCyclePeriod = expdutycycleperiod,
                MinPctOverlapDutyCycles = minimumoctoverlapcycles,

                GlobalInhibition = false,
                NumActiveColumnsPerInhArea = 0.02 * numcolumns,
                PotentialRadius = (int)(0.15 * inputbits),
                LocalAreaDensity = explocalareadensity,
                ActivationThreshold = expactivationthreshold,

                MaxSynapsesPerSegment = (int)(0.01 * numcolumns),
                Random = new ThreadSafeRandom(42),
                StimulusThreshold = 10

            };

            // This dictionary defines a set of typical encoder parameters.
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "W", 15},
                { "N", inputbits},
                { "Radius", -1.0},
                { "MinVal", minvalue},
                { "Periodic", false},
                { "Name", "scalar"},
                { "ClipInput", false},
                { "MaxVal", maxvalue}
            };

            EncoderBase encoder = new ScalarEncoder(settings);

            // We create here  random input values.
            List<double> inputValues = new List<double>();
            for (int i = (int)minvalue; i < (int)maxvalue; i++)
            {
                inputValues.Add((double)i);
            }

            var sp = RunExperiment(cfg, encoder, inputValues, numcolumns, filePathName, maxvalue);
            return sp;
        }

        /// <summary>
        /// Implements the experiment.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="encoder"></param>
        /// <param name="inputValues"></param>
        /// <returns>The trained version of the SP.</returns>
        private static SpatialPooler RunExperiment(HtmConfig cfg, EncoderBase encoder, List<double> inputValues, int numColumns,string filePathName, double maxvalue)
        {
            // Creates the htm memory.
            var mem = new Connections(cfg);

            int endCycle = 0;
            int startCycle = 0;

            bool isInStableState = false;

            // HPC extends the default Spatial Pooler algorithm.
            // The purpose of HPC is to set the SP in the new-born stage at the beginning of the learning process.
            // In this stage the boosting is very active, but the SP behaves instable. After this stage is over
            // (defined by the second argument) the HPC is controlling the learning process of the SP.
            // Once the SDR generated for every input gets stable, the HPC will fire event that notifies your code
            // that SP is stable now.
            HomeostaticPlasticityController hpa = new HomeostaticPlasticityController(mem, inputValues.Count * 40,
                (isStable, numPatterns, actColAvg, seenInputs) =>
                {
                    // Event should only be fired when entering the stable state.
                    // Ideal SP should never enter instable state after stable state.
                    if (isStable == false)
                    {
                        Debug.WriteLine($"INSTABLE STATE");
                        Console.WriteLine($"INSTABLE STATE");

                        isInStableState = false;
                    }
                    else
                    {
                        Debug.WriteLine($"STABLE STATE");
                        Console.WriteLine($"STABLE STATE");

                        isInStableState = true;
                    }
                });


            // It creates the instance of Spatial Pooler Multithreaded version.
            SpatialPooler sp = new SpatialPooler(hpa);
            //sp = new SpatialPoolerMT(hpa);

            // Initializes the Spatial Pooler
            sp.Init(mem, new DistributedMemory() { ColumnDictionary = new InMemoryDistributedDictionary<int, NeoCortexApi.Entities.Column>(1) });

            // mem.TraceProximalDendritePotential(true);

            // It creates the instance of the neo-cortex layer.
            // Algorithm will be performed inside of that layer.
            CortexLayer<object, object> cortexLayer = new CortexLayer<object, object>("L1");

            // Add encoder as the very first module. This model is connected to the sensory input cells
            // that receive the input. Encoder will receive the input and forward the encoded signal
            // to the next module.
            cortexLayer.HtmModules.Add("encoder", encoder);

            // The next module in the layer is Spatial Pooler. This module will receive the output of the
            // encoder.
            cortexLayer.HtmModules.Add("sp", sp);

            double[] inputs = inputValues.ToArray();

            // Will hold the SDR of every inputs.
            Dictionary<double, int[]> prevActiveCols = new Dictionary<double, int[]>();

            // Will hold the similarity of SDRk and SDRk-1 for every input.
            Dictionary<double, double> prevSimilarity = new Dictionary<double, double>();

            //
            // Initialize start similarity to zero.
            foreach (var input in inputs)
            {
                prevSimilarity.Add(input, 0.0);
                prevActiveCols.Add(input, new int[0]);
            }

            // Learning process will take 1000 iterations (cycles)
            int maxSPLearningCycles = 1000;

            // Initializing counter to break the loop after reaching stability
            int stableCycleElapsed = 0;

            int loopBreakerThreshold = 100;

            // Creating a Dictionary to store SDR values.
            Dictionary<int, List<List<int>>> SdrDictionary = new Dictionary<int, List<List<int>>>();

            // Define a list to store SDRs for the 0th input
            List<(int Cycle, double Similarity, int[] ActCols, List<int> SDR)> sdrsForInput0 = new List<(int Cycle, double Similarity, int[] ActCols, List<int> SDR)>();

            // Define a dictionary to track stability for each input
            Dictionary<int, int> stableIterationCount = new Dictionary<int, int>();

            // Open the file for writing
            using (StreamWriter writer = new StreamWriter(filePathName))
            {
                for (int cycle = 0; cycle < maxSPLearningCycles; cycle++)
                {
                    Debug.WriteLine($"Cycle  * {cycle} * Stability: {isInStableState}");
                    writer.WriteLine($"Cycle  * {cycle} * Stability: {isInStableState}");
                    Console.WriteLine($"Cycle  * {cycle} * Stability: {isInStableState}");

                    foreach (var input in inputs)
                    {
                        double similarity;

                        // Learn the input pattern.
                        // Output lyrOut is the output of the last module in the layer.
                        var lyrOut = cortexLayer.Compute((object)input, true) as int[];

                        // This is a general way to get the SpatialPooler result from the layer.
                        var activeColumns = cortexLayer.GetResult("sp") as int[];

                        var actCols = activeColumns.OrderBy(c => c).ToArray();

                        similarity = MathHelpers.JaccardSimilarity(activeColumns, prevActiveCols[input]);

                        // Check if the input key already exists in the dictionary.
                        if (!SdrDictionary.ContainsKey(Convert.ToInt32(input)))
                        {
                            // If not, add a new list to store SDR values for this input.
                            SdrDictionary[Convert.ToInt32(input)] = new List<List<int>>();
                        }

                        // Add the current SDR to the list for this input.
                        SdrDictionary[Convert.ToInt32(input)].Add(actCols.ToList());

                        if (Convert.ToInt32(input) <= 99 && MathHelpers.AreArraysEqual(activeColumns, prevActiveCols[input]))
                        {
                            stableIterationCount[Convert.ToInt32(input)]++; // Increment stable cycles count
                        }
                        else
                        {
                            stableIterationCount[Convert.ToInt32(input)] = 0; // Reset stable cycles count
                        }

                        if (Convert.ToInt32(input) == 0) // Check if the input is the 0th input
                        {
                            // Add the current cycle number and SDR for the 0th input to the list
                            sdrsForInput0.Add((cycle, similarity, actCols, actCols.ToList()));
                        }

                        string output = $"[cycle={cycle.ToString("D4")}, N={stableIterationCount[Convert.ToInt32(input)]} , stablecycles ={stableCycleElapsed}, i={input}, cols={actCols.Length} s={similarity}] SDR: {string.Join(", ", SdrDictionary[Convert.ToInt32(input)].Last())}";
                        Debug.WriteLine(output);
                        writer.WriteLine(output);
                        Console.WriteLine(output);

                        prevActiveCols[input] = activeColumns;
                        prevSimilarity[input] = similarity;
                    }

                    // Condition to check if the Spatial Pooler has Entered into Stable state.
                    if (isInStableState)
                    {
                        // Incrementing the Counter to get the desire value to print the SDR for 100 Stable Cycles 
                        stableCycleElapsed++;

                        if (stableCycleElapsed == loopBreakerThreshold)
                        {
                            // Print the last 100 stable cycles.
                            startCycle = Math.Max(0, cycle - 99); // Adjusted start cycle

                            // Checking the last 100th Cycle for the Iteration 
                            endCycle = cycle; // End cycle is the current cycle

                            Console.WriteLine("");
                            Console.WriteLine($"The Spatial Learning Experiment has achieved Stability at Cycle: {startCycle}"); 
                            Console.WriteLine($"The Spatial Learning Experiment has exited after 100 stable cycles at Cycle: {endCycle}");

                            Debug.WriteLine("");
                            Debug.WriteLine($"The Spatial Learning Experiment has achieved Stability at Cycle: {startCycle}");
                            Debug.WriteLine($"The Spatial Learning Experiment has exited after 100 stable cycles at Cycle: {endCycle}");

                            writer.WriteLine("");
                            writer.WriteLine($"The Spatial Learning Experiment has achieved Stability at Cycle: {startCycle}");                          
                            writer.WriteLine($"The Spatial Learning Experiment has exited after 100 stable cycles at Cycle: {endCycle}");

                            // Print stable cycles
                            PrintStableCycles(SdrDictionary, startCycle, endCycle, writer);

                            // Print SDRs of the 0th input
                            PrintSdrsForInput(SdrDictionary, sdrsForInput0, writer);

                            // Break after printing the last 100 stable cycles.
                            break;
                        }
                    }
                    else
                    {
                        // Clearing all SDR stored in dictionary during the instable state.
                        SdrDictionary.Clear();
                        // Setting the stableCycleElapsed to reset / 0 during the Instable state of Spatial pooler.
                        stableCycleElapsed = 0;
                        Debug.WriteLine($"Counter is set to zero; stability is not yet reached");
                        Console.WriteLine($"Counter is set to zero; stability is not yet reached");
                        writer.WriteLine($"Counter is set to zero; stability is not yet reached");
                    }
                }
            }
            sp.FirstStableCycle = startCycle;
            sp.LastStableCycle = endCycle;
            
            return sp;
        }

        /// <summary>
        /// Prints the SDRs for stable cycles within the specified range of cycles.
        /// </summary>
        /// <param name="SdrDictionary">A dictionary containing SDRs for different inputs and cycles.</param>
        /// <param name="startCycle">The starting cycle number to print SDRs from.</param>
        /// <param name="endCycle">The ending cycle number to print SDRs until.</param>
        /// <param name="writer">The StreamWriter instance to write to the file.</param>
        public static void PrintStableCycles(Dictionary<int, List<List<int>>> SdrDictionary, int startCycle, int endCycle, StreamWriter writer)
        {
            string separator = new string('*', 120);
            Debug.WriteLine($"{separator}\n");
            Debug.WriteLine($"Printing the SDRs of 100 consecutive stable cycles after achieving the stable state:\n");
            writer.WriteLine($"{separator}\n");
            writer.WriteLine($"Printing the SDRs of 100 consecutive stable cycles after achieving the stable state:\n");

            for (int i = startCycle; i <= endCycle; i++)
            {
                Debug.WriteLine($"Cycle * {i} *:");
                writer.WriteLine($"Cycle * {i} *:");

                foreach (var kvp in SdrDictionary)
                {
                    // Check if the key exists in the dictionary and has the required number of iterations.
                    if (SdrDictionary.ContainsKey(kvp.Key) && i - startCycle < SdrDictionary[kvp.Key].Count)
                    {
                        // Get the SDRs for the current input key and cycle.
                        List<int> sdrsForInput = SdrDictionary[kvp.Key][i - startCycle];

                        // Print the SDRs in a formatted way.
                        Debug.WriteLine($" Iteration: {kvp.Key} | SDR of {i - startCycle + 1} stable cycle: {Helpers.StringifyVector(sdrsForInput.ToArray())}");
                        writer.WriteLine($" Iteration: {kvp.Key} | SDR of {i - startCycle + 1} stable cycle: {Helpers.StringifyVector(sdrsForInput.ToArray())}");
                    }
                }

                Debug.WriteLine("");  // New line for the next cycle.
                writer.WriteLine("");  // New line for the next cycle.
            }
        }

        /// <summary>
        /// Prints the SDRs for the 0th input along with cycle number, similarity, active columns, and SDRs.
        /// </summary>
        /// <param name="sdrDictionary">A dictionary containing SDRs for different inputs.</param>
        /// <param name="sdrsForInput0">A list containing tuples of cycle number, similarity, active columns, and SDRs for the 0th input.</param>
        /// <param name="writer">The StreamWriter instance to write to the file.</param>
        private static void PrintSdrsForInput(Dictionary<int, List<List<int>>> sdrDictionary, List<(int Cycle, double Similarity, int[] ActCols, List<int> SDR)> sdrsForInput0, StreamWriter writer)
        {
            string separator = new string('*', 120);
            Debug.WriteLine($"{separator}\n");
            Debug.WriteLine($"Comparison of the SDRs of 0th input for all cycles:\n");
            writer.WriteLine($"{separator}\n");
            writer.WriteLine($"Comparison of the SDRs of 0th input for all cycles:\n");

            foreach (var (cycle, similarity, actCols, sdr) in sdrsForInput0)
            {
                string outputLine = $"Input 0, Cycle: {cycle}, s: {similarity}, Columns: {actCols.Length}, SDR: {string.Join(", ", sdr)}";
                Debug.WriteLine(outputLine);
                writer.WriteLine(outputLine);
            }
        }
    }
}
