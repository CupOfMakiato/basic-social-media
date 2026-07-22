output "github_actions_lambda_deploy_role" {
  description = "GitHub Actions Lambda deploy role with ARN and name"
  value = {
    arn  = aws_iam_role.github_actions_lambda_deploy.arn
    name = aws_iam_role.github_actions_lambda_deploy.name
  }
}

output "github_actions_lambda_deploy_role_arn" {
  description = "ARN of the GitHub Actions Lambda deploy role"
  value       = aws_iam_role.github_actions_lambda_deploy.arn
}

output "github_oidc_provider_arn" {
  description = "ARN of the GitHub Actions OIDC provider"
  value       = aws_iam_openid_connect_provider.github_actions.arn
}
