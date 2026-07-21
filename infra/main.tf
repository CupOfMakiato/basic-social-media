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

module "api_gateway" {
  source = "./modules/api-gateway"

  name                 = "${local.project_name}-${local.environment}-api"
  lambda_function_name = module.api_lambda.function_name
  lambda_invoke_arn    = module.api_lambda.invoke_arn
  cors_origins         = var.cors_origins
}

output "api_url" {
  value = module.api_gateway.api_endpoint
}

output "lambda_function_name" {
  value = nonsensitive(module.api_lambda.function_name)
}
