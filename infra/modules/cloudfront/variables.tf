# =============================================================================
# CloudFront Module - Variable Declarations
# =============================================================================

variable "project_name" {
  description = "Name of the project, used for resource naming"
  type        = string
}

variable "environment" {
  description = "Environment name"
  type        = string
}

variable "api_gateway_domain" {
  description = "Domain name of the API Gateway origin"
  type        = string
}

variable "custom_domain" {
  description = "Custom domain name for the API. Leave empty to use CloudFront default domain"
  type        = string
  default     = ""
}

variable "web_acl_id" {
  description = "AWS WAF Web ACL ARN to associate with the CloudFront distribution"
  type        = string
  default     = ""
}
