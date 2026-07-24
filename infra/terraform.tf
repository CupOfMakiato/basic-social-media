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
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 5.22"
    }
    neon = {
      source  = "kislerdm/neon"
      version = "~> 0.14"
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
