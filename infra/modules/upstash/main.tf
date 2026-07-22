resource "upstash_redis_database" "this" {
  database_name  = var.database_name
  region         = var.region
  primary_region = var.region == "global" ? var.primary_region : null
  tls            = true
  eviction       = var.eviction
}
