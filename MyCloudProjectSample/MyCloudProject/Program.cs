using MyCloudProject.Common;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using MyExperiment;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using System.Text.Json;
using System.Text;
using System.IO;

namespace MyCloudProject
{
    /// <summary>
    /// The entry point for the experiment application. It sets up the environment, handles configuration, logging,
    /// and orchestrates the execution of the experiment based on messages received from the Azure Queue.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The name of the project for the experiment.
        /// </summary>
        private static string _projectName = "ML 23/24-03 Implement the New Spatial Learning Experiment _TheLazyCoders";

        /// <summary>
        /// The main method that starts the experiment, processes requests, and handles results.
        /// </summary>
        /// <param name="args">Command-line arguments for the application.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        static async Task Main(string[] args)
        {
            // Create a cancellation token source to handle cancellation requests.
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Handle the cancellation request (Ctrl+C) to properly cancel the operation.
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };


            // Initialize configuration settings from the command-line arguments or environment.
            var cfgRoot = Common.InitHelpers.InitConfiguration(args);
            var cfgSec = cfgRoot.GetSection("MyConfig");

            // Determine the path for the output file.
            string outputFilePath = Environment.GetEnvironmentVariable("OUTPUT_FILE_PATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "output.txt");
            

            Console.WriteLine($"{_projectName}");
            

            // Initialize logging.
            var logFactory = InitHelpers.InitLogging(cfgRoot);
            var logger = logFactory.CreateLogger<Program>();
            var storagelogger = logFactory.CreateLogger<AzureStorageProvider>();

            logger?.LogInformation($"{DateTime.Now} - Started experiment: {_projectName}");
            

            // Initialize the storage provider and experiment instances.
            IStorageProvider storageProvider = new AzureStorageProvider(cfgSec, storagelogger);
            IExperiment experiment = new Experiment(cfgSec, storageProvider, logger);
            logger?.LogInformation("Attempting to receive messages from the queue...");

            // Main loop to continuously process messages from the queue until cancellation is requested.
            while (cancellationTokenSource.Token.IsCancellationRequested == false)
            {
                
                // Receive an experiment request from the storage provider.
                IExerimentRequest request = await storageProvider.ReceiveExperimentRequestAsync(cancellationTokenSource.Token);

                if (request != null)
                {
                    try
                    {
                        
                        // Set experiment parameters based on the received request.
                        logger?.LogInformation("Setting the Experiment Parameters");
                        experiment.setExperimentDetails(request.ExperimentId, request.Name, request.Description);

                        // Run the experiment and get the result.                        
                        IExperimentResult result = await experiment.RunAsync(request.MinValue, request.MaxValue, request.BoostMax, request.MinimumOctOverlapCycles,  request.InputBits, request.NumColumns, request.ExpCellsPerColumn, request.ExpDutyCyclePeriod, request.ExpLocalAreaDensity, request.ExpActivationThreshold, outputFilePath);

                        // Upload the result to Blob storage.
                        await storageProvider.UploadResultAsync("output-file", result);
                        logger?.LogInformation($"{DateTime.Now} - Experiment finished. Uploading output to Blob storage.");


                        // Upload experiment result data to Table Storage.
                        await storageProvider.UploadExperimentResult(result);
                        logger?.LogInformation("Successfully uploaded the data into Table Storage.");

                        // Commit the request by deleting the message from the queue.
                        await storageProvider.CommitRequestAsync(request);
                        logger?.LogInformation("Message deleted from the queue. Waiting for the next message.");
                    }
                    catch (Exception ex)
                    {
                        // Log any errors that occur during the experiment run.
                        logger?.LogError(ex, "An error occurred while running the experiment.");
                    }
                }
                else
                {
                    // If no request is received, wait for a short period before checking again.
                   
                    await Task.Delay(500);
                    logger?.LogTrace("Queue empty, currently waiting for queue message...");
                }
            }

            logger?.LogInformation($"{DateTime.Now} - Experiment exit: {_projectName}");
        }
    }
}
