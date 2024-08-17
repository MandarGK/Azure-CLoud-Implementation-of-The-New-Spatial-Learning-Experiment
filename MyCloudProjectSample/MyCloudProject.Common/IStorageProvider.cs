using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCloudProject.Common
{
    /// <summary>
    /// Defines the contract for all storage operations.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Receives the next message from the queue.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>NULL if there are no messages in the queue.</returns>
        // IExerimentRequest ReceiveExperimentRequestAsync(CancellationToken token);
        Task<IExerimentRequest> ReceiveExperimentRequestAsync(CancellationToken token);

        
        /// <summary>
        /// Uploads the result of the experiment in the cloud or any other kind of store or database.
        /// </summary>
        /// <param name="experimentName">The name of the experiment at the remote (cloud) location. The operation creates the file with the name of experiment.</param>
        /// <param name="result">The result of the experiment that should be uploaded to the table.</param>
        /// <remarks>See step 5 (oposite way) in the architecture picture.</remarks>
        Task UploadResultAsync(string experimentName, IExperimentResult result);

        /// <summary>
        /// Uploads the result of an experiment to a specified storage location.
        /// </summary>
        /// <param name="result">The result of the experiment that should be uploaded.</param>
        /// <remarks>This method handles the process of uploading experiment results to a cloud storage or other storage systems.</remarks>
        Task UploadExperimentResult(IExperimentResult result);


        /// <summary>
        /// Makes sure that the message is deleted from the queue.
        /// </summary>
        /// <param name="request">The requests received by <see cref="nameof(IStorageProvider.ReceiveExperimentRequestAsync)"/>.</param>
        /// <returns></returns>
        Task CommitRequestAsync(IExerimentRequest request);

    }
}
