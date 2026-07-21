variable "database_name" {
  description = "Upstash Redis database name"
  type        = string
}

variable "region" {
  description = "Upstash Redis region"
  type        = string
  default     = "global"
}

variable "primary_region" {
  description = "Primary region when region is global"
  type        = string
  default     = "ap-southeast-1"
}

variable "eviction" {
  description = "Evict keys when the database reaches max size"
  type        = bool
  default     = true
}
