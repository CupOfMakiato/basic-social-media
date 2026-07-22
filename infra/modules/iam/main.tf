resource "aws_iam_openid_connect_provider" "github_actions" {
  url = "https://token.actions.githubusercontent.com"

  client_id_list = [
    "sts.amazonaws.com",
  ]

  tags = {
    Name        = "${var.project_name}-${var.environment}-github-actions"
    Environment = var.environment
    Purpose     = "GitHub Actions OIDC"
  }
}

resource "aws_iam_policy" "github_actions_lambda_deploy" {
  name        = "${var.project_name}-${var.environment}-github-actions-lambda-deploy"
  description = "Allows GitHub Actions to deploy the API Lambda zip"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Sid    = "DeployApiLambdaZip"
        Effect = "Allow"
        Action = [
          "lambda:UpdateFunctionCode",
          "lambda:GetFunction",
          "lambda:GetFunctionConfiguration"
        ]
        Resource = var.lambda_function_arn
      }
    ]
  })

  tags = {
    Name        = "${var.project_name}-${var.environment}-github-actions-lambda-deploy"
    Environment = var.environment
  }
}

resource "aws_iam_role" "github_actions_lambda_deploy" {
  name        = "${var.project_name}-${var.environment}-github-actions-lambda-deploy"
  description = "Role for GitHub Actions to deploy the API Lambda zip"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Federated = aws_iam_openid_connect_provider.github_actions.arn
        }
        Action = "sts:AssumeRoleWithWebIdentity"
        Condition = {
          StringEquals = {
            "token.actions.githubusercontent.com:aud" = "sts.amazonaws.com"
            "token.actions.githubusercontent.com:sub" = "repo:${var.github_repo}:ref:refs/heads/main"
          }
        }
      }
    ]
  })

  tags = {
    Name        = "${var.project_name}-${var.environment}-github-actions-lambda-deploy"
    Environment = var.environment
    Purpose     = "GitHub Actions Lambda deploy"
  }
}

resource "aws_iam_role_policy_attachment" "github_actions_lambda_deploy" {
  role       = aws_iam_role.github_actions_lambda_deploy.name
  policy_arn = aws_iam_policy.github_actions_lambda_deploy.arn
}
