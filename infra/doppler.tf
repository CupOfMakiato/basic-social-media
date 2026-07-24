data "doppler_secrets" "this" {
  project = var.doppler_project
  config  = var.doppler_config
}

# Local values derived from Doppler secrets
locals {
  # Environment configuration
  environment  = try(data.doppler_secrets.this.map.TF_ENVIRONMENT, var.environment)
  project_name = try(data.doppler_secrets.this.map.TF_PROJECT_NAME, var.project_name)
  aws_region   = try(data.doppler_secrets.this.map.TF_AWS_REGION, var.aws_region)

  # Domain configuration
  api_domain   = try(data.doppler_secrets.this.map.TF_API_DOMAIN, "https://localhost:7161")
  frontend_url = try(data.doppler_secrets.this.map.TF_FRONTEND_URL, "http://localhost:3000")
  #   ses_domain      = try(data.doppler_secrets.this.map.TF_SES_DOMAIN, "empty")

  # Database configuration
  neon_org_id                     = data.doppler_secrets.this.map.NEON_ORG_ID
  neon_region                     = try(data.doppler_secrets.this.map.TF_NEON_REGION, "aws-ap-southeast-1")
  db_connection_string            = module.neondb.database_connection_string
  redis_connection_string         = try(data.doppler_secrets.this.map.CONNECTIONSTRINGS_REDIS, "")
  upstash_redis_connection_string = module.upstash.connection_string
  test_admin_email                = data.doppler_secrets.this.map.TEST_ADMIN_EMAIL
  test_admin_password             = data.doppler_secrets.this.map.TEST_ADMIN_PASSWORD
  test_user_email                 = data.doppler_secrets.this.map.TEST_USER_EMAIL
  test_user_password              = data.doppler_secrets.this.map.TEST_USER_PASSWORD

  lambda_environment = {
    ASPNETCORE_ENVIRONMENT               = local.environment
    ConnectionStrings__DefaultConnection = local.db_connection_string
    ConnectionStrings__Redis             = local.redis_connection_string
    ConnectionStrings__UpstashRedis      = local.upstash_redis_connection_string
    JwtSettings__SecretKey               = data.doppler_secrets.this.map.JWTSETTINGS_SECRETKEY
    JwtSettings__Issuer                  = local.api_domain
    JwtSettings__Audience                = local.api_domain
    Cognito__Authority                   = module.cognito.issuer_url
    Cognito__ClientId                    = module.cognito.web_client_id
    EncryptionSettings__AesKey           = data.doppler_secrets.this.map.ENCRYPTIONSETTINGS_AESKEY
  }

  #r2 configuration
  r2_access_key_id     = data.doppler_secrets.this.map.R2_ACCESS_KEY_ID
  r2_secret_access_key = data.doppler_secrets.this.map.R2_SECRET_KEY
  r2_bucket_name       = data.doppler_secrets.this.map.R2_BUCKET_NAME
  r2_account_id        = data.doppler_secrets.this.map.R2_ACCOUNT_ID
  r2_public_endpoint   = data.doppler_secrets.this.map.R2_PUBLIC_ENDPOINT
  r2_location          = try(data.doppler_secrets.this.map.R2_LOCATION, "apac")
}

resource "doppler_secret" "database_connection_string" {
  project = var.doppler_project
  config  = var.doppler_config
  name    = "CONNECTIONSTRINGS_DEFAULTCONNECTION"
  value   = module.neondb.database_connection_string
}
