resource "cloudflare_r2_bucket" "assets" {
  account_id    = var.account_id
  name          = var.bucket_name
  location      = var.location
  storage_class = "Standard"
}
