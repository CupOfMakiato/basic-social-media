data "aws_cloudfront_cache_policy" "caching_disabled" {
  name = "Managed-CachingDisabled"
}

data "aws_cloudfront_origin_request_policy" "all_viewer_except_host_header" {
  name = "Managed-AllViewerExceptHostHeader"
}

resource "aws_cloudfront_distribution" "api" {
  enabled         = true
  is_ipv6_enabled = true

  retain_on_delete    = true
  wait_for_deployment = false

  comment     = "${var.project_name} API CDN"
  price_class = "PriceClass_All"
  aliases     = var.custom_domain != "" ? [var.custom_domain] : []
  web_acl_id  = var.web_acl_id

  origin {
    domain_name = replace(var.api_gateway_domain, "https://", "")
    origin_id   = "apigateway"
    origin_path = ""

    custom_origin_config {
      http_port              = 80
      https_port             = 443
      origin_protocol_policy = "https-only"
      origin_ssl_protocols   = ["TLSv1.2"]
    }
  }

  default_cache_behavior {
    target_origin_id       = "apigateway"
    viewer_protocol_policy = "redirect-to-https"
    allowed_methods        = ["DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT"]
    cached_methods         = ["GET", "HEAD", "OPTIONS"]
    compress               = true

    cache_policy_id          = data.aws_cloudfront_cache_policy.caching_disabled.id
    origin_request_policy_id = data.aws_cloudfront_origin_request_policy.all_viewer_except_host_header.id
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    cloudfront_default_certificate = var.custom_domain == ""
    acm_certificate_arn            = var.custom_domain != "" ? aws_acm_certificate.api[0].arn : null
    ssl_support_method             = var.custom_domain != "" ? "sni-only" : null
    minimum_protocol_version       = "TLSv1.2_2021"
  }

  tags = {
    Name        = "${var.project_name}-api-cdn"
    Environment = var.environment
    Project     = var.project_name
  }

  depends_on = [aws_acm_certificate_validation.api]
}
