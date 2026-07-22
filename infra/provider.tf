provider "doppler" {
  doppler_token = var.DOPPLER_TOKEN
}

provider "upstash" {
  email   = var.UPSTASH_EMAIL
  api_key = var.UPSTASH_API_KEY
}

provider "aws" {
  region     = var.aws_region
  access_key = var.AWS_ACCESS_KEY_ID
  secret_key = var.AWS_SECRET_ACCESS_KEY

  default_tags {
    tags = {
      Project     = var.project_name
      Environment = var.environment
      ManagedBy   = "Terraform"
      Repository  = "basic-social-media"
    }
  }
}

provider "aws" {
  alias      = "us_east_1"
  region     = "us-east-1"
  access_key = var.AWS_ACCESS_KEY_ID
  secret_key = var.AWS_SECRET_ACCESS_KEY
}
