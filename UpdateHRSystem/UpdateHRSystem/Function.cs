using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace UpdateHRSystem
{
    public class Function
    {
        /// <summary>
        /// Mock HR System update API returns "Approved" or "Rejected" based on odd or even minute.
        /// </summary>
        /// <param name="input">LeaveID</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            //Ideally, the function would read the DynamoDB table with the LeaveID and update the HR system. 
            //This is only a mock API to mimick a HR system update and enable the Step Function workflow.
            return "HR System updated";
        }
    }
}
