## AWS Serverless Workshop Decoupled Architecture

A sample .NET web application based on decoupled architecture design principles.

## License Summary

The documentation is made available under the Creative Commons Attribution-ShareAlike 4.0 International License. See the LICENSE file.

The sample code within this documentation is made available under the MIT-0 license. See the LICENSE-SAMPLECODE file.

# Overview
Create a fully decoupled, microservices architecture based Leave Management application using a variety of AWS Services such as, 
* Amazon ECS
* Amazon Elastic Container Registry
* AWS Lambda
* Amazon Elastic Load Balancer
* AWS X-Ray
* AWS Step Functions
* Amazon DynamoDB
* Amazon Simple Notification Service (SNS)
* Amazon API Gateway
* AWS CloudWatch

# Application Architecture
![System Architecture](/images/DecoupledArchitectureDiagram.png)
## The application supports the following functionalities

* Submit Leave Request
* Approve / Reject Leave Request
* List Leave Requests
* Notify status to target audience

## Following are the major parts of the application

* Web front end - Serves the user interface 
* Microservices  - Serves business functionalities 
* WorkFlow services - Performs back-end business operations

### The following diagram shows the request flow between the components for a new leave request workflow

![Leave Request Workflow](/images/DecoupledArchitectureDiagram-Workflow.png)

## Setup Instructions
#### NOTE
> This is a Level 200-300 workshop and the expectation is that you are already aware of using the AWS console, basic AWS and cloud concepts such as Cloudformation, Serverless applications, API management, Load Balancers, Workflows, .NET development & Visual Studio. 

### Prerequisites / Environment setup
> I will use US-EAST-1 AWS region for all instructions here. Replace it with an appropriate region if you are using one other than US-EAST-1

* Install Visual Studio 2019 or 2017 Community edition. 
    *  Download from here - https://visualstudio.microsoft.com/vs/community/ 
* Install and configure AWS Tools for Visual Studio.
    *  Download from here - https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2017
* Install and configure AWS Tools for PowershellCore.
    *  Download and configure from here - 
https://docs.aws.amazon.com/powershell/latest/userguide/pstools-getting-set-up-windows.html
* Clone this GitHub repo
    * https://github.com/awsimaya/DecoupledArchitecture
* Install Docker for Windows
    * Download from here - https://docs.docker.com/docker-for-windows/install/
### Create Workflow services
#### Create Amazon SNS Topic

> This SNS topic is used to notify users of Leave status changes

* Login to AWS console and navigate to **Amazon SNS** topic
* Enter **LeaveApproval** in the **MyTopic** textbox. Click **Next step**
* On the next page, click **Create topic**
* Copy and paste the **ARN** of the newly created SNS topic into a text editor
* Click on **LeaveApproval** link
* Under **Subscriptions**, click **Create subscription**
* Select **Email** under **Protocol** dropdown
* Enter your email address and click **Create subscription**
* You will receive a new email from **AWS Notifications** containing a link to confirm the subscription. Click **Confirm Subscription** link on the email

#### Create UpdatePayRoll Lambda Function
> This is a mock Lambda function that mimics a fake Payroll system update

* Open Visual Studio
* Open UpdatePayRoll.sln solution from UpdatePayRoll folder in this repo
* Right click on the project file and choose **Publish to AWS Lambda**
* In the wizard that pops up, choose a region you want the solution to be deployed to and click **Next**
* Select **AWSLambdaFullAccess** role for the **Role Name** dropdown and click **Upload**
* Your Lambda application is now deployed to your AWS environment
* Navigate to the newly created Lambda function home page on the AWS console
* Copy the ARN for Lambda from the top right of the screen and paste into a text editor

#### Create UpdateHRSystem Lambda Function

> This is a mock Lambda function that mimics a fake HR system update

* Open Visual Studio
* Open **UpdateHRSystem.sln** solution from **UpdateHRSystem** folder in this repo
* Right click on the project file and choose **Publish to AWS Lambda**
* In the wizard that pops up, choose a region you want the solution to be deployed to and click **Next**
* Select **AWSLambdaFullAccess** role for the **Role Name** dropdown and click **Upload**
* Your Lambda application is now deployed to your AWS environment
* Navigate to the newly created Lambda function home page on the AWS console
* Copy the ARN for Lambda from the top right of the screen and paste into a text editor

