variable "function_name" {
  description = "Lambda function name"
  type        = string
}

variable "package_file" {
  description = "Path to the Lambda zip package"
  type        = string
}

variable "handler" {
  description = ".NET Lambda handler"
  type        = string
}

variable "runtime" {
  description = "Lambda runtime"
  type        = string
  default     = "dotnet8"
}

variable "memory_size" {
  description = "Lambda memory in MB"
  type        = number
  default     = 512
}

variable "timeout" {
  description = "Lambda timeout in seconds"
  type        = number
  default     = 30
}

variable "log_retention_days" {
  description = "CloudWatch log retention"
  type        = number
  default     = 14
}

variable "environment_variables" {
  description = "Lambda environment variables"
  type        = map(string)
  sensitive   = true
  default     = {}
}
