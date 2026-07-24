# =============================================================================
# Cognito Module - User Authentication
# =============================================================================

locals {
  frontend_url          = trimsuffix(var.frontend_url, "/")
  google_client_id      = var.google_client_id == null ? "" : trimspace(var.google_client_id)
  google_client_secret  = var.google_client_secret == null ? "" : trimspace(var.google_client_secret)
  google_client_id_set  = nonsensitive(local.google_client_id != "")
  google_secret_set     = nonsensitive(local.google_client_secret != "")
  google_enabled        = local.google_client_id_set && local.google_secret_set
  google_partial_config = local.google_client_id_set != local.google_secret_set
  test_accounts = {
    admin = {
      email    = var.test_admin_email
      password = var.test_admin_password
      group    = aws_cognito_user_group.admins.name
    }
    user = {
      email    = var.test_user_email
      password = var.test_user_password
      group    = aws_cognito_user_group.users.name
    }
  }
}

# User Pool
resource "aws_cognito_user_pool" "main" {
  name = "${var.project_name}-${var.environment}-users"

  username_attributes      = ["email"]
  auto_verified_attributes = ["email"]

  schema {
    name                = "email"
    attribute_data_type = "String"
    required            = true
    mutable             = true
  }

  schema {
    name                = "role"
    attribute_data_type = "String"
    required            = false
    mutable             = true
  }

  password_policy {
    minimum_length                   = 8
    require_uppercase                = true
    require_lowercase                = true
    require_numbers                  = true
    require_symbols                  = false
    temporary_password_validity_days = 7
  }

  mfa_configuration = "OPTIONAL"

  software_token_mfa_configuration {
    enabled = true
  }

  account_recovery_setting {
    recovery_mechanism {
      name     = "verified_email"
      priority = 1
    }
  }

  deletion_protection = var.environment == "production" ? "ACTIVE" : "INACTIVE"

  lifecycle {
    ignore_changes = [schema]

    precondition {
      condition     = !local.google_partial_config
      error_message = "Google OAuth requires both google_client_id and google_client_secret, or neither."
    }
  }
}

# User Pool Client
resource "aws_cognito_user_pool_client" "web_client" {
  name         = "${var.project_name}-${var.environment}-web-client"
  user_pool_id = aws_cognito_user_pool.main.id

  explicit_auth_flows = [
    "ALLOW_USER_SRP_AUTH",
    "ALLOW_REFRESH_TOKEN_AUTH"
  ]

  allowed_oauth_flows_user_pool_client = true
  allowed_oauth_flows                  = ["code"]
  allowed_oauth_scopes                 = ["email", "openid", "profile"]

  callback_urls = [
    "${local.frontend_url}/auth/callback"
  ]

  logout_urls = [
    local.frontend_url
  ]

  supported_identity_providers = concat(
    ["COGNITO"],
    local.google_enabled ? ["Google"] : []
  )

  access_token_validity  = 15
  id_token_validity      = 15
  refresh_token_validity = 30

  token_validity_units {
    access_token  = "minutes"
    id_token      = "minutes"
    refresh_token = "days"
  }

  prevent_user_existence_errors = "ENABLED"
  enable_token_revocation       = true

  depends_on = [aws_cognito_identity_provider.google]
}

# User Pool Domain
resource "aws_cognito_user_pool_domain" "main" {
  domain       = "${var.project_name}-${var.environment}"
  user_pool_id = aws_cognito_user_pool.main.id
}

# User Groups

resource "aws_cognito_user_group" "admins" {
  name         = "Admins"
  user_pool_id = aws_cognito_user_pool.main.id
  description  = "Administrators"
  precedence   = 1
}

resource "aws_cognito_user_group" "users" {
  name         = "Users"
  user_pool_id = aws_cognito_user_pool.main.id
  description  = "Users group"
  precedence   = 2
}

resource "aws_cognito_user" "test" {
  for_each = local.test_accounts

  user_pool_id   = aws_cognito_user_pool.main.id
  username       = each.value.email
  password       = each.value.password
  message_action = "SUPPRESS"

  attributes = {
    email          = each.value.email
    email_verified = "true"
  }
}

resource "aws_cognito_user_in_group" "test" {
  for_each = local.test_accounts

  user_pool_id = aws_cognito_user_pool.main.id
  group_name   = each.value.group
  username     = aws_cognito_user.test[each.key].username
}

# Google OAuth Identity Provider
resource "aws_cognito_identity_provider" "google" {
  count = local.google_enabled ? 1 : 0

  user_pool_id  = aws_cognito_user_pool.main.id
  provider_name = "Google"
  provider_type = "Google"

  provider_details = {
    client_id        = local.google_client_id
    client_secret    = local.google_client_secret
    authorize_scopes = "profile email openid"
  }

  attribute_mapping = {
    email       = "email"
    username    = "sub"
    given_name  = "given_name"
    family_name = "family_name"
    picture     = "picture"
  }

  lifecycle {
    ignore_changes = [provider_details]
  }
}