#### Create LeaveApprovalWorkflow Step Function
* Open **workflow.json** from **StepFunctionWorkFlow** folder
* Replace **<LAMBDA_ARN_FOR_UpdateHRSystem>** with the ARN you saved into the text editor in **Create UpdateHRSystem Lambda Function** step
* Replace **<LAMBDA_ARN_FOR_UpdatePayRoll>** with the ARN you saved into the text 
editor in **Create UpdatePayRoll Lambda Function** step
* Replace **<SNS_TOPIC_ARN>** with the ARN you saved into the text editor in **Create Amazon SNS Topic** step
* Login to AWS console
* Navigate to AWS Step Functions home page by typing **Step Functions** on the search bar and selecting the first result
* Ensure **Author with code snippets** option is selected. Enter **LeaveApprovalWorkflow** in the **NAME** textbox
* Copy all the content from **workflow.json** file and paste it into the **State machine definition** textbox
* Ensure **Create an IAM role for me** option is selected. Enter **leaveapprovalworkflowrole** in the **Name** textbox
* Click **Create state machine**
* Your step function is created successfully
* Copy and paste the ARN of the Step Function you just created to a text editor for later usage

### Create Microservices

>In this section we will create the Lambda functions used in processing requests from the web front end, API Gateway endpoints and a DynamoDB table to store Leave data

#### Create LeaveManagementAPIs
We will be creating the following resources as a result of this section

 * A DynanmoDB table named **LeaveRequests**
 * A Lambda function called **AddOrUpdateLeaveRequest**
 * A Lambda function called **GetLeaveRequests**
 * An API Gateway API called **LeaveManagementAPI**

##### Steps

* Open **LeaveManagementAPI.sln** from **LeaveManagementAPI** folder
* Right click on **LeaveManagementAPI** project and click **Publish to AWS Lambda...**
* On the newly opened wizard, Enter **LeaveManagementAPI** in the **Stack Name** textbox
* Ensure **LeaveRequests** is the value of the **LeaveTableName** textbox
* Select **true** for **ShouldCreateTable** textbox
* Click **Publish**
* Visual Studio will open a new screen called **Stack: LeaveManagementAPI** to show the events taking place in creating the stack
* Once you see **CREATE_COMPLETE** in green color, copy the URL from **AWS Serverless URL** and paste into a text editor
* Navigate to DynamoDB home page on AWS console and select **LeaveRequests** table under **Tables**
* Copy the **Latest stream ARN** value (shown in the picture below) and paste it into a text editor for later usage

![DynamoDB setting](/images/DynamoDB.png)

* Navigate to API Gateway home page on the AWS console and select **LeaveManagementAPI** API
* Select **Stages** and select **Prod** 
* Under **Logs/Tracing** tab, check **Enable CloudWatch Logs** and **Enable X-Ray Tracing** checkboxes as shown below

![API Gateway XRay setting](/images/APIGatewayXRay.png)

* Open **LeaveRequestProcessor.sln** solution under **LeaveRequestProcessor** folder
* In **Function.cs** file, replace **<SNS_TOPIC_ARN>** with the ARN you saved into the text editor in **Create Amazon SNS Topic** step
* In **Function.cs** file, replace **<STEP_FUNCTION_STATEMACHINE_ARN>** with the ARN you saved into the text editor in **Create LeaveApprovalWorkflow Step Function** step
* In **serverless.template** file, replace **<DYNAMO_DB_STREAM_ARN>** with the DynamoDB stream ARN you created earlier
* Right click on **LeaveRequestProcessor** project and select **Publish to AWS Lambda...**
* In the publish wizard, enter **LeaveRequestProcessor** in the **Stack Name** textbox
* Enter any name for the S3bucket in the **S3 Bucket** text box
* Click **Publish**
* Visual Studio will open a new window to show the progress of the deployment and will show **CREATE_COMPLETE** once successfully deployed

### Create Web front end

> In this section we will create the web front end that will help users create and manage leave approval requests

#### Create a new IAM role for ECS task execution
> In this section, we will create a new IAM role for the ECS Service you will create to assume
* Open AWS console and navigate to **IAM** -> **ROLES** 
* Click **Create role**
* In the next screen, select **AWS service** for **Select type opf trusted identity**
* Select **Elastic Container Service** under  **Choose the service that will use this role**
* Select **Elastic Container Service Task** under  **Select your use case**. Click **Next: Permissions**
* In the next screen, select **AmazonECSTaskExecutionRolePolicy** policy, click **Attach policy** and click **Next: Tags**
* Click **Next** in the next page
* Enter **ecsTaskExecutionRole** as name in the next page. Click **Create role**
* Go back to **ecsTaskExecutionRole**, click **Add Inline policy** and go to **JSON** tab
* Copy and paste the following JSON into the textarea
```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "xray:PutTraceSegments",
                "xray:PutTelemetryRecords"
            ],
            "Resource": [
                "*"
            ]
        }
    ]
}
```
* Click **Review policy**
* Name the policy as **PushtoXRayPol** and click **Create policy**

