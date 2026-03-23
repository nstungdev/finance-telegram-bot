# AWS Lambda CI/CD (GitHub Actions)

This repository uses the workflow at `.github/workflows/deploy-lambda.yml`.

## Trigger Rules
- Auto deploy on push to `master`.
- Only runs when files under `src/**` are changed.
- Manual trigger is also supported via `workflow_dispatch`.

## Required GitHub Secrets
Add these repository secrets before first deployment:

| Secret | Description | Example |
|--------|-------------|---------|
| `AWS_ROLE_TO_ASSUME` | IAM role ARN for GitHub OIDC | `arn:aws:iam::123456789012:role/github-actions-lambda-deploy` |
| `AWS_REGION` | AWS region | `ap-southeast-1` |
| `TELEGRAM_BOT_TOKEN` | Telegram Bot API token | `123456:ABC-DEF` |
| `TELEGRAM_DEFAULT_CHAT_ID` | Default chat ID for fallback replies | `6330844503` |

## What the Workflow Does
1. Restores and publishes the .NET API project.
2. Deploys infrastructure via SAM (`template.yaml`): Lambda function + HTTP API Gateway.
3. Reads the API Gateway endpoint from CloudFormation outputs.
4. Calls Telegram `setWebhook` to register the endpoint automatically.

## Infrastructure (SAM Template)
The `template.yaml` at the repository root defines:
- **AWS::Serverless::Function** — Lambda function running the ASP.NET API.
- **HttpApi events** — API Gateway HTTP API with `ANY /{proxy+}` catch-all route.
- **Parameters** — `TelegramBotToken` and `TelegramDefaultChatId` injected as environment variables.

## Lambda App Setup
The API project enables Lambda HTTP API hosting in `Program.cs`:

```csharp
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, 
    new SourceGeneratorLambdaJsonSerializer<AppJsonSerializerContext>());
```

This uses the AOT-safe source-generated serializer to avoid reflection.

## First-Time OIDC Setup
To allow GitHub Actions to assume an AWS role, configure OIDC trust once:
1. Create an IAM OIDC provider for `token.actions.githubusercontent.com`.
2. Create an IAM role with trust policy allowing your repo, and attach permissions for CloudFormation, Lambda, API Gateway, IAM, and S3.
3. Store the role ARN as `AWS_ROLE_TO_ASSUME` secret.
