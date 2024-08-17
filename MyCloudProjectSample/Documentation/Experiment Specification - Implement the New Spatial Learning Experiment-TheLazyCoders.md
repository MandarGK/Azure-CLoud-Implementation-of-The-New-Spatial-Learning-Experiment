# Azure Cloud Implementation: ML 23/24-03 Implement the New Spatial Learning Experiment _TheLazyCoders
<a id="top"></a>

## Table of contents
1. [Introduction](#introduction)
2. [SE Experiment Preview](#se-experiment-preview)
3. [Goal of Cloud Experiment](#goal-of-cloud-experiment)
4. [Project Architecture](#project-architecture)
5. [Project Implementation](#project-implementation)
6. [Asynchronous methods for Experiment Request Handling and Result Storage](#asynchronous-methods-for-experiment-request-handling-and-result-storage)
7. [Azure Cloud Deployment](#azure-cloud-deployment)
8. [Docker Image Creation and Deployment](#docker-image-creation-and-deployment)
9. [Project Execution](#project-execution)
10. [Conclusion](#conclusion)
11. [References](#references)

## Introduction
The new Spatial Learning experiment aims to enhance the Spatial Pooling (SP) algorithm by fine-tuning key parameters and optimizing the performance of the HTM Spatial Pooler while addressing specific challenges encountered during the learning phase. The slow activation of the dataset during the Spatial Pooler (SP) learning phase was resolved by optimizing indexing, improving system efficiency. Jaccard similarity was used to compare Sparse Distributed Representations (SDRs), effectively capturing both shared patterns and unique variations. Essential parameters were fine-tuned to ensure system stability, with the final output demonstrating the improvements clearly. 

The project has been refactored for deployment in a cloud environment using Microsoft Azure. Consequently, a Docker image was created and deployed using Azure Container Registry and Azure Container Instances. Moreover, message queue-based input processing has been implemented, and the resulting output is stored in Azure Blob Storage. On the other hand, the performance and health of the application are monitored using Azure Container Instances.

## SE Experiment Preview

Hierarchical Temporal Memory (HTM) is a machine learning algorithm designed to emulate the human neocortex. The HTM architecture includes:

- **Spatial Pooler (SP)**: Transforms input data into Sparse Distributed Representations (SDRs).
- **Temporal Memory (TM)**: Handles the learning process.

In this project, the focus is on a new Spatial Learning experiment aimed at optimizing the HTM Spatial Pooler. This involves:

- **Fine-Tuning Parameters**: Adjusting key parameters to enhance the performance of the Spatial Pooler.
- **Addressing Challenges**: Resolving issues such as slow activation of mini-columns during the learning phase through detailed debugging and analysis.

The experiment uses a dictionary for storing SDR values and introduces two key parameters:

- **N**: Total iterations where the SDR of the input remains unchanged.
- **Stable Cycles**: Measures of stability in the learning process.

The effectiveness of these methods is demonstrated through empirical data, highlighting significant improvements in efficiency metrics and system stability.

### Important SE Project Links

1. [SE Project Documentation](https://github.com/MandarGK/neocortexapi/tree/TheLazyCoders/source/Documentation(New%20Spatial%20Learning%20Experiment-2023-2024))<br/>
2. [The Spatial Learning Experiment](https://github.com/MandarGK/neocortexapi/tree/TheLazyCoders/source/Samples/NeoCortexApiExperiment)<br/>

## Goal of Cloud Experiment

The goal of this Azure Cloud implementation for the "Implement the new Spatial Learning Experiment" project is to deploy the Spatial Learning experiment into a cloud environment. The project is containerized using Docker, and the Docker image is published to an Azure Container Registry. The image is then run using Azure Container Instances.

Additionally, Azure Blob Storage is integrated to handle input data, store Sparse Distributed Representations (SDRs), and upload experiment results. The main experiment focuses on optimizing the HTM Spatial Pooler by fine-tuning parameters and validating improvements in SDR stability and system performance. The Results, including SDR stability and system performance metrics, are stored and analyzed to validate the experiment's success. 

Implementing cloud solutions leverages scalability and elasticity, enabling them to handle a wide range of data sources and support various applications, including sensor data processing, natural language understanding, and recommendation systems.

[Move to Top](#top)

## Project Architecture

In this section, Figure 1 illustrates the project's architectural diagram, showcasing the integration of various components that constitute the project's architecture and workflow. This diagram highlights the interactions and interdependencies among the different modules, providing a detailed overview of the entire project structure.

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/architecture.jpg">
  <br>
  <em>Figure 1: <i>Project Architecture - Azure Cloud Implementation: ML23/24-03 Implement the New Spatial Learning Experiment</i></em>
</p>


### Features
- Receives experiment requests from an Azure Storage queue.
- Runs the Spatial Learning experiment with the parameters received from trigger queue message.
- Uploads experiment results to Azure Table Storage, including output SDR's to Azure Blob storage,  for analysis and documentation.
- Logs the entire process for debugging, monitoring, and ensuring consistent system performance.




### Tools and Technologies Used
| Tools/Technology                | Description                                                                                                                                                                      |
|---------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Visual Studio**         | A comprehensive Integrated Development Environment (IDE) from Microsoft, utilized for developing and debugging applications across multiple platforms, including Windows, macOS, and Linux. |
| **GitHub**                | A web-based platform offering version control and collaborative development, utilizing Git for managing source code across various programming languages and facilitating team collaboration. |
| **Docker**                | A containerization platform that streamlines the deployment, scaling, and management of applications by encapsulating them within lightweight, portable containers, ideal for cloud environments like Azure. |
| **Docker Image**          | A self-contained, lightweight snapshot of an application and its dependencies, designed to be deployed in Docker containers, ensuring consistency and portability across different environments. |
| **Azure Container Instances** | A serverless compute service allowing the execution of Docker containers in the Azure cloud without the need to manage underlying virtual machines, simplifying container deployment and scaling. |
| **Azure Container Registry** | A managed container registry service in Azure for storing, managing, and securing Docker container images, facilitating streamlined deployment and version control. |
| **Azure Storage**         | A cloud-based storage solution by Microsoft providing scalable and secure storage options, including blob, file, queue, and table storage, to handle diverse data storage needs. |
| **Azure Blob Storage**    | An Azure service designed for storing vast amounts of unstructured data, such as documents and media files. In this project, it is used to manage input and output data containers for storing experiment data and results. |
| **Azure Queue**           | A messaging service that facilitates asynchronous communication by queuing messages, which trigger application processes or workflows, ensuring efficient task management and execution. |

[Move to Top](#top)

## Project Implementation
This section offers a comprehensive breakdown of the project's architecture and the classes utilized to operate it in a cloud environment. It also answers questions related to the implementation details. Furthermore, the link to the cloud project are given below for better understanding. 

[MyCloudProject](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/tree/TheLazyCoders/Source/MyCloudProjectSample/MyCloudProject)

[MyCloudProject.Common](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/tree/TheLazyCoders/Source/MyCloudProjectSample/MyCloudProject.Common)

[MyExperiment](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/tree/TheLazyCoders/Source/MyCloudProjectSample/MyExperiment)

### 1. MyCloudProject
The main project directory containing the cloud-related code.

- **Dependencies**: Required dependencies for cloud deployment.
- **Properties**: Project properties and settings.
- **Program.cs**: The main entry point of the application. It initializes configuration, logging, and runs the experiment in a loop, processing messages from the Azure Queue.
- **appsettings.json**: Configuration settings for the project.
- **appsettings.Development.json**: Configuration settings for the development environment.

### 2. MyCloudProject.Common
Contains common interfaces and helper classes used across the project.

- **Dependencies**: Required dependencies for shared components.
- **ExperimentRequest.cs**: Defines the structure of experiment requests.
- **IExperiment.cs**: Interface defining the contract for experiments.
- **IExperimentResult.cs**: Interface defining the contract for experiment results.
- **InitHelpers.cs**: Helper methods used for initialization tasks.
- **IStorageProvider.cs**: Interface for storage provider implementations.

### 3. MyExperiment
Contains the implementation specific to the Spatial Learning experiment.

- **Dependencies**: Required dependencies for the Spatial Learning experiment.
- **AzureStorageProvider.cs**: Implementation of the Azure storage provider for handling data storage and retrieval.
- **ExerimentRequestMessage.cs**: Defines the structure of experiment request messages.
- **Experiment.cs**: Contains the core logic for the Spatial Learning experiment. It orchestrates experiment execution, manages input and output, and interacts with the storage provider.
- **ExperimentResult.cs**: Defines the structure of results produced by the Spatial Learning experiment.
- **MathHelper.cs**: Provides mathematical utilities and helper functions used in the spatial learning computations.
- **MyConfig.cs**: Configuration settings related to the Spatial Learning experiment.
- **SpatialLearningExperiment.cs**: Implements the spatial learning algorithm specific to the Spatial Learning experiment.
- **SpatialPooler.cs**: Implements the spatial pooling algorithm, generating sparse distributed representations for the Spatial Learning experiment.



[Move to Top](#top)


## Asynchronous methods for Experiment Request Handling and Result Storage

This section details the implementation of key asynchronous methods responsible for managing the lifecycle of experiment requests and results in a cloud-based Spatial Learning experiment. These methods cover the entire process, from receiving and processing messages from an Azure Storage Queue to uploading results to Azure Table and Blob Storage, and finally, ensuring the cleanup of processed messages to prevent duplication.

### `ReceiveExperimentRequestAsync` Method

| Method | Description |
|--------|-------------|
| `ReceiveExperimentRequestAsync` | The ReceiveExperimentRequestAsync method facilitates the asynchronous retrieval and processing of messages from an Azure Storage Queue. It initializes a QueueClient using the storage connection string and queue name specified in the configuration. The method attempts to receive messages from the queue, logging each action for traceability. Upon receiving a message, it converts the message body from Base64 to a UTF-8 string and deserializes it into an ExperimentRequestMessage object. If the deserialization is successful, it assigns the MessageId and MessageReceipt to the request object for further processing. The method logs the successful processing or any errors if encountered during the process. If no message is received or an error occurs, it logs the outcome and returns null. In our experiment, we take the input directly from the queue message, ensuring that the experiment runs based on the parameters specified in the received ExperimentRequestMessage. |

```csharp
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
```

### `UploadExperimentResult` Method

| Method | Description |
|--------|-------------|
| `UploadExperimentResult` | The UploadExperimentResult method handles the asynchronous upload of experiment results to Azure Table Storage. It initializes a TableServiceClient using the storage connection string from the configuration, and retrieves a TableClient for the specified table. The method ensures the table exists or creates it if necessary. A unique RowKey is generated for each new entity, which is populated with data from the experiment result. The method attempts to add the entity to the table, logging successful operations and handling errors such as request failures and unexpected exceptions. |


```csharp
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

```

### `UploadResultAsync` Method

| Method | Description |
|--------|-------------|
| `UploadResultAsync` | The UploadResultAsync method is responsible for asynchronously uploading experiment result files to Azure Blob Storage. The method begins by extracting the output file from the result object. It then assigns the experiment name to the OutputFile property, effectively setting the output file name to match the experiment's name. Next, a BlobServiceClient is initialized using the storage connection string from the configuration, and a BlobContainerClient is created or retrieved for the specified container. The method ensures that the container exists by creating it asynchronously if it doesn't already exist. A unique blob name is then generated using the experiment name and a GUID, ensuring that each upload is uniquely identified. A BlobClient is initialized for the specific blob using this unique name. The method proceeds to upload the file to Azure Blob Storage, with the overwrite option enabled to replace any existing file with the same name. Finally, the method logs the successful upload or catches and logs any exceptions that may occur during the process, ensuring robust error handling and traceability.|




```csharp
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

```
### `CommitRequestAsync` Method


| Method | Description  |
|---|---|
| `CommitRequestAsync` | The CommitRequestAsync method processes the deletion of a message from an Azure Storage Queue. It begins by verifying that the provided request is not null, throwing an ArgumentNullException if it is. The method then initializes a QueueClient using the storage connection string and queue name specified in the configuration. It attempts to delete the message identified by MessageId and MessageReceipt from the provided IExperimentRequest. The method logs the successful deletion of the message or any exceptions that occur during the process. This ensures that messages are removed from the queue after processing, preventing duplicate processing. Robust error handling is in place, with logs generated for traceability. |

```csharp
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
```

These methods collectively handle the lifecycle of experiment requests and results within your cloud-based Spatial Learning experiment, including message retrieval, result storage, and cleanup operations.

[Move to Top](#top)

## Azure Cloud Deployment

To deploy our application in Azure, start by setting up the essential components: Resource Group, Storage Container, Queue, Table, Container Registry, and Container Instances. These components are created manually via the Azure portal. Once configured, you can view and manage all these resources within the specified Resource Group. This setup ensures that all required resources are organized and accessible for effective cloud application deployment and management.

| Azure Services             | Details             |
|----------------------------|---------------------|
| Azure Resource Group       | RG-Team_TheLazyCoders       |
| Azure Container Registry   | thelazycodersregistry    |
| Azure Container Instances  | thelazycoderscontainerinstance  |
| Azure Storage              | thelazycoders3     |
|result Container            | result-files|
| Result Table               | results          |
| Queue                      | trigger-queue          |


<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/Azure%20Resource%20group.png">
  <br>
  <em>Figure 2: <i>Azure Resource Group - RG-Team_TheLazyCoders</i></em>
</p>

### Experiment Input Parameters

The experiment input parameters are provided through an Azure Storage Queue trigger-queue in JSON format. These parameters guide the configuration and execution of the experiment by defining various settings and thresholds. 
```json
{
  "ExperimentId": "ML 23/24-03",
  "Name": "The Spatial Learning Experiment",
  "Description": "Experiment-1",
  "MessageId": "1",
  "MessageReceipt": "1-receipt",
  "MinValue": 0,
  "MaxValue": 100,
  "BoostMax": 10,
  "MinimumOctOverlapCycles": 0.001,
  "InputBits": 200,
  "NumColumns":1024,
  "ExpCellsPerColumn": 10,
  "ExpDutyCyclePeriod": 1000,
  "ExpLocalAreaDensity": -1,
  "ExpActivationThreshold": 10
}
```

#### Below is a description of each input parameter:


| **Parameter**                | **Description**                                                                                                                |
|------------------------------|--------------------------------------------------------------------------------------------------------------------------------|
| **ExperimentId**              | A unique identifier for the experiment.                                                                                        |
| **Name**                      | The name of the experiment.                                                                                                     |
| **Description**               | A brief description of the input file or entity related to the experiment.                                                     |
| **MessageId**                 | The unique identifier for the message in the queue. This is used to track and delete the message after processing.              |
| **MessageReceipt**            | A receipt provided by the Azure Queue service when the message is received. This is required for message deletion after processing. |
| **MinValue**                  | The minimum value to be used in the experiment.                                                                                 | 
| **MaxValue**                  | The maximum value to be used in the experiment.                                                                                 |
| **BoostMax**                  | The maximum boost value applied during the experiment's learning phase.                                                         |
| **MinimumOctOverlapCycles**   | A number between 0 and 1.0, used to set a floor on how often a column should be activated, when learning spatial patterns.                                                |                                                       
| **InputBits**                 | The number of input bits used in the experiment.                                                                                |
| **NumColumns**                | The number of columns used in the experiment's grid or structure.                                                               |
| **ExpCellsPerColumn**         | The number of cells per column within the experiment's structure.                                                               |
| **ExpDutyCyclePeriod**        | The period used to calculate duty cycles. Higher values make it take longer to respond to changes in boost or synPerConnectedCell. Shorter values make it more unstable and likely to oscillate.                                                                       |
| **ExpLocalAreaDensity**        | The desired density of active columns within a local inhibition area.                                                                  |
| **ExpActivationThreshold**    | Activation threshold used in sequence learning. If the number of active connected synapses on a distal segment is at least this threshold, the segment is declared as active one.               |


These parameters are used to configure various aspects of the experiment, such as thresholds, cycles, and structural elements like columns and cells. They allow for flexibility in experiment design, enabling adjustments to key settings that influence the experiment's behavior and outcomes.

The **ExperimentId**, **MessageId**, and **MessageReceipt** are crucial for tracking and managing the experiment within the cloud environment, ensuring that each experiment is uniquely identified and its associated message is correctly processed and removed from the queue.


### Experiment Output Parameters

The ExperimentResult class is designed to encapsulate all output data from an experiment, tailored for use with Azure Table Storage. It includes essential properties such as ExperimentId, Name, and Description, providing key identifiers and context for the experiment. Timestamps like StartTimeUtc and EndTimeUtc record the precise start and end times, while DurationSec to track the total experiment duration. The class also captures the location of the result file through the OutputFile property. 

Detailed experiment input parameters are stored, including MinValue, MaxValue, and BoostMax, which define the range and boost factors used. Spatial Pooler Configuration settings such as InputBits, NumColumns, ExpCellsPerColumn, MinimumOctOverlapCycles,ExpDutyCyclePeriod, ExpLocalAreaDensity, and ExpActivationThreshold that provides insights into the experiment’s setup. 

Additionally properties like  FirstStableCycle, and LastStableCycle offer further details on output stability metrics. By implementing both ITableEntity and IExperimentResult, the class ensures that experiment results are stored and retrieved in an organized, efficient manner, supporting robust data management and analysis.

The Experiment Output properties are defined in ExperimentResult.cs as shown below:
 
```csharp
    public class ExperimentResult : ITableEntity, IExperimentResult
    {
        public ExperimentResult(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string ExperimentId { get; set; }
        public string Name { get; set; }     
        public string Description { get; set; }      
        public DateTime? StartTimeUtc { get; set; }      
        public DateTime? EndTimeUtc { get; set; }       
        public long DurationSec { get; set; }        
        public TimeSpan Duration { get; set; }        
        public string OutputFile { get; set; }       
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double BoostMax { get; set; }
        public double MinimumOctOverlapCycles { get; set; }
        public int InputBits { get; set; }
        public int NumColumns { get; set; }
        public int ExpCellsPerColumn { get; set; }
        public int ExpDutyCyclePeriod { get; set; } 
        public int ExpLocalAreaDensity { get; set; }
        public int ExpActivationThreshold { get; set; }
        public int FirstStableCycle { get; set; }
        public int LastStableCycle { get; set; }
        

    }
```


#### Columns of the Table
Each key-value pair added to the TableEntity corresponds to a column in the Azure Table Storage. Here's a breakdown of the columns:

| **Parameter**                | **Description**                                                                                                                                                        |
|------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **ExperimentId**             | A unique identifier for the experiment. This helps in associating the output results with a specific experiment.                                                        |
| **Name**                     | The name of the experiment. This provides a human-readable identifier for the experiment's results.                                                                     |
| **Description**              | A brief description of the output file or entity related to the experiment. This gives context to what the experiment produced or the nature of the results.            |
| **StartTimeUtc**             | The UTC start time of the experiment. This records when the experiment began, which is important for tracking and analysis.                                              |
| **EndTimeUtc**               | The UTC end time of the experiment. This records when the experiment concluded, helping to calculate the total duration.                                                 |
| **DurationInSec**            | The total duration of the experiment in seconds. This provides a clear metric of how long the experiment took from start to finish.                                      |
| **Ip_MinValue**              | The minimum input value used in the experiment, recorded as part of the output for analysis and verification.                                                                  |
| **Ip_MaxValue**              | The maximum input value used in the experiment, recorded as part of the output for analysis and verification.                                                                  |
| **Ip_MaxBoost**              | The maximum boost value applied during the experiment's learning phase, recorded in the output for assessing its impact.                                                |
| **Ip_MinOctOverlapCycles**   | The minimum overlap cycles recorded during the experiment, particularly during spatial pattern learning, as part of the output for stability analysis.                  |
| **Ip_InputBits**             | The number of input bits used in the experiment, recorded in the output for reference and further analysis.                                                              |
| **Ip_NumColumns**            | The number of columns used in the experiment's grid or structure, recorded in the output to understand the experiment's configuration.                                    |
| **Ip_CellsPerColumn**        | The number of cells per column within the experiment's structure, recorded in the output to understand the depth of each column and its impact on results.               |
| **Ip_DutyCyclePeriod**       | The duty cycle period used during the experiment, recorded in the output to assess its influence on the experiment's dynamics and stability.                             |
| **Ip_LocalAreaDensity**      | The density of active columns within a local inhibition area, recorded as part of the output for understanding sparsity in the experiment's spatial pooler.              |
| **Ip_ActivationThreshold**   | The activation threshold used in sequence learning, recorded in the output to evaluate its role in pattern recognition and learning accuracy.                            |
| **Op_FirstStableCycle**      | The first cycle in which the experiment's output stabilized, marking the beginning of a steady-state phase and recorded for performance analysis.                        |
| **Op_LastStableCycle**       | The last cycle in which the experiment's output was stable, indicating when the experiment began to conclude, recorded for final result analysis.                        |
| **OutputFile**               | The location or link to the output file generated by the experiment, which contains the detailed results or data produced during the experiment.                         |

For visual reference, table can typically viewed in the Azure Storage Explorer or the Azure portal. The structure includes the PartitionKey, RowKey, Timestamp and all input parameters (prefixed with "Ip_") as well as output parameters (prefixed with "Op_").

| PartitionKey | RowKey    | Timestamp               | ExperimentId | Name | Description | StartTimeUtc           | EndTimeUtc             | DurationInSec | Ip_MinValue | Ip_MaxValue | Ip_MaxBoost | Ip_MinOctOverlapCycles | Ip_InputBits | Ip_NumColumns | Ip_CellsPerColumn | Ip_DutyCyclePeriod | Ip_LocalAreaDensity | Ip_ActivationThreshold | Op_FirstStableCycle | Op_LastStableCycle | OutputFile      |
|--------------|-----------|-------------------------|--------------|------|-------------|------------------------|------------------------|---------------|-------------|-------------|-------------|------------------------|--------------|---------------|-------------------|-------------------|---------------------|-----------------------|---------------------|--------------------|-----------------|
| results   | rowKey123 | 2024-08-15T01:00:00.000Z | ML 23/24-03       | The Spatial Learning Experiment | Experiment-1   | 2024-08-15T00:00:00Z    | 2024-08-15T01:00:00Z    | 3600          | 0         | 100        | 10        | 0.001                    | 200          | 1024          | 10                | 1000               | -1                | 15                    | 395                  | 494                 | outputfile     |


[Move to Top](#top)


## Docker Image Creation and Deployment

For the deployment, Docker images were built using Visual Studio, which streamlines the process of containerizing the application. Below are the steps followed to manage Docker images in the deployment process:

### Build Docker Image in Visual Studio

The application was built and packaged into a Docker image directly within Visual Studio, taking advantage of its integrated tools for Docker support.

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/Docker%20Build%20in%20Vs.png">
  <br>
  <em>Figure 3: <i>Build Docker Image in Visual Studio</i></em>
</p>

### List Docker Images

To view the list of Docker images available locally, use the following command:

```bash
docker images
``` 

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/docker%20images.png">
  <br>
  <em>Figure 4: <i>List docker images using command line</i></em>
</p>

### Tag the Docker Image Locally

```bash
docker tag mycloudproject:latest anushruthpal/thelazycoders:v1
```

### Push the Image to Docker Hub

```bash
docker push anushruthpal/thelazycoders:v1
```

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/Docker%20Hub.png">
  <br>
  <em>Figure 5: <i>Docker image in Docker Hub repository</i></em>
</p>

### Log in to the Azure Container Registry (ACR)
```bash
az acr login -n thelazycodersregistry.azurecr.io
```

### Tag the Docker Image for the Azure Container Registry (ACR)
```bash
docker tag mycloudproject:latest thelazycodersregistry.azurecr.io/mycloudproject1:v3
```

### Push the Image to the Azure Container Registry (ACR)
```bash
docker push thelazycodersregistry.azurecr.io/mycloudproject1:v3
```

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/Docker%20Image%20in%20ACR.png">
  <br>
  <em>Figure 6: <i>Docker image in Azure Container Registry(ACR)</i></em>
</p>

For a complete reference of all commands executed during the Docker image creation and deployment process, please refer to the commands in the [reference file](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Docker%20Deployment%20Commands.txt).

By building the Docker image in Visual Studio and pushing it to the Azure Container Registry, the application is prepared for deployment in Azure Container Instances. This approach leverages Visual Studio’s capabilities to streamline the development and deployment workflow, ensuring that the containerized application is consistently available and scalable within the Azure ecosystem.

[Move to Top](#top)

## Project Execution

In this section, we provide a comprehensive guide on how to execute the project within the Azure environment. This includes detailed steps on setting up and configuring the necessary Azure resources, initiating the experiment, and monitoring its progress. 

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/process%20flow%20diagram.jpg">
  <br>
  <em>Figure 7: <i>Process flow diagram of Azure Cloud Implementation: ML23/24-03 Implement the New Spatial Learning Experiment</i></em>
</p>

Follow the instructions below to ensure a smooth and successful execution of the spatial learning experiment using Azure's cloud services.

### 1. Starting the Azure Container Instance
In the first step, you will start the Azure Container Instance named thelazycoderscontainerinstance. This container instance is responsible for executing the spatial learning experiment. To begin, navigate to the Azure Portal or use the Azure CLI to start the container instance, ensuring that it is properly configured and ready for operation. This step initializes the environment where the experiment will be conducted.



<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/start.png">
  <br>
  <em>Figure 8: <i>Start the Azure Container Instance(ACI)</i></em>
</p>



### 2. Initializing the project with Queue Message 
This experiment is designed to be triggered automatically through messages sent to an Azure Queue. The system continuously monitors the queue for incoming messages, which contain the necessary parameters to start the experiment. Each message includes a trigger signal, along with  parameters that define the range for a random input generator used in the experiment and other spatial pooler parameters. These messages can be sent using either the Azure Portal or Azure Storage Explorer, providing flexibility in how to manage and automate the execution of your experiments. Once the message is received, the experiment begins immediately, utilizing the specified values for its random input generation process and configuring the spatial pooler with the input parameters.

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/tigger-queue%20messaged%20received.png">
  <br>
  <em>Figure 9: <i>Initializing the project with trigger-queue Message </i></em>
</p>



### 3. Monitoring Experiment Status with Azure Container Instances Logs
The logs generated by Azure Container Instances (ACI) play a crucial role in monitoring the real-time status of the running project. These logs provide detailed insights into the execution flow, helping to track the progress of your experiment, diagnose issues, and verify that the system is functioning as expected. The logs capture various events, including the receipt of queue messages, the initiation of the experiment, and the random input generation based on the MinValue and MaxValue parameters result storage and experiment termination. By regularly reviewing these logs, one can ensure that the experiment runs smoothly, identify any errors or anomalies, and make informed decisions on potential adjustments or troubleshooting steps. The logs are accessible directly through the Azure Portal, making it convenient to stay informed about your project's status without needing additional monitoring tools.



<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/exp%20started%20again%20after%201st%20run%20with%20new%20queue.png">
  <br>
  <em>Figure 10: <i>Experiment Logs Azure Container Instances</i></em>
</p>

### 4. Experiment Output
Once the Experiment execution is complete, the results can be reviewed using either the Azure Portal or Azure Storage Explorer. The output from the Experiment is stored in the outputfile Blob container named result-files under the filename output-(random GUID).txt with, as illustrated in Figure 11. This file contains the final results of the experiment, providing a comprehensive summary of the execution. Additionally, an output table named results is created to present the output results in a structured format, facilitating easier analysis and interpretation, as shown in Figure 12. By accessing these resources through the Azure Portal or Azure Storage Explorer, one can efficiently retrieve and examine the outcomes of your Experiment, ensuring that  all necessary data for evaluation and reporting are accurate.


<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/output-result-files.png">
  <br>
  <em>Figure 11: <i>Output files in Blob Container(results-files)</i></em>
</p>

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/output-results%20table%20entries.png">
  <br>
  <em>Figure 12: <i>Output table (results) in Azure Tables</i></em>
</p>


### 5. Stopping the Azure Container Instances After Project Execution

Once the project execution is complete and you have retrieved the output data, it is advisable to stop the Azure Container Instance (ACI) to conserve Azure resources and maintain system efficiency. Stopping the ACI helps prevent unnecessary costs and frees up resources that can be allocated to other tasks or projects. We can stop the container instance directly through the Azure Portal or Azure CLI. This practice ensures that your system remains cost-effective and that resources are managed effectively, promoting overall operational efficiency.

<p align="center">
  <img src="https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2023-2024/blob/TheLazyCoders/Source/MyCloudProjectSample/Documentation/Images/stop.png">
  <br>
  <em>Figure 13: <i>Stopping the Azure Container Instances</i></em>
</p>


[Move to Top](#top)

## Conclusion


The Cloud Implementation effectively utilizes Azure's cloud services to conduct a Spatial learning Experiment in conjunction with the Neocortex API. This integration highlights how cloud technology can enhance machine learning projects by providing a scalable and efficient platform. The system manages tasks such as receiving trigger messages, generating Sparse Distributed Representations (SDRs) based on input parameters, and storing experiment results in Azure Blob Storage and Azure Table Storage. By leveraging Azure's cloud infrastructure and Docker, the project achieves superior scalability and performance, efficiently handling complex computations and experiments. The use of Blob Storage ensures secure and scalable storage of output files, while Table Storage facilitates the structured and efficient storage of experiment data. This setup not only demonstrates the effectiveness of cloud services in executing large-scale experiments but also paves the way for future advancements and more sophisticated analyses as technology evolves.

### References
1. [Neocortex API](https://github.com/ddobric/neocortexapi)
2. [Microsoft Azure Documentation]( https://learn.microsoft.com/en-us/azure/?product=popular​)



