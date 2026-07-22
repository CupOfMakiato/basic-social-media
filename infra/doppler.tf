data "doppler_secrets" "this" {
  project = var.doppler_project
  config  = var.doppler_config
}

# Local values derived from Doppler secrets
locals {
  # Environment configuration
  environment  = try(data.doppler_secrets.this.map.TF_ENVIRONMENT, "dev")
  project_name = try(data.doppler_secrets.this.map.TF_PROJECT_NAME, "socialmedia")
  aws_region   = try(data.doppler_secrets.this.map.TF_AWS_REGION, "ap-southeast-1")

  # Domain configuration
  api_domain = try(data.doppler_secrets.this.map.TF_API_DOMAIN, "https://localhost:7161")
  #   ses_domain      = try(data.doppler_secrets.this.map.TF_SES_DOMAIN, "empty")

  # Database configuration
  db_connection_string            = data.doppler_secrets.this.map.CONNECTIONSTRINGS_DEFAULTCONNECTION
  redis_connection_string         = try(data.doppler_secrets.this.map.CONNECTIONSTRINGS_REDIS, "")
  upstash_redis_connection_string = module.upstash.connection_string

  lambda_environment = {
    ASPNETCORE_ENVIRONMENT               = local.environment
    ConnectionStrings__DefaultConnection = local.db_connection_string
    ConnectionStrings__Redis             = local.redis_connection_string
    ConnectionStrings__UpstashRedis      = local.upstash_redis_connection_string
    JwtSettings__SecretKey               = data.doppler_secrets.this.map.JWTSETTINGS_SECRETKEY
    JwtSettings__Issuer                  = local.api_domain
    JwtSettings__Audience                = local.api_domain
    EncryptionSettings__AesKey           = data.doppler_secrets.this.map.ENCRYPTIONSETTINGS_AESKEY
  }

  #r2 configuration
  r2_access_key_id     = data.doppler_secrets.this.map.R2_ACCESS_KEY_ID
  r2_secret_access_key = data.doppler_secrets.this.map.R2_SECRET_KEY
  r2_bucket_name       = data.doppler_secrets.this.map.R2_BUCKET_NAME
  r2_account_id        = data.doppler_secrets.this.map.R2_ACCOUNT_ID
  r2_public_endpoint   = data.doppler_secrets.this.map.R2_PUBLIC_ENDPOINT
}
