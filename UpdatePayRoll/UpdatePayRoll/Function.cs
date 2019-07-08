using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace UpdatePayRoll
{
    public class Function
    {
        
        /// <summary>
        /// Mock function to update PayRoll system
        /// </summary>
        /// <param name="input">LeaveID</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            // Just returns "Updated" to mimic a PayRoll system update and help the Step Function workflow
            return "PayRoll Updated";
        }
    }
}
