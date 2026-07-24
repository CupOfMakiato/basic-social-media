# =============================================================================
# Cognito Module - Variable Declarations
# =============================================================================

variable "project_name" {
  description = "Name of the project, used for resource naming"
  type        = string
}

variable "environment" {
  description = "Environment name (e.g., dev, staging, production)"
  type        = string
}

variable "aws_region" {
  description = "AWS region where resources will be created"
  type        = string
}

variable "frontend_url" {
  description = "Frontend origin for OAuth callbacks"
  type        = string
  default     = "http://localhost:3000"
}

variable "google_client_id" {
  description = "Google OAuth client ID"
  type        = string
  sensitive   = true
  default     = null
  nullable    = true
}

variable "google_client_secret" {
  description = "Google OAuth client secret"
  type        = string
  sensitive   = true
  default     = null
  nullable    = true
}

variable "test_admin_email" {
  description = "Email address for the test administrator"
  type        = string

  validation {
    condition     = can(regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", var.test_admin_email))
    error_message = "test_admin_email must be a valid email address."
  }
}

variable "test_admin_password" {
  description = "Permanent password for the test administrator"
  type        = string
  sensitive   = true

  validation {
    condition = (
      length(var.test_admin_password) >= 8 &&
      can(regex("[A-Z]", var.test_admin_password)) &&
      can(regex("[a-z]", var.test_admin_password)) &&
      can(regex("[0-9]", var.test_admin_password))
    )
    error_message = "test_admin_password must be at least 8 characters and include upper-case, lower-case, and numeric characters."
  }
}

variable "test_user_email" {
  description = "Email address for the test user"
  type        = string

  validation {
    condition     = can(regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", var.test_user_email))
    error_message = "test_user_email must be a valid email address."
  }
}

variable "test_user_password" {
  description = "Permanent password for the test user"
  type        = string
  sensitive   = true

  validation {
    condition = (
      length(var.test_user_password) >= 8 &&
      can(regex("[A-Z]", var.test_user_password)) &&
      can(regex("[a-z]", var.test_user_password)) &&
      can(regex("[0-9]", var.test_user_password))
    )
    error_message = "test_user_password must be at least 8 characters and include upper-case, lower-case, and numeric characters."
  }
}
