using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Newtonsoft.Json;
using Amazon.XRay.Recorder.Handlers.AwsSdk;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LeaveManagementAPI
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set
        // the name of the DynamoDB table used to store blog posts.
        const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "LeaveTable";
        IDynamoDBContext DDBContext { get; set; }


        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            AWSSDKHandler.RegisterXRayForAllServices();

            // Check to see if a table name was passed in through environment variables and if so 
            // add the table mapping.
            var tableName = System.Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Leave)] = new Amazon.Util.TypeMapping(typeof(Leave), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
        }

        /// <summary>
        /// Constructor used for testing passing in a preconfigured DynamoDB client.
        /// </summary>
        /// <param name="ddbClient"></param>
        /// <param name="tableName"></param>
        public Functions(IAmazonDynamoDB ddbClient, string tableName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Leave)] = new Amazon.Util.TypeMapping(typeof(Leave), tableName);
            }

            var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            this.DDBContext = new DynamoDBContext(ddbClient, config);
        }

        /// <summary>
        /// A Lambda function that returns back a page worth of Leave requests.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of Leave Requests</returns>
        public async Task<APIGatewayProxyResponse> GetLeaveRequestsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Getting Leave Requests");

            var search = this.DDBContext.ScanAsync<Leave>(null);
            var page = await search.GetNextSetAsync();

            context.Logger.LogLine($"Found {page.Count} blogs");

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = JsonConvert.SerializeObject(page),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };

            return response;
        }

        /// <summary>
        /// A Lambda function that adds a blog post.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> AddOrUpdateLeaveRequestAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var leave = JsonConvert.DeserializeObject<Leave>(request?.Body);
            leave.LeaveID = leave.LeaveID ?? Guid.NewGuid().ToString();

            context.Logger.LogLine($"Saving Leave Request with id {leave.LeaveID}");

            await DDBContext.SaveAsync<Leave>(leave);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = leave.LeaveID.ToString(),
                Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            };

            context.Logger.LogLine("Save complete");
            return response;
        }
    }
}
