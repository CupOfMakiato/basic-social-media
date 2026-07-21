terraform {
  required_version = ">= 1.5"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.28"
    }
    doppler = {
      source  = "dopplerhq/doppler"
      version = "~> 1.21"
    }
    awsutils = {
      source  = "cloudposse/awsutils"
      version = ">= 0.11.0"
    }
    upstash = {
      source  = "upstash/upstash"
      version = "~> 2.1.0"
    }
  }
}
