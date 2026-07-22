# =============================================================================
# Global Variables - Required for Providers
# =============================================================================
variable "cors_origins" {
  description = "List of allowed CORS origins for API Gateway"
  type        = list(string)
  default     = ["http://localhost:3000"]
}

# =============================================================================
# Doppler Configuration (Required)
# =============================================================================

variable "DOPPLER_TOKEN" {
  description = "Doppler service token for accessing secrets (set via TF_VAR_doppler_token)"
  type        = string
  sensitive   = true
}

variable "doppler_project" {
  description = "Doppler project name"
  type        = string
  default     = "socialmedia"
}

variable "doppler_config" {
  description = "Doppler config name (dev, staging, production)"
  type        = string
  default     = "dev"
}

# =============================================================================
# Provider Configuration Variables
# =============================================================================
# These are needed for provider blocks which cannot use data sources
# Set via environment variables or they'll use defaults

variable "aws_region" {
  description = "AWS region (can be overridden via TF_VAR_aws_region, otherwise uses Doppler value)"
  type        = string
  default     = "ap-southeast-1"
}

variable "environment" {
  description = "Environment name (can be overridden via TF_VAR_environment, otherwise uses Doppler value)"
  type        = string
  default     = "dev"
}

variable "project_name" {
  description = "Project name (can be overridden via TF_VAR_project_name, otherwise uses Doppler value)"
  type        = string
  default     = "socialmedia"
}

variable "AWS_ACCESS_KEY_ID" {
  description = "AWS access key ID (pulled from Doppler via TF_VAR_AWS_ACCESS_KEY_ID)"
  type        = string
  sensitive   = true
}

variable "AWS_SECRET_ACCESS_KEY" {
  description = "AWS secret access key (pulled from Doppler via TF_VAR_AWS_SECRET_ACCESS_KEY)"
  type        = string
  sensitive   = true
}

variable "UPSTASH_EMAIL" {
  description = "Upstash account email (pulled from Doppler via TF_VAR_UPSTASH_EMAIL)"
  type        = string
}

variable "UPSTASH_API_KEY" {
  description = "Upstash API key (pulled from Doppler via TF_VAR_UPSTASH_API_KEY)"
  type        = string
  sensitive   = true
}

variable "lambda_package_path" {
  description = "Path to the zipped Lambda deployment package"
  type        = string
  default     = "../artifacts/BasicSocialMedia.API.zip"
}

variable "lambda_handler" {
  description = "Lambda handler from the published .NET package"
  type        = string
  default     = "BasicSocialMedia.API"
}

variable "github_repo" {
  description = "GitHub repository allowed to deploy, in org/repo format"
  type        = string
  default     = "CupOfMakiato/basic-social-media"
}
