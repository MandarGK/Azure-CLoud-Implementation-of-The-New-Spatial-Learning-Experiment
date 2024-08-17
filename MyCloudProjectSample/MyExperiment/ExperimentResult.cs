using Azure;
using Azure.Data.Tables;
using MyCloudProject.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyExperiment
{

    public class ExperimentResult : ITableEntity, IExperimentResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentResult"/> class with the specified partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key for the experiment result.</param>
        /// <param name="rowKey">The row key for the experiment result.</param>
        public ExperimentResult(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the partition key for the experiment result.
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets the row key for the experiment result.
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for the experiment result.
        /// Nullable to allow for cases where the timestamp might not be set.
        /// </summary>
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the ETag for the experiment result.
        /// </summary>
        public ETag ETag { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the experiment.
        /// </summary>
        public string ExperimentId { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the experiment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the experiment.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the UTC start time of the experiment.
        /// Nullable to allow for cases where the start time might not be set.
        /// </summary>
        public DateTime? StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC end time of the experiment.
        /// Nullable to allow for cases where the end time might not be set.
        /// </summary>
        public DateTime? EndTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the duration of the experiment in seconds.
        /// </summary>
        public long DurationSec { get; set; }
       
        /// <summary>
        /// Gets or sets the duration of the experiment as a TimeSpan.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the path to the output file of the experiment.
        /// </summary>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the minimum value for the experiment's parameters.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        /// Gets or sets the maximum value for the experiment's parameters.
        /// </summary>
        public double MaxValue { get; set; }


        /// <summary>
        /// Gets or sets the maximum boost value applied during the experiment's learning phase.
        /// </summary>
        public double BoostMax { get; set; }

        /// <summary>
        /// Gets or sets A number between 0 and 1.0, used to set a floor on how often a column should be activated, when learning spatial patterns.
        /// </summary>
        public double MinimumOctOverlapCycles { get; set; }


        /// <summary>
        /// Gets or sets the number of input bits used in the experiment.
        /// </summary>
        public int InputBits { get; set; }

        /// <summary>
        /// Gets or sets the number of columns used in the experiment's grid or structure.
        /// </summary>
        public int NumColumns { get; set; }

        /// <summary>
        /// Gets or sets the number of cells per column within the experiment's structure.
        /// </summary>
        public int ExpCellsPerColumn { get; set; }

        /// <summary>
        /// Gets or setsThe period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost or synPerConnectedCell. Shorter values make it more unstable and likely to oscillate.
        /// </summary>
        public int ExpDutyCyclePeriod { get; set; }

        /// <summary>
        /// The desired density of active columns within a local inhibition area.
        /// </summary>
        public int ExpLocalAreaDensity { get; set; }

        /// <summary>
        /// Gets or sets the activation threshold for the experiment.
        /// Defines the minimum requirement for triggering an action or result.
        /// </summary>
        public int ExpActivationThreshold { get; set; }

        /// <summary>
        /// Gets or sets the First Stable Cycle for the experiment.
        /// </summary>
        public int FirstStableCycle { get; set; }

        /// <summary>
        /// Gets or sets the Last Stable Cycle for the experiment.
        /// </summary>
        public int LastStableCycle { get; set; }

    }
}
