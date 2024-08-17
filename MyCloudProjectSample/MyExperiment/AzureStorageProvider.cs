using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using MyCloudProject.Common;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace MyExperiment
{
    public class AzureStorageProvider : IStorageProvider
    {
        private MyConfig _config;
        private QueueClient _queueClient;
        private ILogger<AzureStorageProvider> logger;


        public AzureStorageProvider(IConfigurationSection configSection, ILogger<AzureStorageProvider> logger)
        {
            _config = new MyConfig();
            configSection.Bind(_config);
            this.logger = logger;
        }

        /// <summary>
        /// Commits a request by deleting its associated message from the Azure Queue.
        /// </summary>
        /// <param name="request">The request containing the message ID and receipt of the message to be deleted.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="request"/> is null.</exception>
        /// <remarks>
        /// This method initializes a <see cref="QueueClient"/> and attempts to delete the specified message. 
        /// Successful deletion is logged. Exceptions during the process are caught and logged.
        /// </remarks>
        public async Task CommitRequestAsync(IExerimentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }

            // Initialize the QueueClient with the storage connection string and queue name
            QueueClient queueClient = new QueueClient(this._config.StorageConnectionString, this._config.Queue);

            try
            {
                // Delete the message from the queue
                await queueClient.DeleteMessageAsync(request.MessageId, request.MessageReceipt);

                // Log that the message was successfully deleted
                logger?.LogInformation($"{DateTime.Now} - Successfully Deleted the request with MessageId: {request.MessageId}");
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the deletion process
                logger?.LogError($"{DateTime.Now} - An error occurred while deleting the request. Exception: {ex.Message}");
            }
        }


        /// <summary>
        /// Receives and processes messages from the Azure Queue.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="IExerimentRequest"/> object if a message is successfully received and deserialized; otherwise, returns null.
        /// </returns>
        /// <remarks>
        /// Initializes a <see cref="QueueClient"/> to receive messages from the queue. Messages are deserialized into an 
        /// <see cref="ExerimentRequestMessage"/> object. Logs received messages and deserialization results. Exceptions are caught 
        /// and logged. Returns null if no messages are received or the operation is canceled.
        /// </remarks>
        public async Task<IExerimentRequest> ReceiveExperimentRequestAsync(CancellationToken token)
        {
            
            // Initialize the QueueClient with the storage connection string and queue name
            _queueClient = new QueueClient(this._config.StorageConnectionString, this._config.Queue);
            
            // Receive messages from the queue asynchronously
            QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync();

            // Check if any messages were received
            if (messages != null && messages.Length > 0)
            {
                
                foreach (var message in messages)
                {
                    try
                    {
                        // Extract message text from the message body
                        string msgTxt = message.Body.ToString();

                        // Log that a message has been received
                        logger?.LogInformation($"{DateTime.Now} - Received the trigger-queue message:\n {msgTxt}");

                        // Deserialize the message JSON into an ExerimentRequestMessage object
                        var request = JsonSerializer.Deserialize<ExerimentRequestMessage>(msgTxt);

                        // Check if deserialization was successful
                        if (request != null)
                        {
                            // Assign message ID and pop receipt to the request object
                            request.MessageId = message.MessageId;
                            request.MessageReceipt = message.PopReceipt;

                            // Return the deserialized request object
                            return request;
                        }
                    }
                    catch (JsonException ex)
                    {
                        // Log an error if deserialization fails
                        logger?.LogError($"{DateTime.Now} - Failed to deserialize the message. Exception: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        // Log an error for any other exceptions during message processing
                        logger?.LogError($"{DateTime.Now} - An error occurred while processing the message. Exception: {ex.Message}");
                    }
                }
            }


            // Return null if cancellation is requested or no messages are received
            return null;
        }

        /// <summary>
        /// Uploads the experiment result to Azure Table Storage.
        /// </summary>
        /// <param name="result">The experiment result to be uploaded, including details such as ExperimentId, Name, Description, and timings.</param>
        /// <remarks>
        /// Initializes a <see cref="TableServiceClient"/> and <see cref="TableClient"/> to interact with Azure Table Storage. 
        /// Creates the table if it does not exist and adds a new entity with a unique RowKey. Handles exceptions by logging errors.
        /// </remarks>
        /// <exception cref="Exception">Thrown if any errors occur during the upload process.</exception>
        public async Task UploadExperimentResult(IExperimentResult result)
        {
            try
            {
                // New instance of the TableClient class
                TableServiceClient tableServiceClient = new TableServiceClient(this._config.StorageConnectionString);
                TableClient tableClient = tableServiceClient.GetTableClient(tableName: this._config.ResultTable);
                await tableClient.CreateIfNotExistsAsync();

                // Generate a unique RowKey
                string uniqueRowKey = Guid.NewGuid().ToString();

                // Creating a table entity from the result
                var entity = new TableEntity(this._config.ResultTable, uniqueRowKey)
                {
                    { "ExperimentId", result.ExperimentId },
                    { "Name", result.Name },
                    { "Description", result.Description },
                    { "StartTimeUtc", result.StartTimeUtc },
                    { "EndTimeUtc", result.EndTimeUtc },
                    { "DurationInSec", result.DurationSec },
                    { "Ip_MinValue" ,result.MinValue },
                    { "Ip_MaxValue" ,result.MaxValue },
                    { "Ip_MaxBoost" ,result.BoostMax },
                    { "Ip_MinOctOverlapCycles" ,result.MinimumOctOverlapCycles },
                    { "Ip_InputBits" ,result.InputBits },
                    { "Ip_NumColumns" ,result.NumColumns },
                    { "Ip_CellsPerColumn" ,result.ExpCellsPerColumn },
                    { "Ip_DutyCyclePeriod" ,result.ExpDutyCyclePeriod },
                    { "Ip_LocalAreaDensity" ,result.ExpLocalAreaDensity },
                    { "Ip_ActivationThreshold" ,result.ExpActivationThreshold },
                    { "Op_FirstStableCycle", result.FirstStableCycle },
                    { "Op_LastStableCycle", result.LastStableCycle },
                    { "OutputFile", result.OutputFile },

                };

                // Adding the newly created entity to the Azure Table.
                await tableClient.AddEntityAsync(entity);

            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                logger?.LogError(ex, "Failed to upload to Table Storage:");
                Console.WriteLine(ex.Message);
             
            }
            
        }

        /// <summary>
        /// Uploads the experiment result file to Azure Blob Storage.
        /// </summary>
        /// <param name="experimentName">The name of the experiment used as the name for the blob in the cloud storage.</param>
        /// <param name="result">The experiment result object containing the path of the output file to be uploaded.</param>
        /// <remarks>
        /// Initializes a <see cref="BlobServiceClient"/> and <see cref="BlobContainerClient"/> to interact with Azure Blob Storage. 
        /// Creates the container if it does not exist and uploads the result file to the specified blob. The file is overwritten if it already exists.
        /// Handles exceptions by logging errors.
        /// </remarks>
        /// <exception cref="Exception">Thrown if any errors occur during the upload process.</exception>

        public async Task UploadResultAsync(string experimentName, IExperimentResult result)
        {
            try
            {
                // Get the first output file from the result's OutputFiles list
                string outputFile = result.OutputFile;

                // Assign the experiment name to the result's OutputFile property
                result.OutputFile = experimentName;

                // Initialize the BlobServiceClient with the storage connection string from the configuration
                BlobServiceClient blobServiceClient = new BlobServiceClient(this._config.StorageConnectionString);

                // Initialize the BlobContainerClient for the specified result container from the configuration
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(this._config.ResultContainer);

                // Create the container if it does not already exist (asynchronously)
                await containerClient.CreateIfNotExistsAsync();

                // Generate a unique blob name using the experiment name and a GUID
                string uniqueBlobName = $"{experimentName}-{Guid.NewGuid()}{Path.GetExtension(outputFile)}";

                // Initialize the BlobClient for the specific blob using the output file's name
                BlobClient blobClient = containerClient.GetBlobClient(uniqueBlobName);

                // Upload the file to the blob storage, overwriting it if it already exists (asynchronously)
                await blobClient.UploadAsync(outputFile, overwrite: true);

               
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                logger?.LogError(ex, "An error occurred while uploading the result:");
                Console.WriteLine(ex.Message);

                
            }
        }

    }

}