
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    public interface IExperimentResult
    {
        // Unique identifier for the experiment
        string ExperimentId { get; set; }

        // Name of the experiment
        public string Name { get; set; }

        // Description of the experiment
        public string Description { get; set; }

        // UTC timestamp when the experiment started 
        DateTime? StartTimeUtc { get; set; }

        // UTC timestamp when the experiment ended 
        DateTime? EndTimeUtc { get; set; }

        // Duration of the experiment in seconds
        public long DurationSec { get; set; }

        // Single output file of the experiment 
        public string OutputFile { get; set; }

        // Minimum value result from the experiment
        public double MinValue { get; set; }

        // Maximum value result from the experiment
        public double MaxValue { get; set; }

        // Duration of the experiment as a TimeSpan object
        public TimeSpan Duration { get; set; }

        //The maximum boost value applied during the experiment's learning phase.
        public double BoostMax { get; set; }

        //A number between 0 and 1.0, used to set a floor on how often a column should be activated, when learning spatial patterns.
        public double MinimumOctOverlapCycles { get; set;}

        //The number of input bits used in the experiment.
        public int InputBits { get; set; }

        //The number of columns used in the experiment's grid or structure.
        public int NumColumns { get; set; }

        //The number of cells per column within the experiment's structure.
        public int ExpCellsPerColumn { get; set; }

        //The period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost or synPerConnectedCell. Shorter values make it more unstable and likely to oscillate.
        public int ExpDutyCyclePeriod { get; set; }

        //The desired density of active columns within a local inhibition area.
        public int ExpLocalAreaDensity { get; set; }

        //Activation threshold used in sequence learning. If the number of active connected synapses on a distal segment is at least this threshold, the segment is declared as active one.
        public int ExpActivationThreshold { get; set; }

        //The First Stable Cycle for the experiment.
        public int FirstStableCycle { get; set; }

        //The Last Stable Cycle for the experiment.
        public int LastStableCycle { get; set; }
    }

}