#### Publish web app to Amazon ECS
> In this section, we will publish the Web application to Amazon ECS
* Open **LeaveWebApp.sln** file from the **LeaveWebApp** folder
* In **HomeController.cs**, replace **<API_GATEWAY_URL>** with the API Gateway URL you saved in step **Create LeaveManagementAPIs**
* Right click on **LeaveWebApp** project and click **Publish Container to AWS...**
* In the wizard, enter **leavewebapp** in **Docker Repository** field
* Select **Service on ECS Cluster** option (as shown below) and click **Next**
![PublishWebApp](/images/PublishWebApp.png)
* In the next window, select **Create an empty cluster** and enter **LeaveWebApp** in the textbox next to the dropdown. Click **Next**
* In the next window, select **Create New** for **Service** and enter **LeaveWebApp** in the textbox next to dropdown. Click **Next**
* In the next window, check **Configure Application Load Balancer** check box, select **Create New** for  **Load Balancer:** dropdown and enter **LeaveWebApp** in the textbox next to it.Click **Next**
![PublishWebApp](/images/PublishWebApp2.png)
* In the next window, select **Create New** for **Task Definition** dropdown and enter **LeaveWebApp** in the textbox next to it.
* Select **Create New** for **Container** dropdown and enther **LeaveWebApp** in the textbox next to it.
* Select **Existing role: ecsTaskExecutionRole** for **Task Role:** dropdown
* Select **ecsTaskExecutionRole** for **Task Execution Role** dropdown
* Click **Publish**
* Visual Studio will now start publishing the application environment
* You just published the Web application to Amazon ECS
* Once complete, you will be able to see a new ECS service called **LeaveWebApp** on the AWS console

#### Configure XRay for the ECS Service
> In this section, we will create a X-Ray daemon container which will run as a side-car container  along with the LeaveWebApp container to process and send X-Ray data to the X-Ray service on AWS.

> More about this topic here - https://docs.aws.amazon.com/xray/latest/devguide/xray-daemon-ecs.html

* Open PowerShell and execute the following commands. The first command pulls the **aws-xray-daemon** container image from docker hub. The second command tags the image as **xraydaemon**
```docker
docker pull amazon/aws-xray-daemon
docker tag amazon/aws-xray-daemon:latest xraydaemon:latest
```
* Login to AWS console and navigate to **Amazon ECS**. Click **Repositories** on the left navigation section
* Click **Create repository** and name it **xraydaemon**
* Click **Create repository** 
* Click on **View push commands** on the top right of the screen
* Choose Windows or macOS/Linux tab and execute the command from Step 1. The command will return a very long URL. Copy and paste the URL into the command line and press enter.
* Skip the step 2 command, because we already have the container image downloaded from docker hub earlier. Execute commands from Step 3 and Step 4.
* Once complete, go to the **xraydaemon** repository on the AWS ECR home page and ensure the new image has been published successfully
* Navigate to **Task Definitions** on **Amazon ECS** home page and click **LeaveWebApp**
* Select the latest version and click **Create new revision**
* Under **Container Definitions** click **Add container**
* Name the container **xray-daemon**
* Copy and paste the container image URI from **xraydaemon** ECR repo into the **Image** textbox
* Click **Add** button at the bottom
* Select the newly created version of the Task Definition
* Select **Actions** -> **Update Service**
* Select **LeaveWebApp** in the **Cluster** dropdown
* Check **Force new deployment** checkbox
* Click **Next step** in the next few screens until you get to **Review** screen. Click **Update Service**
* You just finished deploying the Leave Web App to Amazon ECS 

### Test drive the application
* Copy and paste the DNS name of the **LeaveWebApp** ALB from the EC2 home page on AWS console into a browser and press enter
* You should be able to see the application as shown below
![LeaveWebAppHome](/images/LeaveWebAppHome.png)
* You can Submit a Leave Request and also Approve a Leave Request using this application by clicking on the appropriate links.
* Once you submit a Leave Request, you will see an email arrive in your email inbox that you configured earlier
* Also another email will land in your email inbox once you approve a request as well
* After you play around with the application for a while, you can navigate to X-Ray home page on AWS console and click **Service map**
* You should be able to see the service map as shown below
![Xray](/images/X-Ray.png)
