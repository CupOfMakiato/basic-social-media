module "cognito" {
  source = "./modules/cognito"

  project_name         = local.project_name
  environment          = local.environment
  aws_region           = local.aws_region
  frontend_url         = local.frontend_url
  google_client_id     = try(data.doppler_secrets.this.map.GOOGLE_CLIENT_ID, null)
  google_client_secret = try(data.doppler_secrets.this.map.GOOGLE_CLIENT_SECRET, null)
}

module "upstash" {
  source = "./modules/upstash"

  database_name  = "${local.project_name}-${local.environment}-redis"
  primary_region = var.aws_region
}

module "api_lambda" {
  source = "./modules/lambda"

  function_name         = "${local.project_name}-${local.environment}-api"
  package_file          = var.lambda_package_path
  handler               = var.lambda_handler
  environment_variables = local.lambda_environment
}

module "iam" {
  source = "./modules/iam"

  project_name        = local.project_name
  environment         = local.environment
  github_repo         = var.github_repo
  lambda_function_arn = module.api_lambda.arn
}

module "api_gateway" {
  source = "./modules/api-gateway"

  name                 = "${local.project_name}-${local.environment}-api"
  lambda_function_name = module.api_lambda.function_name
  lambda_invoke_arn    = module.api_lambda.invoke_arn
  cors_origins         = var.cors_origins
}

module "cloudfront" {
  source = "./modules/cloudfront"

  providers = {
    aws.us_east_1 = aws.us_east_1
  }

  project_name       = local.project_name
  environment        = local.environment
  api_gateway_domain = module.api_gateway.api_endpoint
}

output "api_url" {
  value = module.api_gateway.api_endpoint
}

output "cloudfront_url" {
  value = "https://${module.cloudfront.distribution_domain}"
}

output "cognito_client_id" {
  value = nonsensitive(module.cognito.web_client_id)
}

output "cognito_user_pool_id" {
  value = nonsensitive(module.cognito.user_pool_id)
}

output "cognito_hosted_ui_url" {
  value = nonsensitive(module.cognito.hosted_ui_url)
}

output "lambda_function_name" {
  value = nonsensitive(module.api_lambda.function_name)
}

output "github_actions_lambda_deploy_role_arn" {
  value = module.iam.github_actions_lambda_deploy_role_arn
}
