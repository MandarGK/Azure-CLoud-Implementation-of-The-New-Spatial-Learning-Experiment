using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    /// <summary>
    /// Defines the contract for the message request that will run your experiment.
    /// </summary>
    public interface IExerimentRequest
    {
        /// <summary>
        /// Any identifier of yout choice.
        /// </summary>
        public string ExperimentId { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the input file or entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the input file or entity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the identifier for a message.
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the receipt or acknowledgment of the message.
        /// </summary>
        public string MessageReceipt { get; set; }

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
    }
}
