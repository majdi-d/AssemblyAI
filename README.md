
# Speech-to-Text with AssemblyAI and AWS  

This repository demonstrates a serverless solution for performing speech-to-text using **AssemblyAI**, **AWS Lambda**, **SQS**, and **Blazor Web** as the front-end. The project leverages AssemblyAI’s advanced transcription capabilities integrated with AWS cloud technologies to provide an efficient and scalable transcription service.  

## Features  
- **Speech-to-Text Conversion**: Uses AssemblyAI's API for high-accuracy transcription of uploaded audio files.  
- **Serverless Architecture**: Implements AWS Lambda for processing audio files and storing transcription results.  
- **Blazor Front-End**: User-friendly web interface for uploading audio files and viewing transcription results.  
- **Scalable Design**: Utilizes AWS SQS for asynchronous communication and handles high workloads seamlessly.  
- **Rich Metadata**: Extracts additional insights such as confidence scores, word count, sentiment analysis, and speaker identification.  

---

## Architecture  

The solution is designed with the following components:  

1. **Blazor Web Front-End**  
   - Allows users to upload audio files and view transcription results dynamically.  

2. **AWS Lambda Function**  
   - Triggered by S3 events to process uploaded audio files via AssemblyAI API.  
   - Saves transcription data to DynamoDB and S3 for further use.  

3. **SQS Integration**  
   - Acts as a bridge between AWS Lambda and the Blazor application to handle message-based communication.  

4. **DynamoDB and S3 Buckets**  
   - **DynamoDB**: Stores transcription metadata for quick access.  
   - **S3 Buckets**: Stores raw audio files and JSON-formatted transcription results.  

---

## Deployment  

This project is deployed using **Terraform** for Infrastructure-as-Code. It provisions the following AWS resources:  
- **Source and Target S3 Buckets**: For storing raw audio and processed transcription files.  
- **SQS Queue**: Ensures decoupled and scalable communication.  
- **DynamoDB Table**: Stores transcription metadata, such as text, timestamps, and confidence scores.  
- **IAM Policies and Roles**: Secure access to AWS services.  
- **AWS Lambda Function**: Processes audio files using AssemblyAI.  
- **ECR Repository**: Stores Docker images for containerized deployments.  
- **ECS and Task Definitions**: Runs front-end services on AWS Fargate.  
- **Application Load Balancer**: Exposes the Blazor front-end securely.  

---

## Setup  

### Prerequisites  
1. **AWS Account**: Ensure you have access to AWS services like S3, Lambda, and DynamoDB.  
2. **AssemblyAI API Key**: Register at [AssemblyAI](https://www.assemblyai.com/) to obtain an API key.  
3. **Terraform**: Install Terraform for deploying the infrastructure.  
4. **.NET SDK**: Install the .NET SDK for Blazor and Lambda function development.  

---

### Deployment Steps  
1. **Clone the Repository**  
   ```bash  
   git clone https://github.com/your-username/speech-to-text-assemblyai.git  
   cd speech-to-text-assemblyai  
   ```  

2. **Configure AssemblyAI API Key**  
   - Add your AssemblyAI API key as an environment variable for Lambda.  

3. **Deploy Infrastructure with Terraform**  
   ```bash  
   cd terraform  
   terraform init  
   terraform apply  
   ```  

4. **Build and Deploy Lambda Function**  
   ```bash  
   cd src/LambdaFunction  
   dotnet build  
   dotnet lambda deploy-function SpeechToTextProcessor  
   ```  

5. **Run the Front-End**  
   ```bash  
   cd src/BlazorApp  
   dotnet run  
   ```  

---

## Usage  

1. **Upload Audio Files**: Drag and drop or browse to select an audio file in the Blazor web app.  
2. **View Results**: After processing, transcription results and metadata will be displayed in the UI.  

---

## Screenshots  

### Front-End  
![Front-End UI](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/bzcbtefy4801omr1rzhp.png)  

### DynamoDB Table  
![DynamoDB Table](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/szdvakcgwhw36c3d4tpr.png)  

### Lambda Logs  
![Lambda Logs](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/hfg1z70trcou5umbwp3d.png)  

### Transcription JSON  
![Transcription JSON](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/z9mk5kggm5c885pah12p.png)  

---

## Contributing  
Contributions are welcome! Feel free to fork the repository, create a feature branch, and submit a pull request.  

---

## License  
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.  

---

## Acknowledgements  
- [AssemblyAI](https://www.assemblyai.com/) for their powerful transcription API.  
- [AWS](https://aws.amazon.com/) for the serverless infrastructure and tools.  

---  

## Links  
- [Lambda Function Repository](https://github.com/majdi-d/AssemblyAI)  
- [Front-End Repository](https://github.com/majdi-d/BlazorApp1)  
- [Connect on LinkedIn](https://www.linkedin.com/in/majdidhissi/)  

---  

Thank you for exploring this project! If you find it helpful, please give it a star ⭐ and share your feedback!  
