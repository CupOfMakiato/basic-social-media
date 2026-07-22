variable "project_name" {
  description = "Name of the project, used for resource naming"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "github_repo" {
  description = "GitHub repository allowed to assume the deploy role, in org/repo format"
  type        = string
}

variable "lambda_function_arn" {
  description = "API Lambda ARN the deploy role can update"
  type        = string
}
