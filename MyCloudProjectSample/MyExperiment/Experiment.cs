using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCloudProject.Common;
using NeoCortexApiExperiment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyExperiment
{
    /// <summary>
    /// Implements the machine learning experiment for cloud deployment. This class refactors code from a previous SE project.
    /// </summary>
    public class Experiment : IExperiment
    {
        private IStorageProvider storageProvider;  // Interface for storage operations
        private ILogger logger;  // Logger for recording events and errors
        private MyConfig config;  // Configuration settings for the experiment
        private string experimentId;  // Unique identifier for the experiment
        private string experimentName;  // Name of the experiment
        private string experimentDesc;  // Description of the experiment

        /// <summary>
        /// Initializes a new instance of the <see cref="Experiment"/> class.
        /// </summary>
        /// <param name="configSection">Configuration section containing settings for the experiment.</param>
        /// <param name="storageProvider">The storage provider to interact with cloud storage.</param>
        /// <param name="log">Logger for recording events and errors.</param>
        public Experiment(IConfigurationSection configSection, IStorageProvider storageProvider, ILogger log)
        {
            this.storageProvider = storageProvider;
            this.logger = log;
            config = new MyConfig();
            configSection.Bind(config);
        }

        /// <summary>
        /// Sets the details for the experiment.
        /// </summary>
        /// <param name="experimentId">The unique identifier for the experiment.</param>
        /// <param name="experimentName">The name of the experiment.</param>
        /// <param name="experimentDescription">A description of the experiment.</param>
        public void setExperimentDetails(string experimentId, string experimentName, string experimentDescription)
        {
            this.experimentId = experimentId;
            this.experimentName = experimentName;
            this.experimentDesc = experimentDescription;
        }

        /// <summary>
        /// Runs the experiment asynchronously with the specified parameters.
        /// </summary>
        /// <param name="minvalue">Minimum input value for the experiment.</param>
        /// <param name="maxvalue">Maximum input value for the experiment.</param>
        /// <param name="boostmax">Maximum boost value for the experiment.</param>
        /// <param name="minimumoctoverlapcycles">Minimum overlap cycles for the experiment.</param>
        /// <param name="inputbits">Number of input bits.</param>
        /// <param name="numcolumns">Number of columns in the Spatial Pooler.</param>
        /// <param name="expcellspercolumn">Number of cells per column in the Spatial Pooler.</param>
        /// <param name="expdutycycleperiod">Duty cycle period for the experiment.</param>
        /// <param name="explocalareadensity">Local area density for the experiment.</param>
        /// <param name="expactivationthreshold">Activation threshold for the experiment.</param>
        /// <param name="filePath">Path to the output file.</param>
        /// <returns>An <see cref="IExperimentResult"/> representing the results of the experiment.</returns>
        public Task<IExperimentResult> RunAsync(double minvalue, double maxvalue, double boostmax, double minimumoctoverlapcycles, int inputbits, int numcolumns, int expcellspercolumn, int expdutycycleperiod, int explocalareadensity, int expactivationthreshold, string filePath)
        {
          

            // Initialize the ExperimentResult object to store results
            ExperimentResult response = new ExperimentResult(this.config.GroupId, null);

            // Log the start of the experiment
            response.StartTimeUtc = DateTime.UtcNow;
            logger?.LogInformation("Experiment started at {StartTime}", response.StartTimeUtc);

            // Execute the experiment
            SpatialLearningExperiment experiment = new SpatialLearningExperiment();
            var output = experiment.Run(minvalue, maxvalue, boostmax, minimumoctoverlapcycles, inputbits, numcolumns, expcellspercolumn, expdutycycleperiod, explocalareadensity, expactivationthreshold, filePath);
           
            // Log and calculate experiment duration
            response.EndTimeUtc = DateTime.UtcNow;
            var elapsedTime = response.EndTimeUtc - response.StartTimeUtc;
            response.DurationSec = (long)elapsedTime.GetValueOrDefault().TotalSeconds;
            response.Description = experimentDesc;
            response.ExperimentId = experimentId;
            response.Name = experimentName;
            response.MinValue = minvalue;
            response.MaxValue = maxvalue;   
            response.BoostMax = boostmax;
            response.MinimumOctOverlapCycles = minimumoctoverlapcycles;
            response.InputBits = inputbits;
            response.NumColumns = numcolumns;
            response.ExpCellsPerColumn = expcellspercolumn;
            response.ExpDutyCyclePeriod = expdutycycleperiod;
            response.ExpLocalAreaDensity = explocalareadensity;
            response.ExpActivationThreshold = expactivationthreshold;
            response.FirstStableCycle = output.FirstStableCycle;
            response.LastStableCycle = output.LastStableCycle;
            response.OutputFile = filePath;

            // Return the results asynchronously
            return Task.FromResult<IExperimentResult>(response);
        }
    }
}
